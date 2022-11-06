﻿using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Logging;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using ManagedBass;
using PCLUntils.Objects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Media
{
    [PseudoClasses(":empty")]
    [TemplatePart("PART_ImageView", typeof(Image))]
    public unsafe class FFmpegView : TemplatedControl, IPlayerView
    {
        private Image image;
        private Errors error;
        private Task playTask;
        private Bitmap bitmap;
        private int decodeStream;
        private bool _isAttached = false;
        private readonly bool isInit = false;
        private readonly DispatcherTimer timer;
        private readonly VideoStreamDecoder video;
        private readonly AudioStreamDecoder audio;
        private CancellationTokenSource cancellationToken;
        public Errors LastError => error;
        public static readonly StyledProperty<Stretch> StretchProperty =
            AvaloniaProperty.Register<FFmpegView, Stretch>(nameof(Stretch), Stretch.Uniform);
        /// <summary>
        /// Gets or sets a value controlling how the video will be stretched.
        /// </summary>
        public Stretch Stretch
        {
            get => GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }
        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromLogicalTree(e);
            timer.Stop();
            cancellationToken.Cancel();
        }
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            _isAttached = true;
            base.OnAttachedToVisualTree(e);
        }
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            _isAttached = false;
            base.OnDetachedFromVisualTree(e);
        }
        static FFmpegView()
        {
            StretchProperty.Changed.AddClassHandler<FFmpegView>(OnStretchChange);
        }
        private static void OnStretchChange(FFmpegView sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is Stretch stretch)
                sender.image.Stretch = stretch;
        }
        public FFmpegView()
        {
            video = new VideoStreamDecoder();
            audio = new AudioStreamDecoder();
            video.MediaCompleted += VideoMediaCompleted;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            isInit = Init();
        }
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            image = e.NameScope.Get<Image>("PART_ImageView");
        }
        private void VideoMediaCompleted(TimeSpan duration)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                timer.Stop();
                DisplayVideoInfo();
            });
        }
        public double? Position => video?.Position.TotalSeconds;
        public bool Play()
        {
            bool state = false;
            try
            {
                state = video.Play();
                if (!Bass.ChannelPlay(decodeStream, true))
                    error = Bass.LastError;
                timer.Start();
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return state;
        }
        public bool Play(string uri)
        {
            if (!isInit)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, "FFmpeg : dosnot initialize device");
                return false;
            }
            bool state = false;
            try
            {
                if (video.State == MediaState.None)
                {
                    video.InitDecodecVideo(uri);
                    audio.InitDecodecAudio(uri);
                    if (decodeStream != 0)
                        Bass.StreamFree(decodeStream);
                    decodeStream = Bass.CreateStream(audio.SampleRate, audio.Channels, BassFlags.Mono, StreamProcedureType.Push);
                    if (!Bass.ChannelPlay(decodeStream, true))
                        error = Bass.LastError;
                    DisplayVideoInfo();
                }
                state = video.Play();
                audio.Play();
                timer.Start();
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return state;
        }
        public bool SeekTo(int seekTime)
        {
            audio.SeekProgress(seekTime);
            return video.SeekProgress(seekTime);
        }
        public bool Pause()
        {
            try
            {
                Bass.ChannelPause(decodeStream);
                audio.Pause();
                return video.Pause();
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                return false;
            }
        }
        public bool Stop()
        {
            try
            {
                Bass.ChannelStop(decodeStream);
                audio.Stop();
                video.Stop();
                return true;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                return false;
            }
        }
        bool Init()
        {
            try
            {
                cancellationToken = new CancellationTokenSource();
                playTask = new Task(() =>
                {
                    while (true)
                    {
                        try
                        {
                            if (video.IsPlaying && _isAttached)
                            {
                                if (video.TryReadNextFrame(out var frame))
                                {
                                    var convertedFrame = video.FrameConvert(&frame);
                                    bitmap?.Dispose();
                                    bitmap = new Bitmap(PixelFormat.Bgra8888, AlphaFormat.Premul, (IntPtr)convertedFrame.data[0], new PixelSize(video.FrameWidth, video.FrameHeight), new Vector(96, 96), convertedFrame.linesize[0]);
                                    Dispatcher.UIThread.InvokeAsync(() =>
                                    {
                                        if (image.IsNotEmpty())
                                            image.Source = bitmap;
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                        }
                        try
                        {
                            if (audio.IsPlaying)
                            {
                                if (audio.TryReadNextFrame(out var frame))
                                {
                                    var bytes = audio.FrameConvertBytes(&frame);
                                    if (bytes == null) return;
                                    if (Bass.StreamPutData(decodeStream, bytes, bytes.Length) == -1)
                                        error = Bass.LastError;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                        }
                    }
                }, cancellationToken.Token);
                playTask.Start();
                return true;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, "FFmpeg Failed Init: " + ex.Message);
                return false;
            }
        }
        #region 视频信息
        private string codec;
        public string Codec => codec;
        private TimeSpan duration;
        public TimeSpan Duration => duration;
        private double videoFps;
        public double VideoFps => videoFps;
        private double frameHeight;
        public double FrameHeight => frameHeight;
        private double frameWidth;
        public double FrameWidth => frameWidth;
        private int videoBitrate;
        public int VideoBitrate => videoBitrate;
        private double sampleRate;
        public double SampleRate => sampleRate;
        private long audioBitrate;
        public long AudioBitrate => audioBitrate;
        private long audioBitsPerSample;
        public long AudioBitsPerSample => audioBitsPerSample;
        void DisplayVideoInfo()
        {
            try
            {
                duration = video.Duration;
                codec = video.CodecName;
                videoBitrate = video.Bitrate;
                frameWidth = video.FrameWidth;
                frameHeight = video.FrameHeight;
                videoFps = video.FrameRate;
                audioBitrate = audio.Bitrate;
                sampleRate = audio.SampleRate;
                audioBitsPerSample = audio.BitsPerSample;
            }
            catch { }
        }
        #endregion
    }
}
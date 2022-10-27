using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Extensions.Controls;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using ManagedBass;
using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Media
{
    [PseudoClasses(":empty")]
    [TemplatePart("PART_SurfaceView", typeof(SurfaceView))]
    public unsafe class FFmpegView : TemplatedControl, IPlayerView
    {
        private Errors error;
        private Task playTask;
        private Bitmap bitmap;
        private int decodeStream;
        private SurfaceView surface;
        private readonly DispatcherTimer timer;
        private readonly VideoStreamDecoder video;
        private readonly AudioStreamDecoder audio;
        public Errors LastError => error;
        public static readonly StyledProperty<Stretch> StretchProperty =
            AvaloniaProperty.Register<FFmpegView, Stretch>(nameof(Stretch), Stretch.Uniform);
        /// <summary>
        /// Gets or sets a value controlling how the image will be stretched.
        /// </summary>
        public Stretch Stretch
        {
            get => GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }
        public FFmpegView()
        {
            video = new VideoStreamDecoder();
            audio = new AudioStreamDecoder();
            video.MediaCompleted += VideoMediaCompleted;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            Init();
        }
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            surface = e.NameScope.Get<SurfaceView>("PART_SurfaceView");
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
            try
            {
                video.Play();
                timer.Start();
                return true;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                return false;
            }
        }
        public bool Play(string uri)
        {
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
                video.Play();
                audio.Play();
                timer.Start();
                return true;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                return false;
            }
        }
        public bool Pause()
        {
            try
            {
                audio.Pause();
                video.Pause();
                return true;
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
        void Init()
        {
            playTask = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        if (video.IsPlaying)
                        {
                            if (video.TryReadNextFrame(out var frame))
                            {
                                var convertedFrame = video.FrameConvert(&frame);
                                bitmap = new Bitmap(PixelFormat.Bgra8888, AlphaFormat.Premul, (IntPtr)convertedFrame.data[0], new PixelSize(video.FrameWidth, video.FrameHeight), new Vector(96, 96), convertedFrame.linesize[0]);
                                Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    surface?.Enqueue(bitmap);
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
            });
            playTask.Start();
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
        #endregion
    }
}
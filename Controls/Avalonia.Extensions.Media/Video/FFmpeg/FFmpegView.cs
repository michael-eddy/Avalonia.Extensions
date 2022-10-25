using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Logging;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Media
{
    [TemplatePart("PART_ImageView", typeof(Image))]
    public unsafe class FFmpegView : TemplatedControl, IVideoView
    {
        private Task PlayTask;
        private Bitmap bitmap;
        private Image image;
        private readonly VideoStreamDecoder video;
        private readonly DispatcherTimer timer;
        public FFmpegView()
        {
            video = new VideoStreamDecoder();
            video.MediaCompleted += VideoMediaCompleted;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            Init();
        }
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
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
        public bool Play(string path)
        {
            try
            {
                if (video.State == MediaState.None)
                {
                    video.InitDecodecVideo(path);
                    DisplayVideoInfo();
                }
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
        public bool Pause()
        {
            try
            {
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
            PlayTask = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        if (video.IsPlaying)
                        {
                            if (video.TryReadNextFrame(out var frame))
                            {
                                var bytes = video.FrameConvertBytes(&frame);
                                using (var stream = new MemoryStream(bytes))
                                {
                                    bitmap = Bitmap.DecodeToWidth(stream, video.FrameWidth);
                                    if (image != null)
                                        image.Source = bitmap;
                                }
                                image?.InvalidateMeasure();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                    }
                }
            });
            PlayTask.Start();
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
        void DisplayVideoInfo()
        {
            duration = video.Duration;
            codec = video.CodecName;
            videoBitrate = video.Bitrate;
            frameWidth = video.FrameWidth;
            frameHeight = video.FrameHeight;
            videoFps = video.FrameRate;
        }
        #endregion
    }
}
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Media
{
    public unsafe class FFmpegView : ContentControl
    {
        private Task PlayTask;
        private Bitmap bitmap;
        private readonly Canvas canvas;
        private readonly DecodecVideo video;
        private readonly DispatcherTimer timer;
        public FFmpegView()
        {
            video = new DecodecVideo();
            video.MediaCompleted += VideoMediaCompleted;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            canvas = new Canvas();
            Content = canvas;
            Init();
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
        public void Play(string path)
        {
            if (video.State == MediaState.None)
            {
                video.InitDecodecVideo(path);
                DisplayVideoInfo();
            }
            video.Play();
            timer.Start();
        }
        public void Pause() => video.Pause();
        public void Stop() => video.Stop();
        void Init()
        {
            PlayTask = new Task(() =>
            {
                while (true)
                {
                    if (video.IsPlaying)
                    {
                        if (video.TryReadNextFrame(out var frame))
                        {
                            var bytes = video.FrameConvertBytes(&frame);
                            using (var stream = new MemoryStream(bytes))
                                bitmap = Bitmap.DecodeToWidth(stream, video.FrameWidth);
                            canvas.InvalidateMeasure();
                        }
                    }
                }
            });
            PlayTask.Start();
        }
        #region 视频信息
        private string codec;
        private string Codec => codec;
        private TimeSpan duration;
        private TimeSpan Duration => duration;
        private double videoFps;
        private double VideoFps => videoFps;
        private double frameHeight;
        private double FrameHeight => frameHeight;
        private double frameWidth;
        private double FrameWidth => frameWidth;
        private int videoBitrate;
        private int VideoBitrate => videoBitrate;
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
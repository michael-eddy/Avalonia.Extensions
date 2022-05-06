using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using LibVLCSharp.Shared;
using System;
using System.Diagnostics;

namespace Avalonia.Extensions.Media
{
    public partial class PlayerView : UserControl
    {
        private LibVLC? libVLC;
        private MediaPlayer? MediaPlayer;
        private readonly Button playButton;
        private readonly Button rewindButton;
        private readonly VideoView videoView;
        private readonly Button forwardButton;
        public MediaItem Current { get; private set; }
        protected float SeekPosition { get; private set; }
        protected long TotalMilliseconds { get; private set; }
        public PlayerView()
        {
            videoView = new VideoView();
            var panel = new StackPanel
            {
                Orientation = Layout.Orientation.Horizontal,
                HorizontalAlignment = Layout.HorizontalAlignment.Center
            };
            rewindButton = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.REWIND) } };
            panel.Children.Add(rewindButton);
            rewindButton.Click += RewindButton_Click;
            playButton = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.PLAY) } };
            playButton.Click += PlayButton_Click;
            panel.Children.Add(playButton);
            forwardButton = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.FORWARD) } };
            panel.Children.Add(forwardButton);
            forwardButton.Click += ForwardButton_Click;
            videoView.Callback += SetPlayerInfo;
            var root = new Panel
            {
                Opacity = .8,
                Background = Brushes.Gray,
                VerticalAlignment = Layout.VerticalAlignment.Bottom
            };
            root.Children.Add(panel);
            videoView.Content = root;
        }
        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            float p = MediaPlayer.Position + SeekPosition;
            if (p >= 1) p = 0.99F;
            MediaPlayer.Position = p;
        }
        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            var p = MediaPlayer.Position - SeekPosition;
            if (p < 0) p = 0;
            MediaPlayer.Position = p;
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.IsPlaying)
                Pause();
            else
                Play();
        }
        public bool Play()
        {
            try
            {
                if (!videoView.IsDispose)
                {
                    MediaPlayer.Play();
                    playButton.Content = new PathIcon { Data = Geometry.Parse(SvgDic.PLAY) };
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return false;
        }
        public bool Play(string url) => Play(new Uri(url));
        public bool Play(MediaList medias)
        {
            if (Current != null)
                Current.DurationChanged -= Current_DurationChanged;
            Current = new MediaItem(medias);
            return Play(Current);
        }
        public bool Play(Uri uri, params string[] options)
        {
            if (Current != null)
                Current.DurationChanged -= Current_DurationChanged;
            Current = new MediaItem(libVLC, uri, options);
            return Play(Current);
        }
        public bool Play(MediaInput input, params string[] options)
        {
            if (Current != null)
                Current.DurationChanged -= Current_DurationChanged;
            Current = new MediaItem(libVLC, input, options);
            return Play(Current);
        }
        public bool Play(int fd, params string[] options)
        {
            if (Current != null)
                Current.DurationChanged -= Current_DurationChanged;
            Current = new MediaItem(libVLC, fd, options);
            return Play(Current);
        }
        public bool Play(string mrl, FromType type, params string[] options)
        {
            if (Current != null)
                Current.DurationChanged -= Current_DurationChanged;
            Current = new MediaItem(libVLC, mrl, type, options);
            return Play(Current);
        }
        private bool Play(MediaItem media)
        {
            try
            {
                if (!videoView.IsDispose)
                {
                    if (media != null)
                    {
                        Current.DurationChanged += Current_DurationChanged;
                        MediaPlayer.Play(media);
                        playButton.Content = new PathIcon { Data = Geometry.Parse(SvgDic.PLAY) };
                        return true;
                    }
                    else
                        Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, "media cannot be NULL.");
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return false;
        }
        private void Current_DurationChanged(object sender, MediaDurationChangedEventArgs e)
        {
            TotalMilliseconds = e.Duration;
            SeekPosition = 10000F / TotalMilliseconds;
        }
        public bool Pause()
        {
            try
            {
                MediaPlayer.Pause();
                playButton.Content = new PathIcon { Data = Geometry.Parse(SvgDic.PAUSE) };
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
                MediaPlayer.Stop();
                playButton.Content = new PathIcon { Data = Geometry.Parse(SvgDic.PAUSE) };
                return true;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                return false;
            }
        }
        private void SetPlayerInfo(object sender, EventArgs e)
        {
            if (videoView != null)
            {
                MediaPlayer = new MediaPlayer(libVLC);
                videoView.MediaPlayer = MediaPlayer;
                videoView.MediaPlayer.Hwnd = videoView.Hndl.Handle;
                Play(Current);
            }
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            libVLC = new LibVLC(true);
            libVLC.Log += VlcLogger_Event;
            Content = videoView;
        }
        private void VlcLogger_Event(object sender, LogEventArgs e)
        {
            Debug.WriteLine(e.FormattedLog);
            Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, e.FormattedLog);
        }
    }
}
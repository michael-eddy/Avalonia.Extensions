using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using LibVLCSharp.Shared;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Avalonia.Extensions.Media
{
    public partial class PlayerView : UserControl
    {
        private LibVLC? libVLC;
        private MediaPlayer? MediaPlayer;
        private readonly Button prevButton;
        private readonly Button playButton;
        private readonly Button nextButton;
        private readonly Button rewindButton;
        private readonly VideoView videoView;
        private readonly Button forwardButton;
        public PlayerView()
        {
            videoView = new VideoView();
            var panel = new StackPanel
            {
                Orientation = Layout.Orientation.Horizontal,
                HorizontalAlignment = Layout.HorizontalAlignment.Center
            };
            prevButton = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.PREV) } };
            panel.Children.Add(prevButton);
            prevButton.Click += PrevButton_Click;
            rewindButton = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.REWIND) } };
            panel.Children.Add(rewindButton);
            rewindButton.Click += RewindButton_Click;
            playButton = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.PLAY) } };
            playButton.Click += PlayButton_Click;
            panel.Children.Add(playButton);
            forwardButton = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.FORWARD) } };
            panel.Children.Add(forwardButton);
            forwardButton.Click += ForwardButton_Click;
            nextButton = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.SKIP) } };
            panel.Children.Add(nextButton);
            nextButton.Click += NextButton_Click;
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

        }
        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {

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
                MediaPlayer.Play();
                playButton.Content = new PathIcon { Data = Geometry.Parse(SvgDic.PLAY) };
                return true;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                return false;
            }
        }
        public bool Play(string url) => Play(new Uri(url));
        public bool Play(MediaList medias)
        {
            Current = new MediaItem(medias);
            return Play(Current);
        }
        public bool Play(Uri uri, params string[] options)
        {
            Current = new MediaItem(libVLC, uri, options);
            return Play(Current);
        }
        public bool Play(MediaInput input, params string[] options)
        {
            Current = new MediaItem(libVLC, input, options);
            return Play(Current);
        }
        public bool Play(int fd, params string[] options)
        {
            Current = new MediaItem(libVLC, fd, options);
            return Play(Current);
        }
        public bool Play(string mrl, FromType type, params string[] options)
        {
            Current = new MediaItem(libVLC, mrl, type, options);
            return Play(Current);
        }
        public bool Play(MediaItem media)
        {
            try
            {
                if (!IsMuliMedia)
                    MediaPlayer.Play(media);
                else
                    Items.Add(media);
                playButton.Content = new PathIcon { Data = Geometry.Parse(SvgDic.PLAY) };
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
                videoView.MediaPlayer = MediaPlayer;
                videoView.MediaPlayer.Hwnd = videoView.Hndl.Handle;
            }
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            libVLC = new LibVLC(enableDebugLogs: true);
            libVLC.Log += VlcLogger_Event;
            MediaPlayer = new MediaPlayer(libVLC);
            MediaPlayer.Stopped += MediaPlayer_Stopped;
            Content = videoView;
        }
        public bool IsMuliMedia
        {
            get => GetValue(IsMuliMediaProperty);
            set => SetValue(IsMuliMediaProperty, value);
        }
        public static readonly StyledProperty<bool> IsMuliMediaProperty = AvaloniaProperty.Register<PlayerView, bool>(nameof(IsMuliMedia), false);
        public MediaItem Current { get; private set; }
        private ConcurrentBag<MediaItem> Items = new ConcurrentBag<MediaItem>();
        private void MediaPlayer_Stopped(object sender, EventArgs e)
        {

        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void VlcLogger_Event(object sender, LogEventArgs e)
        {
            Debug.WriteLine(e.FormattedLog);
            Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, e.FormattedLog);
        }
    }
}
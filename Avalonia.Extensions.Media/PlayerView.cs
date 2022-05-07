﻿using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using System;
using System.Diagnostics;

namespace Avalonia.Extensions.Media
{
    public class PlayerView : UserControl, IPlayerView
    {
        private LibVLC? libVLC;
        private Button playButton;
        private Button rewindButton;
        private Button forwardButton;
        private readonly VideoView videoView;
        public MediaItem Current { get; private set; }
        protected float SeekPosition { get; private set; }
        protected long TotalMilliseconds { get; private set; }
        public MediaPlayer MediaPlayer { get; protected set; }
        static PlayerView()
        {
            SeekSecondProperty.Changed.AddClassHandler<PlayerView>(OnSeekSecondChange);
        }
        public PlayerView()
        {
            videoView = new VideoView();
            videoView.Content = InitLatout();
        }
        /// <summary>
        /// Defines the <see cref="SeekSecond"/> property.
        /// </summary>
        public static readonly StyledProperty<uint> SeekSecondProperty =
            AvaloniaProperty.Register<PlayerView, uint>(nameof(SeekSecond), 10);
        /// <summary>
        /// 
        /// </summary>
        public uint SeekSecond
        {
            get => GetValue(SeekSecondProperty);
            set => SetValue(SeekSecondProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="SeekSecond"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> LogEnableProperty =
            AvaloniaProperty.Register<PlayerView, bool>(nameof(LogEnable), false);
        /// <summary>
        /// 
        /// </summary>
        public bool LogEnable
        {
            get => GetValue(LogEnableProperty);
            set => SetValue(LogEnableProperty, value);
        }
        public virtual Panel InitLatout()
        {
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
            return root;
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
        public bool Play(string mrl, FromType type, params string[] options)
        {
            if (Current != null)
                Current.DurationChanged -= Current_DurationChanged;
            Current = new MediaItem(libVLC, mrl, type, options);
            return Play(Current);
        }
        public bool Play(MediaItem media)
        {
            try
            {
                if (!videoView.IsDispose)
                {
                    if (media != null)
                    {
                        if (Current != null)
                            Current.DurationChanged -= Current_DurationChanged;
                        Current = media;
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
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                SeekPosition = TotalMilliseconds > 0 ? SeekSecond * 1000F / TotalMilliseconds : 0;
            });
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
            try
            {
                if (videoView != null)
                {
                    MediaPlayer = new MediaPlayer(libVLC);
                    videoView.MediaPlayer = MediaPlayer;
                    videoView.MediaPlayer.Hwnd = videoView.Hndl.Handle;
                    Play(Current);
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                throw;
            }
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            try
            {
                if (LogEnable)
                {
                    libVLC = new LibVLC(true);
                    libVLC.Log += VlcLogger_Event;
                }
                else
                    libVLC = new LibVLC(false);
                Content = videoView;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                throw;
            }
        }
        private void VlcLogger_Event(object sender, LogEventArgs e)
        {
            Debug.WriteLine(e.FormattedLog);
            Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, e.FormattedLog);
        }
        private static void OnSeekSecondChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && sender is PlayerView view)
            {
                var seek = (uint)e.NewValue * 1000F;
                view.SeekPosition = seek / view.TotalMilliseconds;
            }
        }
    }
}
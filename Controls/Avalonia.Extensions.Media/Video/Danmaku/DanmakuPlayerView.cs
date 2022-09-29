using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Danmaku;
using Avalonia.Logging;
using LibVLCSharp.Shared;
using System;

namespace Avalonia.Extensions.Media
{
    public class DanmakuPlayerView : UserControl, IPlayerView, IVideoView
    {
        private LibVLC? libVLC;
        private readonly DanamukuVideoView videoView;
        public MediaItem Current { get; private set; }
        public MediaPlayer MediaPlayer { get; protected set; }
        static DanmakuPlayerView()
        {
            if (!AppBuilderDesktopExtensions.IsVideoInit)
                throw new ApplicationException("you should call UseVideoView in Program.Main init the control");
        }
        public DanmakuPlayerView()
        {
            videoView = new DanamukuVideoView();
            videoView.Content = InitLatout();
        }
        /// <summary>
        /// Defines the <see cref="LogEnable"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> LogEnableProperty =
            AvaloniaProperty.Register<DanmakuPlayerView, bool>(nameof(LogEnable), false);
        /// <summary>
        /// 
        /// </summary>
        public bool LogEnable
        {
            get => GetValue(LogEnableProperty);
            set => SetValue(LogEnableProperty, value);
        }
        public virtual IControl InitLatout()
        {
            IDanmakuView view;
            if (Runtimes.CurrentOS == OS.Windows)
                view = new DanmakuView();
            else
                view = new DanmakuNativeView();
            videoView.Callback += SetPlayerInfo;
            return view;
        }
        public bool Play()
        {
            try
            {
                if (!videoView.IsDispose)
                {
                    MediaPlayer.Play();
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
            Current = new MediaItem(medias);
            return Play(Current);
        }
        public bool Play(Uri uri, params string[] options)
        {
            Current = new MediaItem(libVLC, uri, options);
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
                if (!videoView.IsDispose)
                {
                    if (media != null)
                    {
                        Current = media;
                        MediaPlayer.Play(media);
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
        public bool Pause()
        {
            try
            {
                MediaPlayer.Pause();
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
        private void VlcLogger_Event(object sender, LogEventArgs e) =>
            Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, e.FormattedLog);
        public void LoadDanmaku(Uri uri) => videoView.LoadDanmaku(uri);
    }
}
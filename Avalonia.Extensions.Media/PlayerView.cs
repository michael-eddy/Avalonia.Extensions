using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Media;
using LibVLCSharp.Shared;
using System;
using System.Diagnostics;

namespace Avalonia.Extensions.Media
{
    public class PlayerView : UserControl
    {
        private LibVLC? libVLC;
        private MediaPlayer? MediaPlayer;
        private readonly Button lastButton;
        private readonly VideoView videoView;
        public PlayerView()
        {
            videoView = new VideoView();
            var panel = new Panel
            {
                Opacity = .8,
                Background = Brushes.Gray,
                VerticalAlignment = Layout.VerticalAlignment.Bottom
            };
            lastButton = new Button { Content = new SymbolIcon { Glyph = "&#xEB9E;" } };
            panel.Children.Add(lastButton);
            videoView.Callback += SetPlayerInfo;
            videoView.Content = panel;
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
            Content = videoView;
        }
        private void VlcLogger_Event(object sender, LogEventArgs e)
        {
            Debug.WriteLine(e.FormattedLog);
        }
    }
}
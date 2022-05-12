using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Event;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Avalonia.Extensions.Media
{
    public sealed class MusicPlayerWindow : Window, IPlayerView
    {
        private Popup popup;
        private Button prevBtn;
        private Button playBtn;
        private Button nextBtn;
        private Button menuBtn;
        private ListView playListView;
        internal readonly LibVLC libVLC;
        private readonly VideoView videoView;
        internal readonly MediaPlayer mediaPlayer;
        private readonly IndexProcesser processer;
        private readonly Dictionary<string, string> headers;
        public MusicPlayerWindow() : this(new MusicPlayerOptions()) { }
        public MusicPlayerWindow(MusicPlayerOptions options)
        {
            MinHeight = Height = 40;
            MinWidth = Width = 200;
            headers = options.Headers;
            processer = new IndexProcesser();
            SystemDecorations = SystemDecorations.None;
            Position = options.Position ?? new PixelPoint(Screens.Primary.WorkingArea.Width - 200, Screens.Primary.WorkingArea.Height - 38);
            libVLC = new LibVLC(options.EnableDebugLogs);
            if (options.UserAgent.HasValue)
                libVLC.SetUserAgent(options.UserAgent.Value.UserAgentName, options.UserAgent.Value.UserAgentValue);
            if (options.EnableDebugLogs)
                libVLC.Log += LibVlc_Log;
            mediaPlayer = new MediaPlayer(libVLC) { EnableHardwareDecoding = true };
            mediaPlayer.Stopped += MediaPlayer_Stopped;
            videoView = new VideoView { HorizontalAlignment = Layout.HorizontalAlignment.Stretch, VerticalAlignment = Layout.VerticalAlignment.Stretch };
            videoView.MediaPlayer = mediaPlayer;
            Content = InitLatout();
        }
        public Panel InitLatout()
        {
            Grid root = new Grid();
            root.RowDefinitions.Add(new RowDefinition(0, GridUnitType.Pixel));
            root.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            root.Children.Add(videoView);
            var panel = new StackPanel
            {
                Orientation = Layout.Orientation.Horizontal,
                HorizontalAlignment = Layout.HorizontalAlignment.Center,
                VerticalAlignment = Layout.VerticalAlignment.Stretch
            };
            prevBtn = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.PREVIOUS) } };
            panel.Children.Add(prevBtn);
            prevBtn.Click += PrevBtn_Click;
            playBtn = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.PLAY) } };
            playBtn.Click += PlayBtn_Click;
            panel.Children.Add(playBtn);
            nextBtn = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.NEXT) } };
            panel.Children.Add(nextBtn);
            nextBtn.Click += NextBtn_Click;
            menuBtn = new Button { Content = new PathIcon { Data = Geometry.Parse(SvgDic.MENU) } };
            panel.Children.Add(menuBtn);
            menuBtn.Click += MenuBtn_Click;
            root.Children.Add(panel);
            panel.SetGridDef(1, 0);
            popup = new Popup
            {
                Width = 240,
                Height = 100,
                IsOpen = false,
                PlacementTarget = menuBtn,
                IsLightDismissEnabled = true,
                WindowManagerAddShadowHint = false,
                PlacementMode = PlacementMode.Pointer
            };
            playListView = new ListView();
            playListView.ItemTemplate = new FuncDataTemplate<MusicPlayItem>((x, _) =>
            {
                return new TextBlock
                {
                    MaxWidth = 240,
                    [!TextBlock.TextProperty] = new Binding("Name"),
                    [!TextBlock.ForegroundProperty] = new Binding("Color")
                };
            }, true);
            playListView.ItemClick += PlayListView_ItemClick;
            popup.Child = playListView;
            root.Children.Add(popup);
            return root;
        }
        private void LibVlc_Log(object sender, LogEventArgs e)
        {
            Debug.WriteLine(e.FormattedLog);
            Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, e.FormattedLog);
        }
        private void MediaPlayer_Stopped(object sender, EventArgs e) => Play();
        private void MenuBtn_Click(object sender, RoutedEventArgs e)
        {
            if (popup.IsOpen)
                popup.Close();
            else
                popup.Open();
        }
        private void NextBtn_Click(object sender, RoutedEventArgs e) => Play();
        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.IsPlaying)
                Pause();
            else
            {
                mediaPlayer.Play();
                playBtn.Content = new PathIcon { Data = Geometry.Parse(SvgDic.PAUSE) };
            }
        }
        private void PrevBtn_Click(object sender, RoutedEventArgs e)
        {
            if (processer.Prev(out string url))
            {
                if (!this.Play(url, headers))
                    Play();
                else
                    PopupToast.Show("播放失败！");
            }
        }
        private void PlayListView_ItemClick(object sender, ViewRoutedEventArgs e)
        {
            if (e.ClickMouse == MouseButton.Left && e.ClickItem is ListViewItem item && item.Content is MusicPlayItem playItem)
            {
                var idx = processer.PlayUrls.IndexOf(playItem);
                if (processer.Switch(idx, out string url))
                {
                    if (!this.Play(url, headers))
                    {
                        PopupToast.Show("播放失败！");
                        Play();
                    }
                }
            }
        }
        public bool Pause()
        {
            try
            {
                mediaPlayer.Pause();
                playBtn.Content = new PathIcon { Data = Geometry.Parse(SvgDic.PLAY) };
                return true;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                return false;
            }
        }
        public void Add(string displayName, string playUrl) => processer.Add((displayName, playUrl));
        public bool Play()
        {
            try
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (processer.GetUrl(out string url))
                    {
                        if (!this.Play(url, headers))
                        {
                            PopupToast.Show("播放失败！");
                            Play();
                        }
                    }
                });
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
                mediaPlayer.Stop();
                playBtn.Content = new PathIcon { Data = Geometry.Parse(SvgDic.PAUSE) };
                return true;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                return false;
            }
        }
    }
}
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Logging;
using LibVLCSharp.Shared;
using System;

namespace Avalonia.Extensions.Media
{
    public class PlayerView : TemplatedControl, IDisposable
    {
        private readonly Slider slider;
        private readonly LibVLC _libVlc;
        private readonly Button prev;
        private readonly Button play;
        private readonly Button next;
        private readonly VideoView videoView;
        public MediaPlayer MediaPlayer { get; }
        static PlayerView() => Core.Initialize();
        public PlayerView()
        {
            slider = new Slider();
            _libVlc = new LibVLC();
            prev = new Button { Content = "Prev" };
            play = new Button { Content = "Play", Margin = new Thickness(8, 0) };
            next = new Button { Content = "Next", Margin = new Thickness(8, 0) };
            _libVlc.SetUserAgent("avalonia", PlayerUserAgent);
            MediaPlayer = new MediaPlayer(_libVlc) { EnableHardwareDecoding = EnableHardwareDecoding };
            videoView = new VideoView { MediaPlayer = MediaPlayer };
            MediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
            MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
        }
        private void MediaPlayer_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e) => slider.Maximum = e.Time;
        private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e) => slider.Value = e.Position;
        protected override void OnInitialized()
        {
            DrawTemplate();
            base.OnInitialized();
        }
        private void DrawTemplate()
        {
            Grid canvas = new Grid();
            canvas.Children.Add(videoView);
            Grid grid = new Grid
            {
                Margin = new Thickness(16),
                VerticalAlignment = Layout.VerticalAlignment.Center,
                HorizontalAlignment = Layout.HorizontalAlignment.Center
            };
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.Children.Add(slider);
            StackPanel stackPanel = new StackPanel { Orientation = Layout.Orientation.Horizontal };
            stackPanel.Children.Add(prev);
            stackPanel.Children.Add(play);
            stackPanel.Children.Add(next);
            grid.Children.Add(stackPanel);
            Grid.SetRow(stackPanel, 1);
            canvas.Children.Add(grid);
            Template = new FuncControlTemplate((_, _) => new Decorator { Child = canvas });
            ApplyTemplate();
        }
        public static readonly StyledProperty<bool> EnableHardwareDecodingProperty =
          AvaloniaProperty.Register<PlayerView, bool>(nameof(EnableHardwareDecoding), true);
        public bool EnableHardwareDecoding
        {
            get => GetValue(EnableHardwareDecodingProperty);
            set => SetValue(EnableHardwareDecodingProperty, value);
        }
        public static readonly StyledProperty<string> PlayerUserAgentProperty =
          AvaloniaProperty.Register<PlayerView, string>(nameof(PlayerUserAgent));
        public string PlayerUserAgent
        {
            get => GetValue(PlayerUserAgentProperty);
            set => SetValue(PlayerUserAgentProperty, value);
        }
        public bool Play() => MediaPlayer.Play();
        public bool Play(string url)
        {
            var media = new MediaItem(_libVlc, url);
            return Play(media);
        }
        public bool Play(MediaItem media)
        {
            try
            {
                return MediaPlayer.Play(media);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return false;
        }
        public void Stop() => MediaPlayer.Stop();
        public void Pause() => MediaPlayer.Pause();
        public virtual void Dispose()
        {
            try
            {
                MediaPlayer.TimeChanged -= MediaPlayer_TimeChanged;
                MediaPlayer.PositionChanged -= MediaPlayer_PositionChanged;
                MediaPlayer?.Dispose();
                _libVlc?.Dispose();
            }
            catch { }
        }
    }
}
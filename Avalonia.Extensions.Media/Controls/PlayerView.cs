using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Logging;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;
using System;

namespace Avalonia.Extensions.Media
{
    public class PlayerView : TemplatedControl, IDisposable
    {
        static PlayerView() => Core.Initialize();
        private readonly Slider slider;
        private readonly LibVLC _libVlc;
        private readonly VideoView videoView;
        public MediaPlayer MediaPlayer { get; }
        public PlayerView()
        {
            slider = new Slider();
            _libVlc = new LibVLC();
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
            base.OnInitialized();
            DrawTemplate();
        }
        private void DrawTemplate()
        {
            Canvas canvas = new Canvas();
            canvas.Children.Add(videoView);
            SetPosition(videoView, 0, 0, 0, 0);
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

            grid.Children.Add(stackPanel);
            Grid.SetRow(stackPanel, 1);
            canvas.Children.Add(grid);
            SetPosition(grid, grid.Height, 0, 0, 0);
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
        private void SetPosition(Control control, double top, double left, double right, double bottom)
        {
            Canvas.SetTop(control, top);
            Canvas.SetBottom(control, bottom);
            Canvas.SetLeft(control, left);
            Canvas.SetRight(control, right);
        }
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
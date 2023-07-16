using Avalonia.Interactivity;

namespace Avalonia.Extensions.WebView
{
    public sealed class FullscreenModeChangeEventArgs : RoutedEventArgs
    {
        public bool Fullscreen { get; }
        public FullscreenModeChangeEventArgs(Interactive source, bool fullscreen) : base(WebView.FullscreenEvent, source) =>
            Fullscreen = fullscreen;
    }
}
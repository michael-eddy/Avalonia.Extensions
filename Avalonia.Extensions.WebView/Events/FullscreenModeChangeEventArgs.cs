using Avalonia.Extensions.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Extensions.Events
{
    public sealed class FullscreenModeChangeEventArgs : RoutedEventArgs
    {
        public bool Fullscreen { get; }
        public FullscreenModeChangeEventArgs(IInteractive source, bool fullscreen) : base(WebView.FullscreenEvent, source) =>
            Fullscreen = fullscreen;
    }
}
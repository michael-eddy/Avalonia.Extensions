using Avalonia.Extensions.Events;
using Avalonia.Interactivity;
using System;

namespace Avalonia.Extensions.Controls
{
    sealed class WebView : CefNet.Avalonia.WebView
    {
        public static RoutedEvent<FullscreenModeChangeEventArgs> FullscreenEvent =
            RoutedEvent.Register<WebViewGlue, FullscreenModeChangeEventArgs>("Fullscreen", RoutingStrategies.Bubble);
        public event EventHandler<FullscreenModeChangeEventArgs> Fullscreen
        {
            add { AddHandler(FullscreenEvent, value); }
            remove { RemoveHandler(FullscreenEvent, value); }
        }
        public WebView() { }
        public WebView(CefNet.Avalonia.WebView opener) : base(opener) { }
        protected override CefNet.Internal.WebViewGlue CreateWebViewGlue() => new WebViewGlue(this);
        internal void RaiseFullscreenModeChange(bool fullscreen) =>
            RaiseCrossThreadEvent(OnFullScreenModeChange, new FullscreenModeChangeEventArgs(this, fullscreen), false);
        private void OnFullScreenModeChange(FullscreenModeChangeEventArgs e) => RaiseEvent(e);
    }
}
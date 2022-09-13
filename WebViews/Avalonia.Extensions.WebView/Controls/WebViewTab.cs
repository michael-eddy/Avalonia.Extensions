using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using CefNet;
using System;
using Avalonia.VisualTree;

namespace Avalonia.Extensions.WebView
{
    public class WebViewTab : TabItem, IStyleable
    {
        private class ColoredFormattedText : FormattedText
        {
            public IBrush Brush { get; set; }
        }
        private class WebViewTabTitle : TemplatedControl
        {
            private readonly WebViewTab _tab;
            private ColoredFormattedText _xButton;
            private IBrush _xbuttonBrush;
            public WebViewTabTitle(WebViewTab tab) => _tab = tab;
            public string Text
            {
                get => FormattedText?.Text;
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        FormattedText = null;
                        InvalidateMeasure();
                        return;
                    }
                    FormattedText = new ColoredFormattedText
                    {
                        Text = value,
                        Typeface = new Typeface(FontFamily, FontStyle, FontWeight),
                        FontSize = FontSize,
                        Brush = Brushes.Black,
                    };
                    InvalidateMeasure();
                }
            }
            private ColoredFormattedText FormattedText { get; set; }
            private ColoredFormattedText XButton
            {
                get
                {
                    _xButton ??= new ColoredFormattedText
                    {
                        Text = "x",
                        Typeface = new Typeface(FontFamily, FontStyle, FontWeight.Bold),
                        FontSize = FontSize,
                        Brush = Brushes.Gray,
                    };
                    return _xButton;
                }
            }
            protected override Size MeasureOverride(Size constraint)
            {
                var ft = FormattedText;
                if (ft == null)
                    return base.MeasureOverride(constraint);
                var ftbounds = ft.Bounds;
                return new Size(ftbounds.Width + XButton.Bounds.Width + 4, ftbounds.Height);
            }
            protected override void OnPointerReleased(PointerReleasedEventArgs e)
            {
                if (e.InitialPressMouseButton == MouseButton.Left)
                {
                    if (GetXButtonRect().Contains(e.GetPosition(this)))
                        _tab.Close();
                }
                base.OnPointerReleased(e);
            }
            private Rect GetXButtonRect()
            {
                var xbounds = XButton.Bounds;
                return new Rect(Bounds.Width - xbounds.Width, 0, xbounds.Width, xbounds.Height);
            }
            protected override void OnPointerMoved(PointerEventArgs e)
            {
                SetXButtonBrush(GetXButtonRect().Contains(e.GetPosition(this)) ? Brushes.Black : Brushes.Gray);
                base.OnPointerMoved(e);
            }
            protected override void OnPointerLeave(PointerEventArgs e)
            {
                SetXButtonBrush(Brushes.Gray);
                base.OnPointerLeave(e);
            }
            private void SetXButtonBrush(ISolidColorBrush brush)
            {
                if (brush != _xbuttonBrush)
                {
                    _xbuttonBrush = brush;
                    XButton.Brush = brush;
                    InvalidateVisual();
                }
            }
            public override void Render(DrawingContext drawingContext)
            {
                var formattedText = FormattedText;
                if (formattedText == null) return;
                drawingContext.DrawText(formattedText.Brush, new Point(), formattedText);
                drawingContext.DrawText(XButton.Brush, new Point(Bounds.Width - XButton.Bounds.Width, 0), XButton);
            }
        }
        public WebViewTab() : this(new CefNet.Avalonia.WebView()) { }
        private WebViewTab(CefNet.Avalonia.WebView webview)
        {
            webview.CreateWindow += Webview_CreateWindow;
            webview.DocumentTitleChanged += HandleDocumentTitleChanged;
            WebView = webview;
            Header = new WebViewTabTitle(this);
        }
        Type IStyleable.StyleKey => typeof(TabItem);
        public string Title
        {
            get => ((WebViewTabTitle)Header).Text;
            set => ((WebViewTabTitle)Header).Text = value;
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Content = WebView;
        }
        public void Close()
        {
            WebView.Close();
            if (Parent is not TabControl tabs) return;
            ((AvaloniaList<object>)tabs.Items).Remove(this);
        }
        private void HandleDocumentTitleChanged(object sender, DocumentTitleChangedEventArgs e) => Title = e.Title;
        public IChromiumWebView WebView { get; protected set; }
        public bool PopupHandlingDisabled { get; set; }
        private void Webview_CreateWindow(object sender, CreateWindowEventArgs e)
        {
            if (PopupHandlingDisabled) return;
            var tabs = this.FindTabControl();
            if (tabs == null)
            {
                e.Cancel = true;
                return;
            }
            if (this.GetVisualRoot() is not Window avaloniaWindow)
                throw new InvalidOperationException("Window not found!");
            var webview = new WebView((WebView)WebView);
            IPlatformHandle platformHandle = avaloniaWindow.PlatformImpl.Handle;
            if (platformHandle is IMacOSTopLevelPlatformHandle macOSHandle)
                e.WindowInfo.SetAsWindowless(macOSHandle.GetNSWindowRetained());
            else
                e.WindowInfo.SetAsWindowless(platformHandle.Handle);
            e.Client = webview.Client;
            OnCreateWindow(webview);
        }
        protected void OnCreateWindow(CefNet.Avalonia.WebView webview)
        {
            var tab = new WebViewTab(webview);
            var tabs = this.FindTabControl();
            ((AvaloniaList<object>)tabs.Items).Add(tab);
            tabs.SelectedItem = tab;
        }
    }
}
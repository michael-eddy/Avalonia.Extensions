using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.VisualTree;
using CefNet;
using PCLUntils.Assemblly;
using System;
using System.Globalization;

namespace Avalonia.Extensions.WebView
{
    public class WebViewTab : TabItem
    {
        private class ColoredFormattedText : FormattedText
        {
            private readonly string textToFormat;
            public ColoredFormattedText(string textToFormat, Typeface typeface, double emSize, IBrush foreground) :
                base(textToFormat, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, emSize, foreground)
            {
                this.textToFormat = textToFormat;
            }
            public IBrush Brush { get; set; }
            public string Text => textToFormat;
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
                    FormattedText = new ColoredFormattedText(value, new Typeface(FontFamily, FontStyle, FontWeight), FontSize, Brushes.Black);
                    InvalidateMeasure();
                }
            }
            private ColoredFormattedText FormattedText { get; set; }
            private ColoredFormattedText XButton
            {
                get
                {
                    _xButton ??= new ColoredFormattedText("x", new Typeface(FontFamily, FontStyle, FontWeight.Bold), FontSize, Brushes.Gray);
                    return _xButton;
                }
            }
            protected override Size MeasureOverride(Size constraint)
            {
                var ft = FormattedText;
                if (ft == null)
                    return base.MeasureOverride(constraint);
                return new Size(ft.Width + XButton.Width + 4, ft.Height);
            }
            protected override void OnPointerReleased(PointerReleasedEventArgs e)
            {
                if (e.InitialPressMouseButton == MouseButton.Left)
                {
                    if (XButtonRect.Contains(e.GetPosition(this)))
                        _tab.Close();
                }
                base.OnPointerReleased(e);
            }
            private Rect XButtonRect => new Rect(Bounds.Width - XButton.Width, 0, XButton.Width, XButton.Height);
            protected override void OnPointerMoved(PointerEventArgs e)
            {
                SetXButtonBrush(XButtonRect.Contains(e.GetPosition(this)) ? Brushes.Black : Brushes.Gray);
                base.OnPointerMoved(e);
            }
            protected override void OnPointerExited(PointerEventArgs e)
            {
                SetXButtonBrush(Brushes.Gray);
                base.OnPointerExited(e);
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
                drawingContext.DrawText(formattedText, new Point(0, 0));
                drawingContext.DrawText(XButton, new Point(Bounds.Width - XButton.Width, 0));
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
            ((AvaloniaList<object>)tabs.ItemsSource).Remove(this);
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
            IPlatformHandle platformHandle = avaloniaWindow.PlatformImpl.GetPrivateProperty<IPlatformHandle>("Handle");
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
            ((AvaloniaList<object>)tabs.ItemsSource).Add(tab);
            tabs.SelectedItem = tab;
        }
    }
}
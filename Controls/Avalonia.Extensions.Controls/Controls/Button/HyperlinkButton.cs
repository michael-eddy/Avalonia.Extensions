using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;

namespace Avalonia.Extensions.Controls
{
    public class HyperlinkButton : Button
    {
        private TextBlock TextContent { get; set; }
        protected override Type StyleKeyOverride => typeof(Button);
        public HyperlinkButton() : base()
        {
            SetValue(ForegroundProperty, new SolidColorBrush(Colors.Blue));
            SetValue(BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            Initialize();
            ContentProperty.Changed.AddClassHandler<HyperlinkButton>(OnContentChange);
        }
        private void Initialize()
        {
            TextContent = new TextBlock();
            TextContent.Foreground = Foreground;
            TextContent.Background = Background;
            base.Content = TextContent;
        }
        private void OnContentChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is string chars)
            {
                TextContent.Text = chars;
                TextContent.InvalidateMeasure();
                InvalidateMeasure();
            }
        }
        public new string Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="IsDefaultProperty"/> property.
        /// </summary>
        public static new readonly StyledProperty<string> ContentProperty =
            AvaloniaProperty.Register<HyperlinkButton, string>(nameof(Content));
        public Uri NavigateUri
        {
            get => GetValue(NavigateUriProperty);
            set => SetValue(NavigateUriProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="IsDefaultProperty"/> property.
        /// </summary>
        public static readonly StyledProperty<Uri> NavigateUriProperty =
            AvaloniaProperty.Register<HyperlinkButton, Uri>(nameof(NavigateUri));
    }
}
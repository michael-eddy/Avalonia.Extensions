using Avalonia.Controls;
using Avalonia.Media;

namespace Avalonia.Extensions.Controls
{
    internal sealed class Text : Control
    {
        public static readonly StyledProperty<IBrush> ForegroundProperty =
            AvaloniaProperty.Register<Text, IBrush>("Foreground", new SolidColorBrush(Colors.Black));
        public static readonly StyledProperty<int> FontSizeProperty = AvaloniaProperty.Register<Text, int>("FontSize", 14);
        public static readonly StyledProperty<string> ContentProperty = AvaloniaProperty.Register<Text, string>("Content");
        public string Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        public IBrush Foreground
        {
            get => GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        public int FontSize
        {
            get => GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        public Text()
        {
            AffectsRender<Text>(ContentProperty);
            AffectsRender<Text>(ForegroundProperty);
        }
        public override void Render(DrawingContext context)
        {
            var formattedText = new FormattedText(Content, Typeface.Default, FontSize, TextAlignment.Left, TextWrapping.NoWrap, new Size(300, 300));
            context.DrawText(Foreground, new Point(0, 0), formattedText);
        }
    }
}
using Avalonia.Controls;
using Avalonia.Media;

namespace Avalonia.Extensions.Controls
{
    public sealed class Text : Control
    {
        public static readonly StyledProperty<string> ContentProperty =
            AvaloniaProperty.Register<Text, string>("Content");
        public string Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        public Text()
        {
            AffectsRender<Text>(ContentProperty);
        }
        public override void Render(DrawingContext context)
        {
            var formattedText = new FormattedText(Content, Typeface.Default, 12, TextAlignment.Left,
                 TextWrapping.Wrap, new Size(300, 300));
            context.DrawText(new SolidColorBrush(Colors.Transparent), new Point(10, 0), formattedText);
        }
    }
}
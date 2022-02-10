using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Drawing;

namespace Avalonia.Extensions.Controls
{
    public sealed class Text : Shape
    {
        public static readonly StyledProperty<string> ContentProperty = AvaloniaProperty.Register<Text, string>(nameof(Content));
        public string Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        public static readonly StyledProperty<IBrush> ForegroundProperty = AvaloniaProperty.Register<Text, IBrush>(nameof(Foreground), new SolidColorBrush(Colors.Black));
        public IBrush Foreground
        {
            get => GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        public static readonly StyledProperty<int> FontSizeProperty = AvaloniaProperty.Register<Text, int>(nameof(FontSize), 14);
        public int FontSize
        {
            get => GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        public Text()
        {
            AffectsRender<Text>(ContentProperty);
            AffectsRender<Text>(ForegroundProperty);
            SetValue(StrokeThicknessProperty, 2);
        }
        public override void Render(DrawingContext context)
        {
            var formattedText = new FormattedText(Content, Typeface.Default, FontSize, TextAlignment.Left, TextWrapping.NoWrap, MeasureStringSize);
            context.DrawText(Foreground, new Point(0, 0), formattedText);
        }
        protected override Size MeasureOverride(Size availableSize) => MeasureStringSize;
        protected override Geometry CreateDefiningGeometry() => new RectangleGeometry(new Rect(MeasureStringSize).Deflate(StrokeThickness / 2));
        private Size MeasureStringSize
        {
            get
            {
                try
                {
                    using Bitmap bitmap = new Bitmap(1, 1);
                    var graphics = Graphics.FromImage(bitmap);
                    StringFormat sf = StringFormat.GenericTypographic;
                    sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                    var size = graphics.MeasureString(Content.Trim(), new Font(Typeface.Default.FontFamily.Name, FontSize), PointF.Empty, sf);
                    double width = Math.Ceiling(size.Width), height = Math.Ceiling(size.Height);
                    return new Size(width, height);
                }
                catch { return new Size(); }
            }
        }
    }
}
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Metadata;
using System;
using System.Drawing;

namespace Avalonia.Extensions.Controls
{
    public sealed class Text : Shape
    {
        /// <summary>
        /// Defines the <see cref="Content"/> property
        /// </summary>
        public static readonly DirectProperty<Text, string?> ContentProperty =
            AvaloniaProperty.RegisterDirect<Text, string?>(nameof(Content), o => o.Content, (o, v) => o.Content = v);
        /// <summary>
        /// Gets or sets the content to display in this flyout
        /// </summary>
        [Content]
        public string Content
        {
            get { return _content; }
            set { SetAndRaise(ContentProperty, ref _content, value); }
        }
        /// <summary>
        /// Defines the <see cref="Foreground"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush> ForegroundProperty = AvaloniaProperty.Register<Text, IBrush>(nameof(Foreground), new SolidColorBrush(Colors.Black));
        /// <summary>
        /// Gets or sets a brush used to paint the text.
        /// </summary>
        public IBrush Foreground
        {
            get => GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="FontSize"/> property.
        /// </summary>
        public static readonly StyledProperty<int> FontSizeProperty = AvaloniaProperty.Register<Text, int>(nameof(FontSize), 14);
        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public int FontSize
        {
            get => GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        private string _content;
        public Text()
        {
            _content = string.Empty;
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
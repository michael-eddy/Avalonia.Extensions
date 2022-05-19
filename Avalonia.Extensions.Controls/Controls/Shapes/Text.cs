using Avalonia.Controls.Shapes;
using Avalonia.Extensions.Styles;
using Avalonia.Media;
using Avalonia.Metadata;
using SkiaSharp;
using System;
using FontFamily = Avalonia.Media.FontFamily;

namespace Avalonia.Extensions.Controls
{
    public class Text : Shape
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
        public string? Content
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
        private string? _content;
        private Size _constraint;
        private TextLayout _textLayout;
        private SKTypeface DefaultTypeface { get; }
        private FontFamily DefaultFontFamily { get; }
        internal TextLayout TextLayout => _textLayout ??= CreateTextLayout(_constraint, _content);
        public Text()
        {
            _content = string.Empty;
            AffectsRender<Text>(ContentProperty);
            AffectsRender<Text>(ForegroundProperty);
            SetValue(StrokeThicknessProperty, 2);
            DefaultFontFamily = new FontFamily(FontManager.Current.DefaultFontFamilyName);
            DefaultTypeface = SKTypeface.FromFamilyName(DefaultFontFamily.Name);
        }
        protected void InvalidateTextLayout() => _textLayout = null;
        internal virtual TextLayout CreateTextLayout(Size constraint, string? text)
        {
            if (constraint == Size.Empty)
                return null;
            return new TextLayout(text ?? string.Empty, new Typeface(DefaultFontFamily), FontSize, Foreground);
        }
        public override void Render(DrawingContext context)
        {
            context.FillRectangle(Brushes.Transparent, new Rect(Bounds.Size));
            if (TextLayout == null)
                return;
            using (context.PushPostTransform(Matrix.CreateTranslation(0, 0)))
                TextLayout.Draw(context);
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            if (string.IsNullOrEmpty(_content))
                return default;
            var padding = new Thickness(StrokeThickness);
            availableSize = availableSize.Deflate(padding);
            if (_constraint != availableSize)
            {
                _constraint = availableSize;
                InvalidateTextLayout();
            }
            return (TextLayout?.Size ?? Size.Empty).Inflate(padding);
        }
        protected override Geometry CreateDefiningGeometry() => new RectangleGeometry(new Rect(MeasureStringSize).Deflate(StrokeThickness / 2));
        private Size MeasureStringSize
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Content))
                    {
                        var size = Content.Trim().MeasureString(FontSize, DefaultTypeface);
                        double width = Math.Ceiling(size.Width), height = Math.Ceiling(size.Height);
                        return new Size(width, height);
                    }
                }
                catch { }
                return new Size();
            }
        }
    }
}
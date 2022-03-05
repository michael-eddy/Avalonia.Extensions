using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Extensions.Styles;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Reflection;

namespace Avalonia.Extensions.Controls
{
    public class EditTextView : TextBox, IStyling
    {
        private static TextPresenter _textPresenter;
        Type IStyleable.StyleKey => typeof(TextBox);
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _textPresenter = e.NameScope.Get<TextPresenter>("PART_TextPresenter");
            var newMethod = GetType().GetMethod("RenderInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            var m = _textPresenter.GetType().GetMethod("RenderInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            AssemblyUntils.ReplaceMethod(m, newMethod);
            SetValue(TextBlock.ForegroundProperty, Foreground);
        }
        internal void RenderInternal(DrawingContext context)
        {
            IBrush background = Background;
            if (background != null)
                context.FillRectangle(background, new Rect(Bounds.Size));
            var _text = _textPresenter.GetPrivateField<string>("_text");
            var _constraint = _textPresenter.GetPrivateField<Size>("_constraint");
            var TextLayout = CreateTextLayout(_constraint, _text);
            TextLayout.Draw(context);
        }
        internal TextLayout CreateTextLayout(Size constraint, string text)
        {
            if (constraint == Size.Empty)
                return null;
            return new TextLayout(text ?? string.Empty, new Typeface(FontFamily, FontStyle, FontWeight), FontSize, Foreground, TextAlignment, TextWrapping, TextTrimming, TextDecorations,
                constraint.Width, constraint.Height, maxLines: MaxLines, lineHeight: LineHeight);
        }
        public static readonly StyledProperty<TextDecorationCollection> TextDecorationsProperty =
            AvaloniaProperty.Register<TextLabel, TextDecorationCollection>(nameof(TextDecorations));
        public TextDecorationCollection TextDecorations
        {
            get => GetValue(TextDecorationsProperty);
            set => SetValue(TextDecorationsProperty, value);
        }
        public static readonly StyledProperty<int> MaxLinesProperty =
            AvaloniaProperty.Register<TextLabel, int>(nameof(MaxLines), validate: IsValidMaxLines);
        private static bool IsValidMaxLines(int maxLines) => maxLines >= 0;
        public int MaxLines
        {
            get => GetValue(MaxLinesProperty);
            set => SetValue(MaxLinesProperty, value);
        }
        public static readonly StyledProperty<TextTrimming> TextTrimmingProperty =
            AvaloniaProperty.Register<TextLabel, TextTrimming>(nameof(TextTrimming));
        public TextTrimming TextTrimming
        {
            get => GetValue(TextTrimmingProperty);
            set => SetValue(TextTrimmingProperty, value);
        }
        public static readonly StyledProperty<double> LineHeightProperty =
            AvaloniaProperty.Register<TextLabel, double>(nameof(LineHeight), double.NaN, validate: IsValidLineHeight);
        public double LineHeight
        {
            get => GetValue(LineHeightProperty);
            set => SetValue(LineHeightProperty, value);
        }
        private static bool IsValidLineHeight(double lineHeight)
        {
            if (!double.IsNaN(lineHeight))
                return lineHeight > 0.0;
            return true;
        }
    }
}
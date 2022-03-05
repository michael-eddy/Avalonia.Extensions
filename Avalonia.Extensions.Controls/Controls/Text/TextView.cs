using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Extensions.Styles;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;

namespace Avalonia.Extensions.Controls
{
	public class TextView : TextPresenter
	{
		private Size _constraint;
		private TextLayout _textLayout;
		private string _text => this.GetPrivateField<string>("_text");
		internal TextLayout TextLayout => _textLayout ??= CreateTextLayout(_constraint, _text);
		static TextView()
		{
			CaretIndexProperty.Changed.AddClassHandler<TextView>((x, e) => x.CaretIndexChanged((int)e.NewValue));
		}
		public static readonly StyledProperty<TextTrimming> TextTrimmingProperty =
			AvaloniaProperty.Register<TextLabel, TextTrimming>(nameof(TextTrimming));
		public TextTrimming TextTrimming
		{
			get => GetValue(TextTrimmingProperty);
			set => SetValue(TextTrimmingProperty, value);
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
		public int MaxLines
		{
			get => GetValue(MaxLinesProperty);
			set => SetValue(MaxLinesProperty, value);
		}
		private static bool IsValidMaxLines(int maxLines) => maxLines >= 0;
		public static readonly StyledProperty<double> LineHeightProperty =
			AvaloniaProperty.Register<TextLabel, double>(nameof(LineHeight), double.NaN, validate: IsValidLineHeight);
		public double LineHeight
		{
			get => GetValue(LineHeightProperty);
			set => SetValue(LineHeightProperty, value);
		}
		public static new readonly DirectProperty<TextView, int> CaretIndexProperty =
           TextBox.CaretIndexProperty.AddOwner<TextView>(o => o.CaretIndex, (o, v) => o.CaretIndex = v);
		public new int CaretIndex
		{
			get => this.GetPrivateField<int>("_caretIndex");
			set => base.CaretIndex = value;
		}
		private static bool IsValidLineHeight(double lineHeight)
		{
			if (!double.IsNaN(lineHeight))
				return lineHeight > 0.0;
			return true;
		}
		private void RenderInternal(DrawingContext context)
		{
			IBrush background = Background;
			if (background != null)
				context.FillRectangle(background, new Rect(Bounds.Size));
			TextLayout.Draw(context);
		}
		private void CaretIndexChanged(int caretIndex)
		{
			if (this.GetVisualParent() != null)
			{
				var _caretTimer = this.GetPrivateField<DispatcherTimer>("_caretTimer");
				if (_caretTimer.IsEnabled)
				{
					this.SetPrivateField("_caretBlink", true);
					_caretTimer.Stop();
					_caretTimer.Start();
					InvalidateVisual();
				}
				else
				{
					_caretTimer.Start();
					InvalidateVisual();
					_caretTimer.Stop();
				}
				if (IsMeasureValid)
				{
					var rect = FormattedText.HitTestTextPosition(caretIndex);
					this.BringIntoView(rect);
				}
				else
				{
					Dispatcher.UIThread.Post(() =>
					{
						var rect = FormattedText.HitTestTextPosition(caretIndex);
						this.BringIntoView(rect);
					}, DispatcherPriority.Render);
				}
			}
		}
		public override void Render(DrawingContext context)
		{
			FormattedText.Constraint = Bounds.Size;
			_constraint = Bounds.Size;
			int selectionStart = SelectionStart;
			int selectionEnd = SelectionEnd;
			if (selectionStart != selectionEnd)
			{
				int start = Math.Min(selectionStart, selectionEnd);
				int length = Math.Max(selectionStart, selectionEnd) - start;
				foreach (Rect rect in FormattedText.HitTestTextRange(start, length))
					context.FillRectangle(SelectionBrush, rect);
			}
			RenderInternal(context);
			if (selectionStart != selectionEnd) return;
			IBrush caretBrush = CaretBrush?.ToImmutable();
			if (caretBrush == null)
			{
				Color? backgroundColor = (Background as ISolidColorBrush)?.Color;
				if (backgroundColor.HasValue)
				{
					byte r = (byte)~backgroundColor.Value.R;
					byte green = (byte)~backgroundColor.Value.G;
					byte blue = (byte)~backgroundColor.Value.B;
					caretBrush = new ImmutableSolidColorBrush(Color.FromRgb(r, green, blue));
				}
				else
					caretBrush = Brushes.Black;
			}
			Rect charPos = FormattedText.HitTestTextPosition(CaretIndex);
			double x = Math.Floor(charPos.X) + 0.5, y = Math.Floor(charPos.Y) + 0.5, b = Math.Ceiling(charPos.Bottom) - 0.5;
			Point p1 = new Point(x, y), p2 = new Point(x, b);
			context.DrawLine(new ImmutablePen(caretBrush), p1, p2);
		}
		internal virtual TextLayout CreateTextLayout(Size constraint, string text)
		{
			if (constraint == Size.Empty)
				return null;
			return new TextLayout(text ?? string.Empty, new Typeface(FontFamily, FontStyle, FontWeight), FontSize, Foreground, TextAlignment, TextWrapping, TextTrimming, TextDecorations,
				constraint.Width, constraint.Height, maxLines: MaxLines, lineHeight: LineHeight);
		}
	}
}
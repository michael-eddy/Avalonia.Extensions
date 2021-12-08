using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Extensions.Media;
using Avalonia.Extensions.Styles;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;

namespace Avalonia.Extensions.Controls
{
    public class RunLabel : Control, IRun, IStyling
	{
		public static readonly StyledProperty<IBrush> BackgroundProperty =
			Border.BackgroundProperty.AddOwner<RunLabel>();
		public static readonly StyledProperty<Thickness> PaddingProperty =
			Decorator.PaddingProperty.AddOwner<RunLabel>();
		public static readonly AttachedProperty<FontFamily> FontFamilyProperty =
			AvaloniaProperty.RegisterAttached<RunLabel, Control, FontFamily>(nameof(FontFamily), defaultValue: FontFamily.Default, inherits: true);
		public static readonly AttachedProperty<double> FontSizeProperty =
			AvaloniaProperty.RegisterAttached<RunLabel, Control, double>(nameof(FontSize), defaultValue: 12, inherits: true);
		public static readonly AttachedProperty<FontStyle> FontStyleProperty =
			AvaloniaProperty.RegisterAttached<RunLabel, Control, FontStyle>(nameof(FontStyle), inherits: true);
		public static readonly AttachedProperty<FontWeight> FontWeightProperty =
			AvaloniaProperty.RegisterAttached<RunLabel, Control, FontWeight>(nameof(FontWeight), inherits: true, defaultValue: FontWeight.Normal);
		public static readonly AttachedProperty<IBrush> ForegroundProperty =
			AvaloniaProperty.RegisterAttached<RunLabel, Control, IBrush>(nameof(Foreground), Brushes.Black, inherits: true);
		public static readonly StyledProperty<double> LineHeightProperty =
			AvaloniaProperty.Register<RunLabel, double>(nameof(LineHeight), double.NaN, validate: IsValidLineHeight);
		public static readonly StyledProperty<int> MaxLinesProperty =
			AvaloniaProperty.Register<RunLabel, int>(nameof(MaxLines), validate: IsValidMaxLines);
		public static readonly StyledProperty<TextAlignment> TextAlignmentProperty =
			AvaloniaProperty.Register<RunLabel, TextAlignment>(nameof(TextAlignment));
		public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
			AvaloniaProperty.Register<RunLabel, TextWrapping>(nameof(TextWrapping));
		public static readonly StyledProperty<TextTrimming> TextTrimmingProperty =
			AvaloniaProperty.Register<RunLabel, TextTrimming>(nameof(TextTrimming));
		public static readonly StyledProperty<TextDecorationCollection> TextDecorationsProperty =
			AvaloniaProperty.Register<RunLabel, TextDecorationCollection>(nameof(TextDecorations));
		private string _text;
		private Size _constraint;
		private TextLayout _textLayout;
		private AvaloniaList<string> _contents = new AvaloniaList<string>();
		Type IStyleable.StyleKey => typeof(TextBlock);
		public TextLayout TextLayout => _textLayout ??= CreateTextLayout(_constraint, _text);
		public Thickness Padding
		{
			get => GetValue(PaddingProperty);
			set => SetValue(PaddingProperty, value);
		}
		public IBrush Background
		{
			get => GetValue(BackgroundProperty);
			set => SetValue(BackgroundProperty, value);
		}
		public FontFamily FontFamily
		{
			get => GetValue(FontFamilyProperty);
			set => SetValue(FontFamilyProperty, value);
		}
		public double FontSize
		{
			get => GetValue(FontSizeProperty);
			set => SetValue(FontSizeProperty, value);
		}
		public FontStyle FontStyle
		{
			get => GetValue(FontStyleProperty);
			set => SetValue(FontStyleProperty, value);
		}
		public FontWeight FontWeight
		{
			get => GetValue(FontWeightProperty);
			set => SetValue(FontWeightProperty, value);
		}
		public IBrush Foreground
		{
			get => GetValue(ForegroundProperty);
			set => SetValue(ForegroundProperty, value);
		}
		public double LineHeight
		{
			get => GetValue(LineHeightProperty);
			set => SetValue(LineHeightProperty, value);
		}
		public int MaxLines
		{
			get => GetValue(MaxLinesProperty);
			set => SetValue(MaxLinesProperty, value);
		}
		public TextWrapping TextWrapping
		{
			get => GetValue(TextWrappingProperty);
			set => SetValue(TextWrappingProperty, value);
		}
		public TextTrimming TextTrimming
		{
			get => GetValue(TextTrimmingProperty);
			set => SetValue(TextTrimmingProperty, value);
		}
		public TextAlignment TextAlignment
		{
			get => GetValue(TextAlignmentProperty);
			set => SetValue(TextAlignmentProperty, value);
		}
		public TextDecorationCollection TextDecorations
		{
			get => GetValue(TextDecorationsProperty);
			set => SetValue(TextDecorationsProperty, value);
		}
		[Content]
		public Runs Children { get; } = new Runs();
		static RunLabel()
		{
			ClipToBoundsProperty.OverrideDefaultValue<RunLabel>(true);
			AffectsRender<RunLabel>(BackgroundProperty, ForegroundProperty,
				TextAlignmentProperty, TextDecorationsProperty);
			AffectsMeasure<RunLabel>(FontSizeProperty, FontWeightProperty,
				FontStyleProperty, TextWrappingProperty, FontFamilyProperty,
				TextTrimmingProperty,  PaddingProperty, LineHeightProperty, MaxLinesProperty);
			Observable.Merge<AvaloniaPropertyChangedEventArgs>(ForegroundProperty.Changed,
				TextAlignmentProperty.Changed, TextWrappingProperty.Changed,
				TextTrimmingProperty.Changed, FontSizeProperty.Changed,
				FontStyleProperty.Changed, FontWeightProperty.Changed,
				FontFamilyProperty.Changed, TextDecorationsProperty.Changed,
				PaddingProperty.Changed, MaxLinesProperty.Changed, LineHeightProperty.Changed
			).AddClassHandler<RunLabel>((x, _) => x.InvalidateTextLayout());
		}
		public RunLabel()
		{
			_text = string.Empty;
			Children.CollectionChanged += ChildrenChanged;
		}
		protected virtual void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			IEnumerable<string> controls;
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					controls = e.NewItems.OfType<Run>().Select(x => x.Content);
					_contents.InsertRange(e.NewStartingIndex, controls);
					break;
				case NotifyCollectionChangedAction.Move:
					_contents.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
					break;
				case NotifyCollectionChangedAction.Remove:
					controls = e.OldItems.OfType<Run>().Select(x => x.Content);
					_contents.RemoveAll(controls);
					break;
				case NotifyCollectionChangedAction.Replace:
					for (var i = 0; i < e.OldItems.Count; ++i)
					{
						var index = i + e.OldStartingIndex;
						var child = (Run)e.NewItems[i];
						_contents[index] = child.Content;
					}
					break;
			}
			_text = string.Join("", _contents);
			InvalidateMeasureOnChildrenChanged();
		}
		private protected virtual void InvalidateMeasureOnChildrenChanged()
		{
			InvalidateMeasure();
		}
		public override void Render(DrawingContext context)
		{
			IBrush background = Background;
			if (background != null)
				context.FillRectangle(background, new Rect(base.Bounds.Size));
			if (TextLayout == null)
				return;
			TextAlignment textAlignment = TextAlignment;
			double width = Bounds.Size.Width;
			double offsetX = textAlignment switch
			{
				TextAlignment.Center => (width - TextLayout.Size.Width) / 2.0,
				TextAlignment.Right => width - TextLayout.Size.Width,
				_ => 0.0,
			};
			Thickness padding = Padding;
			double top = padding.Top;
			Size textSize = TextLayout.Size;
			if (Bounds.Height < textSize.Height)
			{
				switch (VerticalAlignment)
				{
					case VerticalAlignment.Center:
						top += (Bounds.Height - textSize.Height) / 2.0;
						break;
					case VerticalAlignment.Bottom:
						top += Bounds.Height - textSize.Height;
						break;
				}
			}
			using (context.PushPostTransform(Matrix.CreateTranslation(padding.Left + offsetX, top)))
				TextLayout.Draw(context);
		}
		protected virtual TextLayout CreateTextLayout(Size constraint, string text)
		{
			if (constraint == Size.Empty)
				return null;
			return new TextLayout(text ?? string.Empty, new Typeface(FontFamily, FontStyle, FontWeight), FontSize, Foreground, TextAlignment, TextWrapping, TextTrimming, TextDecorations, constraint.Width, constraint.Height, maxLines: MaxLines, lineHeight: LineHeight);
		}
		protected void InvalidateTextLayout()
		{
			_textLayout = null;
		}
		protected override Size MeasureOverride(Size availableSize)
		{
			if (string.IsNullOrEmpty(_text))
				return default;
			Thickness padding = Padding;
			availableSize = availableSize.Deflate(padding);
			if (_constraint != availableSize)
			{
				_constraint = availableSize;
				InvalidateTextLayout();
			}
			return (TextLayout?.Size ?? Size.Empty).Inflate(padding);
		}
		protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
		{
			base.OnAttachedToLogicalTree(e);
			InvalidateTextLayout();
			InvalidateMeasure();
		}
		private static bool IsValidMaxLines(int maxLines)
		{
			return maxLines >= 0;
		}
		private static bool IsValidLineHeight(double lineHeight)
		{
			if (!double.IsNaN(lineHeight))
				return lineHeight > 0.0;
			return true;
		}
	}
}
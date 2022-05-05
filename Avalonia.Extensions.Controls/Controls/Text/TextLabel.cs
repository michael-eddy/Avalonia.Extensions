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
	public class TextLabel : Control, IRuns, IStyling
	{
		/// <summary>
		/// Defines the <see cref="Background"/> property.
		/// </summary>
		public static readonly StyledProperty<IBrush> BackgroundProperty =
			Border.BackgroundProperty.AddOwner<TextLabel>();
		/// <summary>
		/// Defines the <see cref="Padding"/> property.
		/// </summary>
		public static readonly StyledProperty<Thickness> PaddingProperty =
			Decorator.PaddingProperty.AddOwner<TextLabel>();
		/// <summary>
		/// Defines the <see cref="FontFamily"/> property.
		/// </summary>
		public static readonly AttachedProperty<FontFamily> FontFamilyProperty =
			AvaloniaProperty.RegisterAttached<TextLabel, Control, FontFamily>(nameof(FontFamily), FontFamily.Default, true);
		/// <summary>
		/// Defines the <see cref="FontSize"/> property.
		/// </summary>
		public static readonly AttachedProperty<double> FontSizeProperty =
			AvaloniaProperty.RegisterAttached<TextLabel, Control, double>(nameof(FontSize), 12, true);
		/// <summary>
		/// Defines the <see cref="FontStyle"/> property.
		/// </summary>
		public static readonly AttachedProperty<FontStyle> FontStyleProperty =
			AvaloniaProperty.RegisterAttached<TextLabel, Control, FontStyle>(nameof(FontStyle), inherits: true);
		/// <summary>
		/// Defines the <see cref="FontWeight"/> property.
		/// </summary>
		public static readonly AttachedProperty<FontWeight> FontWeightProperty =
			AvaloniaProperty.RegisterAttached<TextLabel, Control, FontWeight>(nameof(FontWeight), FontWeight.Normal, true);
		/// <summary>
		/// Defines the <see cref="Foreground"/> property.
		/// </summary>
		public static readonly AttachedProperty<IBrush> ForegroundProperty =
			AvaloniaProperty.RegisterAttached<TextLabel, Control, IBrush>(nameof(Foreground), Brushes.Black, true);
		/// <summary>
		/// Defines the <see cref="LineHeight"/> property.
		/// </summary>
		public static readonly StyledProperty<double> LineHeightProperty =
			AvaloniaProperty.Register<TextLabel, double>(nameof(LineHeight), double.NaN, validate: IsValidLineHeight);
		/// <summary>
		/// Defines the <see cref="MaxLines"/> property.
		/// </summary>
		public static readonly StyledProperty<int> MaxLinesProperty =
			AvaloniaProperty.Register<TextLabel, int>(nameof(MaxLines), validate: IsValidMaxLines);
		/// <summary>
		/// Defines the <see cref="TextAlignment"/> property.
		/// </summary>
		public static readonly StyledProperty<TextAlignment> TextAlignmentProperty = AvaloniaProperty.Register<TextLabel, TextAlignment>(nameof(TextAlignment));
		/// <summary>
		/// Defines the <see cref="TextWrapping"/> property.
		/// </summary>
		public static readonly StyledProperty<TextWrapping> TextWrappingProperty = AvaloniaProperty.Register<TextLabel, TextWrapping>(nameof(TextWrapping));
		/// <summary>
		/// Defines the <see cref="TextTrimming"/> property.
		/// </summary>
		public static readonly StyledProperty<TextTrimming> TextTrimmingProperty = AvaloniaProperty.Register<TextLabel, TextTrimming>(nameof(TextTrimming));
		/// <summary>
		/// Defines the <see cref="TextDecorations"/> property.
		/// </summary>
		public static readonly StyledProperty<TextDecorationCollection> TextDecorationsProperty = AvaloniaProperty.Register<TextLabel, TextDecorationCollection>(nameof(TextDecorations));
		private Runs _items;
		private string _text;
		private Size _constraint;
		private TextLayout _textLayout;
		private readonly AvaloniaList<string> _contents = new AvaloniaList<string>();
		Type IStyleable.StyleKey => typeof(TextBlock);
		internal TextLayout TextLayout => _textLayout ??= CreateTextLayout(_constraint, _text);
		/// <summary>
		/// Gets or sets the padding to place around the <see cref="Children"/>.
		/// </summary>
		public Thickness Padding
		{
			get => GetValue(PaddingProperty);
			set => SetValue(PaddingProperty, value);
		}
		/// <summary>
		/// Gets or sets a brush used to paint the control's background.
		/// </summary>
		public IBrush Background
		{
			get => GetValue(BackgroundProperty);
			set => SetValue(BackgroundProperty, value);
		}
		/// <summary>
		/// Gets or sets the font family used to draw the control's text.
		/// </summary>
		public FontFamily FontFamily
		{
			get => GetValue(FontFamilyProperty);
			set => SetValue(FontFamilyProperty, value);
		}
		/// <summary>
		/// Gets or sets the size of the control's text in points.
		/// </summary>
		public double FontSize
		{
			get => GetValue(FontSizeProperty);
			set => SetValue(FontSizeProperty, value);
		}
		/// <summary>
		/// Gets or sets the font style used to draw the control's text.
		/// </summary>
		public FontStyle FontStyle
		{
			get => GetValue(FontStyleProperty);
			set => SetValue(FontStyleProperty, value);
		}
		/// <summary>
		/// Gets or sets the font weight used to draw the control's text.
		/// </summary>
		public FontWeight FontWeight
		{
			get => GetValue(FontWeightProperty);
			set => SetValue(FontWeightProperty, value);
		}
		/// <summary>
		/// Gets or sets a brush used to paint the text.
		/// </summary>
		public IBrush Foreground
		{
			get => GetValue(ForegroundProperty);
			set => SetValue(ForegroundProperty, value);
		}
		/// <summary>
		/// Gets or sets the height of each line of content.
		/// </summary>
		public double LineHeight
		{
			get => GetValue(LineHeightProperty);
			set => SetValue(LineHeightProperty, value);
		}
		/// <summary>
		/// Gets or sets the maximum number of text lines.
		/// </summary>
		public int MaxLines
		{
			get => GetValue(MaxLinesProperty);
			set => SetValue(MaxLinesProperty, value);
		}
		/// <summary>
		/// Gets or sets the control's text wrapping mode.
		/// </summary>
		public TextWrapping TextWrapping
		{
			get => GetValue(TextWrappingProperty);
			set => SetValue(TextWrappingProperty, value);
		}
		/// <summary>
		/// Gets or sets the control's text trimming mode.
		/// </summary>
		public TextTrimming TextTrimming
		{
			get => GetValue(TextTrimmingProperty);
			set => SetValue(TextTrimmingProperty, value);
		}
		/// <summary>
		/// Gets or sets the text alignment.
		/// </summary>
		public TextAlignment TextAlignment
		{
			get => GetValue(TextAlignmentProperty);
			set => SetValue(TextAlignmentProperty, value);
		}
		/// <summary>
		/// Gets or sets the text decorations.
		/// </summary>
		public TextDecorationCollection TextDecorations
		{
			get => GetValue(TextDecorationsProperty);
			set => SetValue(TextDecorationsProperty, value);
		}
		/// <summary>
		/// Defines the <see cref="Children"/> property.
		/// </summary>
		public static readonly DirectProperty<TextLabel, Runs> TextProperty =
			AvaloniaProperty.RegisterDirect<TextLabel, Runs>(nameof(Children), o => o.Children, (o, v) => o.Children = v);
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		[Content]
		public Runs Children
		{
			get => _items;
			set => SetAndRaise(TextProperty, ref _items, value);
		}
		static TextLabel()
		{
			ClipToBoundsProperty.OverrideDefaultValue<TextLabel>(true);
			AffectsRender<TextLabel>(BackgroundProperty, ForegroundProperty, TextAlignmentProperty, TextDecorationsProperty);
			AffectsMeasure<TextLabel>(FontSizeProperty, FontWeightProperty, FontStyleProperty, TextWrappingProperty, FontFamilyProperty,
				TextTrimmingProperty, PaddingProperty, LineHeightProperty, MaxLinesProperty);
			Observable.Merge<AvaloniaPropertyChangedEventArgs>(ForegroundProperty.Changed, TextAlignmentProperty.Changed, TextWrappingProperty.Changed,
				TextTrimmingProperty.Changed, FontSizeProperty.Changed, FontStyleProperty.Changed, FontWeightProperty.Changed, FontFamilyProperty.Changed,
				TextDecorationsProperty.Changed, PaddingProperty.Changed, MaxLinesProperty.Changed, LineHeightProperty.Changed).AddClassHandler<TextLabel>((x, _) => x.InvalidateTextLayout());
		}
		public TextLabel()
		{
			_items = new Runs();
			_text = string.Empty;
			Children.CollectionChanged += ChildrenChanged;
		}
		protected virtual void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			List<string> controls = new List<string>();
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						var objs = e.NewItems.OfType<AvaloniaObject>();
						foreach (var obj in objs)
						{
							if (obj is LineBreak lb)
								controls.Add(lb.Content);
							else if (obj is Run r)
								controls.Add(r.Content);
						}
						_contents.InsertRange(e.NewStartingIndex, controls);
						break;
					}
				case NotifyCollectionChangedAction.Move:
					_contents.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
					break;
				case NotifyCollectionChangedAction.Remove:
					{
						var objs = e.NewItems.OfType<AvaloniaObject>();
						foreach (var obj in objs)
						{
							if (obj is LineBreak lb)
								controls.Add(lb.Content);
							else if (obj is Run r)
								controls.Add(r.Content);
						}
						_contents.RemoveAll(controls);
						break;
					}
				case NotifyCollectionChangedAction.Replace:
					for (var i = 0; i < e.OldItems.Count; ++i)
					{
						var index = i + e.OldStartingIndex;
						if (e.NewItems[i] is Run child)
							_contents[index] = child.Content;
						else if (e.NewItems[i] is LineBreak)
							_contents[index] = string.Empty;
					}
					break;
			}
			_text = string.Join(string.Empty, _contents);
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
				context.FillRectangle(background, new Rect(Bounds.Size));
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
		internal virtual TextLayout CreateTextLayout(Size constraint, string text)
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
		private static bool IsValidMaxLines(int maxLines) => maxLines >= 0;
		private static bool IsValidLineHeight(double lineHeight)
		{
			if (!double.IsNaN(lineHeight))
				return lineHeight > 0.0;
			return true;
		}
	}
}
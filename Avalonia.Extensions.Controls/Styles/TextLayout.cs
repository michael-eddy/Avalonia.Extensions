using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Extensions.Controls;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Utilities;
using PCLUntils.Assemblly;

namespace Avalonia.Extensions.Styles
{
	internal sealed class TextLayout
	{
		private readonly struct FormattedTextSource : ITextSource
		{
			private readonly ReadOnlySlice<char> _text;
			private readonly TextRunProperties _defaultProperties;
			private readonly IReadOnlyList<ValueSpan<TextRunProperties>> _textModifier;
			public FormattedTextSource(ReadOnlySlice<char> text, TextRunProperties defaultProperties, IReadOnlyList<ValueSpan<TextRunProperties>> textModifier)
			{
				_text = text;
				_defaultProperties = defaultProperties;
				_textModifier = textModifier;
			}
			public TextRun GetTextRun(int textSourceIndex)
			{
				if (textSourceIndex > _text.End)
					return new TextEndOfLine();
				ReadOnlySlice<char> runText = _text.Skip(textSourceIndex);
				if (runText.IsEmpty)
					return new TextEndOfLine();
				ValueSpan<TextRunProperties> textStyleRun = CreateTextStyleRun(runText, _defaultProperties, _textModifier);
				return new TextCharacters(runText.Take(textStyleRun.Length), textStyleRun.Value);
			}
			private static ValueSpan<TextRunProperties> CreateTextStyleRun(ReadOnlySlice<char> text, TextRunProperties defaultProperties, IReadOnlyList<ValueSpan<TextRunProperties>> textModifier)
			{
				if (textModifier == null || textModifier.Count == 0)
					return new ValueSpan<TextRunProperties>(text.Start, text.Length, defaultProperties);
				TextRunProperties currentProperties = defaultProperties;
				bool hasOverride = false;
				int i = 0, length = 0;
				for (; i < textModifier.Count; i++)
				{
					ValueSpan<TextRunProperties> propertiesOverride = textModifier[i];
					TextRange textRange = new TextRange(propertiesOverride.Start, propertiesOverride.Length);
					if (textRange.End >= text.Start)
					{
						if (textRange.Start > text.End)
						{
							length = text.Length;
							break;
						}
						if (textRange.Start > text.Start && propertiesOverride.Value != currentProperties)
						{
							length = Math.Min(Math.Abs(textRange.Start - text.Start), text.Length);
							break;
						}
						length += Math.Min(text.Length - length, textRange.Length);
						if (!hasOverride)
						{
							hasOverride = true;
							currentProperties = propertiesOverride.Value;
						}
					}
				}
				if (length < text.Length && i == textModifier.Count && currentProperties == defaultProperties)
					length = text.Length;
				if (length != text.Length)
					text = text.Take(length);
				return new ValueSpan<TextRunProperties>(text.Start, length, currentProperties);
			}
		}
		private static readonly char[] s_empty = new char[1] { '\u200b' };
		private readonly ReadOnlySlice<char> _text;
		private readonly TextParagraphProperties _paragraphProperties;
		private readonly IReadOnlyList<ValueSpan<TextRunProperties>> _textStyleOverrides;
		private readonly TextTrimming _textTrimming;
		public int MaxLines { get; }
		public double MaxWidth { get; }
		public double MaxHeight { get; }
		public double LineHeight { get; }
		public Size Size { get; private set; }
		public IReadOnlyList<TextLine> TextLines { get; private set; }
		internal TextLayout(string text, Typeface typeface, double fontSize, IBrush foreground, TextAlignment textAlignment = TextAlignment.Left, TextWrapping textWrapping = TextWrapping.NoWrap, TextTrimming textTrimming = TextTrimming.None, TextDecorationCollection textDecorations = null, double maxWidth = double.PositiveInfinity, double maxHeight = double.PositiveInfinity, double lineHeight = double.NaN, int maxLines = 0, IReadOnlyList<ValueSpan<TextRunProperties>> textStyleOverrides = null)
		{
			_text = (string.IsNullOrEmpty(text) ? default : new ReadOnlySlice<char>(text.AsMemory()));
			_paragraphProperties = CreateTextParagraphProperties(typeface, fontSize, foreground, textAlignment, textWrapping, textDecorations, lineHeight);
			_textTrimming = textTrimming;
			_textStyleOverrides = textStyleOverrides;
			LineHeight = lineHeight;
			MaxWidth = maxWidth;
			MaxHeight = maxHeight;
			MaxLines = maxLines;
			UpdateLayout();
		}
		public void Draw(DrawingContext context)
		{
			if (!TextLines.Any())
				return;
			double currentY = 0.0;
			foreach (TextLine textLine in TextLines)
			{
				double offsetX = ControlUtils.GetParagraphOffsetX(textLine.LineMetrics.Size.Width, Size.Width, _paragraphProperties.TextAlignment);
				using (context.PushPostTransform(Matrix.CreateTranslation(offsetX, currentY)))
					textLine.Draw(context);
				currentY += textLine.LineMetrics.Size.Height;
			}
		}
		private static TextParagraphProperties CreateTextParagraphProperties(Typeface typeface, double fontSize, IBrush foreground, TextAlignment textAlignment, TextWrapping textWrapping, TextDecorationCollection textDecorations, double lineHeight)
		{
			return new GenericTextParagraphProperties(new GenericTextRunProperties(typeface, fontSize, textDecorations, foreground), textAlignment, textWrapping, lineHeight);
		}
		private static void UpdateBounds(TextLine textLine, ref double width, ref double height)
		{
			if (width < textLine.LineMetrics.Size.Width)
				width = textLine.LineMetrics.Size.Width;
			height += textLine.LineMetrics.Size.Height;
		}
		private TextLine CreateEmptyTextLine(int startingIndex)
		{
			TextRunProperties properties = _paragraphProperties.DefaultTextRunProperties;
			GlyphRun glyphRun = TextShaper.Current.ShapeText(new ReadOnlySlice<char>(s_empty, startingIndex, 1), properties.Typeface, properties.FontRenderingEmSize, properties.CultureInfo);
			List<ShapedTextCharacters> textRuns = new List<ShapedTextCharacters>
			{
				new ShapedTextCharacters(glyphRun, _paragraphProperties.DefaultTextRunProperties)
			};
			object[] parameters = new object[] { textRuns, TextLineMetrics.Create(textRuns, new TextRange(startingIndex, 1), MaxWidth, _paragraphProperties) };
			return "Avalonia.Media.TextFormatting.TextLineImpl".CreateInstance<TextLine>(parameters);
		}
		private void UpdateLayout()
		{
			if (_text.IsEmpty || MathUtilities.IsZero(MaxWidth) || MathUtilities.IsZero(MaxHeight))
			{
				TextLine textLine = CreateEmptyTextLine(0);
				TextLines = new List<TextLine> { textLine };
				Size = new Size(0.0, textLine.LineMetrics.Size.Height);
				return;
			}
			List<TextLine> textLines = new List<TextLine>();
			double width = 0.0, height = 0.0;
			int currentPosition = 0;
			FormattedTextSource textSource = new FormattedTextSource(_text, _paragraphProperties.DefaultTextRunProperties, _textStyleOverrides);
			TextLine previousLine = null;
			while (currentPosition < _text.Length)
			{
				TextLine textLine2 = TextFormatter.Current.FormatLine(textSource, currentPosition, MaxWidth, _paragraphProperties, previousLine?.TextLineBreak);
				currentPosition += textLine2.TextRange.Length;
				if (textLines.Count > 0 && (textLines.Count == MaxLines || (!double.IsPositiveInfinity(MaxHeight) && height + textLine2.LineMetrics.Size.Height > MaxHeight)))
				{
					if (previousLine?.TextLineBreak != null && _textTrimming != 0)
						textLines[^1] = previousLine.Collapse(GetCollapsingProperties(MaxWidth));
					break;
				}
				if (textLine2.LineMetrics.HasOverflowed && _textTrimming != 0)
					textLine2 = textLine2.Collapse(GetCollapsingProperties(MaxWidth));
				textLines.Add(textLine2);
				UpdateBounds(textLine2, ref width, ref height);
				previousLine = textLine2;
				if (currentPosition == _text.Length && textLine2.TextLineBreak != null)
				{
					TextLine emptyTextLine = CreateEmptyTextLine(currentPosition);
					textLines.Add(emptyTextLine);
				}
			}
			Size = new Size(width, height);
			TextLines = textLines;
		}
		private TextCollapsingProperties GetCollapsingProperties(double width)
		{
			return _textTrimming switch
			{
				TextTrimming.CharacterEllipsis => new TextTrailingCharacterEllipsis(width, _paragraphProperties.DefaultTextRunProperties),
				TextTrimming.WordEllipsis => new TextTrailingWordEllipsis(width, _paragraphProperties.DefaultTextRunProperties),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}
	}
}
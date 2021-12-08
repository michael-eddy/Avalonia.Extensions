using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Extensions.Styles;
using Avalonia.Media;
using Avalonia.Styling;
using System;

namespace Avalonia.Extensions.Controls
{
    /// <summary>
    /// https://github.com/AvaloniaUI/Avalonia/tree/feature/icons/src
    /// </summary>
    public class SymbolIcon : TemplatedControl, IStyling
    {
        public static readonly StyledProperty<string> GlyphProperty =
             AvaloniaProperty.Register<SymbolIcon, string>(nameof(Glyph));
        public string Glyph
        {
            get => GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }
        Type IStyleable.StyleKey => typeof(TextBlock);
        public SymbolIcon()
        {
            this.InitStyle();
            SetValue(FontFamilyProperty, "Segoe MDL2 Assets");
        }
        public class FontIconSource : AvaloniaObject
        {
            public static readonly StyledProperty<string> GlyphProperty =
                SymbolIcon.GlyphProperty.AddOwner<FontIconSource>();
            /// <summary>
            /// Defines the <see cref="FontFamily"/> property.
            /// </summary>
            internal static readonly StyledProperty<FontFamily> FontFamilyProperty =
                TextBlock.FontFamilyProperty.AddOwner<FontIconSource>();
            /// <summary>
            /// Defines the <see cref="FontSize"/> property.
            /// </summary>
            public static readonly StyledProperty<double> FontSizeProperty =
                TextBlock.FontSizeProperty.AddOwner<FontIconSource>();
            /// <summary>
            /// Defines the <see cref="FontStyle"/> property.
            /// </summary>
            public static readonly StyledProperty<FontStyle> FontStyleProperty =
                TextBlock.FontStyleProperty.AddOwner<FontIconSource>();
            /// <summary>
            /// Defines the <see cref="FontWeight"/> property.
            /// </summary>
            public static readonly StyledProperty<FontWeight> FontWeightProperty =
                TextBlock.FontWeightProperty.AddOwner<FontIconSource>();
            public string Glyph
            {
                get => GetValue(GlyphProperty);
                set => SetValue(GlyphProperty, value);
            }
            /// <summary>
            /// Gets or sets the font family used to draw the control's text.
            /// </summary>
            internal FontFamily FontFamily
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
            public static StyledProperty<IBrush?> ForegroundProperty =
                TemplatedControl.ForegroundProperty.AddOwner<FontIconSource>();
            public IBrush? Foreground
            {
                get => GetValue(ForegroundProperty);
                set => SetValue(ForegroundProperty, value);
            }
            public IDataTemplate IconElementTemplate { get; } = new FuncDataTemplate<FontIconSource>((source, _) => new SymbolIcon
            {
                [!GlyphProperty] = source[!GlyphProperty],
                [!FontSizeProperty] = source[!FontSizeProperty],
                [!FontStyleProperty] = source[!FontStyleProperty],
                [!FontFamilyProperty] = source[!FontFamilyProperty],
                [!ForegroundProperty] = source[!ForegroundProperty],
                [!FontWeightProperty] = source[!FontWeightProperty]
            });
        }
    }
}
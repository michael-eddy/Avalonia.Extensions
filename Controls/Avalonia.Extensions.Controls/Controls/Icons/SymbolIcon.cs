﻿using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace Avalonia.Extensions.Controls
{
    public class SymbolIcon : IconElement
    {
        protected static readonly StyledProperty<string> GlyphProperty =
             AvaloniaProperty.Register<SymbolIcon, string>(nameof(Glyph));
        public string Glyph
        {
            get => GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }
        static SymbolIcon()
        {
            AffectsRender<SymbolIcon>(GlyphProperty);
        }
        public SymbolIcon()
        {
            SetValue(FontFamilyProperty, "Segoe MDL2 Assets");
        }
        public class FontIconSource : IconSource
        {
            protected static readonly StyledProperty<string> GlyphProperty =
                SymbolIcon.GlyphProperty.AddOwner<FontIconSource>();
            /// <summary>
            /// Defines the <see cref="FontFamily"/> property.
            /// </summary>
            protected static readonly StyledProperty<FontFamily> FontFamilyProperty =
                TextBlock.FontFamilyProperty.AddOwner<FontIconSource>();
            /// <summary>
            /// Defines the <see cref="FontSize"/> property.
            /// </summary>
            protected static readonly StyledProperty<double> FontSizeProperty =
                TextBlock.FontSizeProperty.AddOwner<FontIconSource>();
            /// <summary>
            /// Defines the <see cref="FontStyle"/> property.
            /// </summary>
            public static readonly StyledProperty<FontStyle> FontStyleProperty =
                TextBlock.FontStyleProperty.AddOwner<FontIconSource>();
            /// <summary>
            /// Defines the <see cref="FontWeight"/> property.
            /// </summary>
            protected static readonly StyledProperty<FontWeight> FontWeightProperty =
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
            public override IDataTemplate IconElementTemplate { get; } = new FuncDataTemplate<FontIconSource>((source, _) => new SymbolIcon
            {
                [!ForegroundProperty] = source[!ForegroundProperty],
                [!GlyphProperty] = source[!GlyphProperty],
                [!FontFamilyProperty] = source[!FontFamilyProperty],
                [!FontSizeProperty] = source[!FontSizeProperty],
                [!FontStyleProperty] = source[!FontStyleProperty],
                [!FontWeightProperty] = source[!FontWeightProperty]
            });
        }
    }
}
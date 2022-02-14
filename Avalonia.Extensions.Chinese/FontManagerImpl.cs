using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Skia;
using SkiaSharp;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Avalonia.Extensions.Controls
{
    public sealed class FontManagerImpl : IFontManagerImpl
    {
        private readonly string _defaultFamilyName;
        private readonly Typeface[] _customTypefaces;
        private readonly Typeface _defaultTypeface = new Typeface("avares://Avalonia.Extensions.Chinese/Assets/Fonts#WenQuanYi Micro Hei");
        /// <summary>
        /// ISO 639, 15924, and 3166-1 c
        /// </summary>
        private readonly string[] _bcp47 = { CultureInfo.CurrentCulture.ThreeLetterISOLanguageName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName };
        public FontManagerImpl()
        {
            _customTypefaces = new[] { _defaultTypeface };
            _defaultFamilyName = _defaultTypeface.FontFamily.FamilyNames.PrimaryFamilyName;
        }
        public string GetDefaultFontFamilyName() => _defaultFamilyName;
        public IEnumerable<string> GetInstalledFontFamilyNames(bool checkForUpdates = false) => _customTypefaces.Select(x => x.FontFamily.Name);
        public bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight, FontFamily fontFamily,
            CultureInfo culture, out Typeface typeface)
        {
            foreach (var customTypeface in _customTypefaces)
            {
                if (customTypeface.GlyphTypeface.GetGlyph((uint)codepoint) == 0)
                    continue;
                typeface = new Typeface(customTypeface.FontFamily.Name, fontStyle, fontWeight);
                return true;
            }
            string fontFamilyName = fontFamily?.FamilyNames.HasFallbacks == true ? fontFamily.Name : string.Empty;
            var fallback = SKFontManager.Default.MatchCharacter(fontFamilyName, (SKFontStyleWeight)fontWeight, SKFontStyleWidth.Normal, (SKFontStyleSlant)fontStyle, _bcp47, codepoint);
            typeface = new Typeface(fallback?.FamilyName ?? _defaultFamilyName, fontStyle, fontWeight);
            return true;
        }
        public IGlyphTypefaceImpl CreateGlyphTypeface(Typeface typeface)
        {
            string familyName;
            switch (typeface.FontFamily.Name)
            {
                case "WenQuanYi Micro Hei":
                case FontFamily.DefaultFontFamilyName:
                    familyName = _defaultTypeface.FontFamily.Name;
                    break;
                default:
                    familyName = typeface.FontFamily.Name;
                    break;
            }
            var skTypeface = SKTypeface.FromFamilyName(familyName, (SKFontStyleWeight)typeface.Weight, SKFontStyleWidth.Normal, (SKFontStyleSlant)typeface.Style);
            var isFakeBold = (int)typeface.Weight >= 600 && !skTypeface.IsBold;
            var isFakeItalic = typeface.Style == FontStyle.Italic && !skTypeface.IsItalic;
            return new GlyphTypefaceImpl(skTypeface, isFakeBold, isFakeItalic);
        }
    }
}
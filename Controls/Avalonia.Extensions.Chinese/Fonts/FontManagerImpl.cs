using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Avalonia.Extensions.Controls
{
    internal sealed class FontManagerImpl : IFontManagerImpl
    {
        private readonly string _defaultFamilyName;
        private readonly Typeface[] _customTypefaces;
        internal readonly Typeface _defaultTypeface = new Typeface(FONT_LOCATION);
        internal const string FONT_LOCATION = "resm:Avalonia.Extensions.Chinese.Assets.Fonts.wqyzenhei#文泉驿正黑";
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
        public bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight, FontFamily fontFamily, CultureInfo culture, out Typeface typeface)
        {
            try
            {
                foreach (var customTypeface in _customTypefaces)
                {
                    if (customTypeface.GlyphTypeface.GetGlyph((uint)codepoint) == 0)
                        continue;
                    typeface = new Typeface(customTypeface.FontFamily, fontStyle, fontWeight);
                    return true;
                }
                var fallback = SKFontManager.Default.MatchCharacter(fontFamily?.Name, (SKFontStyleWeight)fontWeight, SKFontStyleWidth.Normal, (SKFontStyleSlant)fontStyle, _bcp47, codepoint);
                typeface = new Typeface(fallback?.FamilyName ?? _defaultFamilyName, fontStyle, fontWeight);
                return true;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Visual)?.Log(this, "TryMatchCharacter Error:" + ex.Message);
                typeface = default;
                return false;
            }
        }
        public IGlyphTypefaceImpl CreateGlyphTypeface(Typeface typeface)
        {
            try
            {
                var name = _defaultTypeface.FontFamily?.Name;
                var skTypeface = (typeface.FontFamily?.Name ?? FontFamily.DefaultFontFamilyName) switch
                {
                    "WenQuanYi Micro Hei" or FontFamily.DefaultFontFamilyName => SKTypeface.FromFamilyName(_defaultTypeface.FontFamily?.Name),
                    _ => SKTypeface.FromFamilyName(typeface.FontFamily.Name, (SKFontStyleWeight)typeface.Weight, SKFontStyleWidth.Normal, (SKFontStyleSlant)typeface.Style)
                };
                skTypeface ??= SKTypeface.Default;
                Logger.TryGet(LogEventLevel.Debug, LogArea.Visual)?.Log(this, "CreateGlyphTypeface Info:" + skTypeface.FamilyName);
                var isFakeBold = (int)typeface.Weight >= 600 && !skTypeface.IsBold;
                var isFakeItalic = typeface.Style == FontStyle.Italic && !skTypeface.IsItalic;
                return new GlyphTypefaceImpl(skTypeface, isFakeBold, isFakeItalic);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Visual)?.Log(this, "CreateGlyphTypeface Error:" + ex.Message);
                return default;
            }
        }
    }
}
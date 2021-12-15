﻿using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Skia;
using SkiaSharp;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Avalonia.Extensions
{
    public sealed class FontManagerImpl : IFontManagerImpl
    {
        private readonly Typeface[] _customTypefaces;
        private readonly string _defaultFamilyName;
        private readonly Typeface _defaultTypeface = new Typeface("resm:Avalonia.Extensions.Controls.Assets.Fonts#Droid Sans Fallback");
        public FontManagerImpl()
        {
            _customTypefaces = new[] { _defaultTypeface };
            _defaultFamilyName = _defaultTypeface.FontFamily.FamilyNames.PrimaryFamilyName;
        }
        public string GetDefaultFontFamilyName()
        {
            return _defaultFamilyName;
        }
        public IEnumerable<string> GetInstalledFontFamilyNames(bool checkForUpdates = false)
        {
            return _customTypefaces.Select(x => x.FontFamily.Name);
        }
        private readonly string[] _bcp47 = { CultureInfo.CurrentCulture.ThreeLetterISOLanguageName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName };
        public bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight, FontFamily fontFamily, CultureInfo culture, out Typeface typeface)
        {
            foreach (var customTypeface in _customTypefaces)
            {
                if (customTypeface.GlyphTypeface.GetGlyph((uint)codepoint) == 0)
                    continue;
                typeface = new Typeface(customTypeface.FontFamily.Name, fontStyle, fontWeight);
                return true;
            }
            var fallback = SKFontManager.Default.MatchCharacter(fontFamily?.Name, (SKFontStyleWeight)fontWeight,
                SKFontStyleWidth.Normal, (SKFontStyleSlant)fontStyle, _bcp47, codepoint);
            typeface = new Typeface(fallback?.FamilyName ?? _defaultFamilyName, fontStyle, fontWeight);
            return true;
        }
        public IGlyphTypefaceImpl CreateGlyphTypeface(Typeface typeface)
        {
            SKTypeface skTypeface = typeface.FontFamily.Name switch
            {
                FontFamily.DefaultFontFamilyName or "Droid Sans Fallback" => SKTypeface.FromFamilyName(_defaultTypeface.FontFamily.Name),
                _ => SKTypeface.FromFamilyName(typeface.FontFamily.Name, (SKFontStyleWeight)typeface.Weight, SKFontStyleWidth.Normal, (SKFontStyleSlant)typeface.Style)
            };
            return new GlyphTypefaceImpl(skTypeface);
        }
    }
}
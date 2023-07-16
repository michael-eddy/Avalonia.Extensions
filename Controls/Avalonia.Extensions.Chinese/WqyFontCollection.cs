using Avalonia.Media.Fonts;
using System;

namespace Avalonia.Extensions
{
    public sealed class WqyFontCollection : EmbeddedFontCollection
    {
        public WqyFontCollection() : base(
            new Uri("fonts:wqy", UriKind.Absolute),
            new Uri("resm:Avalonia.Extensions.Chinese.Assets.Fonts.wqyzenhei#文泉驿正黑", UriKind.Absolute))
        {
        }
    }
}
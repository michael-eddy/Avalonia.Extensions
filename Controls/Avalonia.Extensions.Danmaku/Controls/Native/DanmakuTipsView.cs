using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using SkiaSharp;
using System;

namespace Avalonia.Extensions.Danmaku
{
    public class DanmakuTipsView : TextBlock
    {
        protected override Type StyleKeyOverride => typeof(TextBlock);
        internal float startCursor { get; private set; } = 0;
        public void SetData(DanmakuNativeView parent, DanmakuModel model)
        {
            startCursor = parent.Timeline;
            var textSize = model.Text.MeasureString(model.Size, SKTypeface.FromFamilyName(FontFamily.Name));
            int marginTop = 0, marginLeft = parent.PartWidth - Convert.ToInt32(textSize.Width / 2);
            switch (model.Location)
            {
                case DanmakuLocation.Bottom:
                    marginTop = parent.random.Next(parent.RangeHeight.Center, parent.RangeHeight.Bottom);
                    break;
                case DanmakuLocation.Top:
                    marginTop = parent.random.Next(0, parent.RangeHeight.Top);
                    break;
                case DanmakuLocation.Position:
                    break;
            }
            Margin = new Thickness(marginLeft, marginTop, 0, 0);
            Text = model.Text;
            FontSize = model.Size;
            Foreground = model.Color;
        }
    }
}
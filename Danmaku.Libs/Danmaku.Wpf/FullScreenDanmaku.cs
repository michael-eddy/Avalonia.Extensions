using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Danmaku.Wpf
{
    internal class FullScreenDanmaku : UserControl
    {
        public TextBlock Text { get; }
        public FullScreenDanmaku()
        {
            Text = new TextBlock();
            Text.Name = "Text";
            Text.FontSize = 40;
            Text.Foreground = Brushes.White;
            Text.FontWeight = FontWeights.Bold;
            DropShadowEffect effect = new DropShadowEffect();
            effect.ShadowDepth = 0;
            effect.BlurRadius = 10;
            effect.Color = Colors.LightGray;
            Text.Effect = effect;
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Content = Text;
        }
        public void ChangeHeight()
        {
            Text.FontSize = 35;
            Text.Measure(new Size(int.MaxValue, int.MaxValue));
        }
    }
}
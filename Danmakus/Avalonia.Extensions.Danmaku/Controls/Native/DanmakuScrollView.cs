using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Styles;
using Avalonia.Styling;
using SkiaSharp;
using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Danmaku
{
    public class DanmakuScrollView : TextBlock, IStyling
    {
        private Task animationRun;
        Type IStyleable.StyleKey => typeof(TextBlock);
        private readonly Animation.Animation animation;
        public DanmakuScrollView()
        {
            animation = new Animation.Animation();
        }
        public void SetData(DanmakuNativeView parent, DanmakuModel model)
        {
            if (model != null)
            {
                var marginRight = parent.Bounds.Width;
                var textSize = model.Text.MeasureString(model.Size, SKTypeface.FromFamilyName(FontFamily.Name));
                var marginTop = parent.random.Next(0, parent.ActualHeight) - textSize.Height.ToInt32();
                Text = model.Text;
                FontSize = model.Size;
                Foreground = model.Color;
                Margin = new Thickness(marginRight, marginTop, 0, 0);
                Init(marginRight, marginTop);
            }
        }
        private async void Init(double marginRight, double marginTop)
        {
            animationRun = null;
            animation.FillMode = FillMode.Forward;
            animation.Duration = TimeSpan.FromSeconds(10);
            var marginLeft = 0 - marginRight;
            KeyFrame startKf = new KeyFrame
            {
                Cue = new Cue(0.0),
                Setters = { new Setter(MarginProperty, new Thickness(marginRight, marginTop, 0, 0)) }
            };
            animation.Children.Add(startKf);
            KeyFrame endKf = new KeyFrame
            {
                Cue = new Cue(1.0),
                Setters = { new Setter(MarginProperty, new Thickness(marginLeft, marginTop, 0, 0)) }
            };
            animation.Children.Add(endKf);
            animationRun = animation.RunAsync(this, null);
            await animationRun;
        }
        public bool IsCompleted => animationRun.IsCompleted;
    }
}
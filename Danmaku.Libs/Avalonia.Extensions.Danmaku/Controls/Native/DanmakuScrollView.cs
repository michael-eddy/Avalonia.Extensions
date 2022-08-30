using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Extensions.Styles;
using Avalonia.Styling;
using System;
using System.Diagnostics;

namespace Avalonia.Extensions.Danmaku
{
    public class DanmakuScrollView : TextBlock, IStyling
    {
        Type IStyleable.StyleKey => typeof(TextBlock);
        private readonly Animation.Animation animation;
        public DanmakuScrollView()
        {
            //Margin = new Thickness(900, 0, 0, 0);
            animation = new Animation.Animation();
            Init();
        }
        private async void Init()
        {
            animation.FillMode = FillMode.Forward;
            animation.Duration = TimeSpan.FromSeconds(10);
            KeyFrame startKf = new KeyFrame
            {
                Cue = new Cue(0.0),
                Setters = { new Setter(MarginProperty, new Thickness(900, 0, 0, 0)) }
            };
            animation.Children.Add(startKf);
            KeyFrame endKf = new KeyFrame
            {
                Cue = new Cue(1.0),
                Setters = { new Setter(MarginProperty, new Thickness(-900, 0, 0, 0)) }
            };
            animation.Children.Add(endKf);
            var animationRun = animation.RunAsync(this, null);
            await animationRun;
            Debug.WriteLine(animationRun.IsCompleted);
        }
    }
}
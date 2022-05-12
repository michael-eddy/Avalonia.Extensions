using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.Extensions.Media
{
    public sealed class MusicPlayer : Window
    {
        public MusicPlayer(PixelPoint point)
        {
            Position = point;
        }
        public override void Show()
        {
            base.Show();
        }
    }
}
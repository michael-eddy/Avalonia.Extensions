using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Controls
{
    public partial class PopupToast : Window
    {
        public PopupToast() : base()
        {
            Opacity = 0.6;
            Topmost = true;
            CanResize = false;
            ShowInTaskbar = false;
            SystemDecorations = SystemDecorations.None;
        }
        public void Popup(string content)
        {
            PopupOptions options = new PopupOptions { ForegroundColor = Colors.White };
            Popup(content, options);
        }
        public void Popup(string content, PopupOptions options = default)
        {
            TextWrapping wrapping = TextWrapping.NoWrap;
            if (double.IsNaN(options.Width))
            {
                var size = PlatformImpl.MeasureString(content, Core.Instance.FontDefault);
                Width = size.Width;
                Height = size.Height;
            }
            else
            {
                var size = PlatformImpl.MeasureString(content, Core.Instance.FontDefault, options.Width);
                Width = size.Width;
                Height = size.Height;
                if (PlatformImpl.MeasureString(content, Core.Instance.FontDefault).Width > size.Width)
                    wrapping = TextWrapping.WrapWithOverflow;
            }
            Content = new TextBlock
            {
                Text = content,
                TextWrapping = wrapping,
                Foreground = options.Foreground,
                VerticalAlignment = options.VerticalAlignment,
                HorizontalAlignment = options.HorizontalAlignment
            };
            Background = options.Background;
            SetLoaction();
            Show();
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await Task.Delay(options.Timeout);
                Close();
            });
        }
        public virtual void SetLoaction()
        {
            var workSize = Screens.Primary.WorkingArea.Size;
            var y = workSize.Height / 8 * 7;
            var x = (workSize.Width - Width) / 2;
            Position = new PixelPoint(Convert.ToInt32(x), y);
        }
    }
}
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
        public static void Show(string content)
        {
            PopupToast toast = new PopupToast();
            toast.Popup(content);
        }
        public static void Show(string content, Window window)
        {
            PopupToast toast = new PopupToast();
            var x = window.Position.X + (toast.PlatformImpl.MeasureString(content, Core.Instance.FontDefault).Width / 2);
            var y = window.ActualHeight() + window.Position.Y - 48;
            var point = new PixelPoint(Convert.ToInt32(x), Convert.ToInt32(y));
            toast.Popup(content, point);
        }
        public static void Show(string content, PopupOptions options)
        {
            PopupToast toast = new PopupToast();
            toast.Popup(content, options);
        }
        private void Popup(string content, PixelPoint? point = null)
        {
            PopupOptions options = new PopupOptions { ForegroundColor = Colors.White };
            Popup(content, point, options);
        }
        private void Popup(string content, PixelPoint? point, PopupOptions options)
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
            if (!point.HasValue)
            {
                var workSize = Screens.Primary.WorkingArea.Size;
                var y = workSize.Height / 8 * 7;
                var x = (workSize.Width - Width) / 2;
                Position = new PixelPoint(Convert.ToInt32(x), y);
            }
            else
                Position = point.Value;
            Show();
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await Task.Delay(options.Timeout);
                Close();
            });
        }
        private void Popup(string content, PopupOptions options)
        {
            Popup(content, null, options);
        }
    }
}
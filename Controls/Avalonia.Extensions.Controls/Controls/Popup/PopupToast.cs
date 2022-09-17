using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using SkiaSharp;
using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Controls
{
    public partial class PopupToast : Window
    {
        private readonly TextBlock textBlock;
        public PopupToast() : base()
        {
            Opacity = 0.6;
            Topmost = true;
            CanResize = false;
            ShowInTaskbar = false;
            textBlock = new TextBlock();
            SystemDecorations = SystemDecorations.None;
        }
        public static void Show(string content) => new PopupToast().Popup(content);
        public static void Show(string content, Window window)
        {
            PopupToast toast = new PopupToast();
            var typeface = SKTypeface.FromFamilyName(toast.textBlock.FontFamily.Name);
            var x = window.Position.X + (content.MeasureString(toast.textBlock.FontSize, typeface).Width / 2);
            var y = window.ActualHeight() + window.Position.Y - 48;
            var point = new PixelPoint(Convert.ToInt32(x), Convert.ToInt32(y));
            toast.Popup(content, point);
        }
        public static void Show(string content, PopupOptions options) => new PopupToast().Popup(content, options);
        private void Popup(string content, PixelPoint? point = null) => Popup(content, point, new PopupOptions { ForegroundColor = Colors.White });
        private void Popup(string content, PixelPoint? point, PopupOptions options)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                TextWrapping wrapping = TextWrapping.NoWrap;
                textBlock.Text = content;
                textBlock.TextWrapping = wrapping;
                textBlock.Foreground = options.Foreground;
                textBlock.VerticalAlignment = options.VerticalAlignment;
                textBlock.HorizontalAlignment = options.HorizontalAlignment;
                Content = textBlock;
                var typeface = SKTypeface.FromFamilyName(textBlock.FontFamily.Name);
                if (double.IsNaN(options.Width))
                {
                    var size = content.MeasureString(FontSize, typeface);
                    Width = size.Width;
                    Height = size.Height;
                }
                else
                {
                    var size = content.MeasureString(FontSize, typeface);
                    Width = size.Width;
                    Height = size.Height;
                    if (content.MeasureString(FontSize, typeface).Width > size.Width)
                        wrapping = TextWrapping.WrapWithOverflow;
                }
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
                await Task.Delay(options.Timeout);
                Close();
            }, DispatcherPriority.ApplicationIdle);
        }
        private void Popup(string content, PopupOptions options) => Popup(content, null, options);
    }
}
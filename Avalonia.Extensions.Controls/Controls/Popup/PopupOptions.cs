using Avalonia.Layout;
using Avalonia.Media;

namespace Avalonia.Extensions.Controls
{
    public partial class PopupToast
    {
        public sealed class PopupOptions
        {
            public int Timeout => (int)Length;
            public float Width { get; set; } = float.NaN;
            public Color ForegroundColor { get; set; } = Colors.Black;
            public Color BackgroundColor { get; set; } = Color.Parse("#333");
            public PopupLength Length { get; set; } = PopupLength.Default;
            internal IBrush Foreground => new SolidColorBrush(ForegroundColor);
            internal IBrush Background => new SolidColorBrush(BackgroundColor);
            public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;
            public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
        }
    }
}
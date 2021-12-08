using Avalonia.Media.Imaging;
using System;

namespace Avalonia.Extensions.Media
{
    public interface IBitmapSource
    {
        Uri Source { get; set; }
        Bitmap BitmapSource { set; }
    }
}
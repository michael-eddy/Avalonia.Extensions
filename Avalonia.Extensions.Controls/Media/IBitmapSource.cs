using Avalonia.Media.Imaging;
using System;
using System.IO;

namespace Avalonia.Extensions.Media
{
    public interface IBitmapSource
    {
        Uri Source { get; set; }
        Bitmap Bitmap { get; set; }
        void SetBitmapSource(Stream stream);
    }
}
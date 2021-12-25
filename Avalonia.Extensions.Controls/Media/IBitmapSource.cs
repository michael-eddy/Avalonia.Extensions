using Avalonia.Media.Imaging;
using Avalonia.Styling;
using System;
using System.IO;

namespace Avalonia.Extensions.Media
{
    public interface IBitmapSource : IStyleable
    {
        Uri Source { get; set; }
        Bitmap Bitmap { get; set; }
        void SetBitmapSource(Stream stream);
    }
}
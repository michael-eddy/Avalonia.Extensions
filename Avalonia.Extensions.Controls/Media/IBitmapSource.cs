using System;
using System.ComponentModel;
using System.IO;

namespace Avalonia.Extensions.Media
{
    public interface IBitmapSource : INotifyPropertyChanged
    {
        Uri Source { get; set; }
        void SetBitmapSource(Stream stream);
    }
}
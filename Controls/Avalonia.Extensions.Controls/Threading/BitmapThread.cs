using Avalonia.Extensions.Media;
using System;

namespace Avalonia.Extensions.Threading
{
    internal sealed class BitmapThread(IBitmapSource owner)
    {
        private IBitmapSource Owner { get; } = owner;
        public void Run(Uri uri)
        {
            Owner.ToStream(uri);
        }
    }
}
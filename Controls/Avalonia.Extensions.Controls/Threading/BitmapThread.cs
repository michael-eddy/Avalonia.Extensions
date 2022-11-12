using Avalonia.Extensions.Media;
using System;

namespace Avalonia.Extensions.Threading
{
    internal sealed class BitmapThread
    {
        private IBitmapSource Owner { get; }
        public BitmapThread(IBitmapSource owner)
        {
            Owner = owner;
        }
        public void Run(Uri uri)
        {
            Owner.ToStream(uri);
        }
    }
}
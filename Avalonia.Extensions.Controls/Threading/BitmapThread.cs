using Avalonia.Controls;
using Avalonia.Extensions.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.Extensions.Threading
{
    internal class BitmapThread : IDisposable
    {
        private int HashCode;
        private IBitmapSource Owner { get; }
        public BitmapThread(IBitmapSource source)
        {
            Owner = source;
        }
        public void Update()
        {
            if (Owner.Source != null)
            {
                var hashCode = Owner.Source.GetHashCode();
                if (HashCode != hashCode)
                {
                    HashCode = hashCode;
                    switch (Owner.Source.Scheme)
                    {
                        case "avares":
                            {

                                break;
                            }
                        case "http":
                        case "https":
                            {

                                break;
                            }
                    }
                }
            }
        }
        public void Dispose()
        {

        }
    }
}
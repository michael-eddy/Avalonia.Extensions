using Avalonia.Interactivity;

namespace Avalonia.Extensions.Event
{
    public class SizeRoutedEventArgs : RoutedEventArgs
    {
        public Size Size { get; }
        public SizeRoutedEventArgs(RoutedEvent args, Size size) : base(args)
        {
            Size = size;
        }
        public double Width => Size.Width;
        public double Height => Size.Height;
    }
}
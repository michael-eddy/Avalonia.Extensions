using Avalonia.Interactivity;

namespace Avalonia.Extensions.Media
{
    public class MediaFailedEventArgs : RoutedEventArgs
    {
        public string Message { get; }
        public MediaFailedEventArgs(RoutedEvent args, string msg) : base(args)
        {
            Message = msg;
        }
    }
}
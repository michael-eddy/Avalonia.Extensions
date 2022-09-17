using Avalonia.Interactivity;

namespace Avalonia.Extensions.Event
{
    public class ValueChangeEventArgs : RoutedEventArgs
    {
        public object OldValue { get; }
        public object NewValue { get; }
        public ValueChangeEventArgs(RoutedEvent args, object newValue, object oldValue) : base(args)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
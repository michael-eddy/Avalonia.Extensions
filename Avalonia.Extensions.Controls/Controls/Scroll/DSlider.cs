using Avalonia.Controls;
using Avalonia.Extensions.Event;
using Avalonia.Interactivity;
using System;

namespace Avalonia.Extensions.Controls
{
    public class DSlider : Slider
    {
        public DSlider()
        {
            ValueProperty.Changed.AddClassHandler<DSlider>(OnValueChange);
        }
        /// <summary>
        /// Defines the <see cref="ScrollEnd"/> event.
        /// </summary>
        public static readonly RoutedEvent<ValueChangeEventArgs> ValueChangeEvent =
           RoutedEvent.Register<DSlider, ValueChangeEventArgs>(nameof(ValueChange), RoutingStrategies.Bubble);
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<ValueChangeEventArgs> ValueChange
        {
            add { AddHandler(ValueChangeEvent, value); }
            remove { RemoveHandler(ValueChangeEvent, value); }
        }
        private void OnValueChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue())
            {
                var args = new ValueChangeEventArgs(ValueChangeEvent, e.NewValue, e.OldValue);
                RaiseEvent(args);
                if (!args.Handled)
                    args.Handled = true;
            }
        }
    }
}
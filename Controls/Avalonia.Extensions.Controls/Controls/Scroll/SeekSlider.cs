using Avalonia.Controls;
using Avalonia.Extensions.Event;
using Avalonia.Extensions.Styles;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System;

namespace Avalonia.Extensions.Controls
{
    public class SeekSlider : Slider, IStyling
    {
        Type IStyleable.StyleKey => typeof(Slider);
        static SeekSlider()
        {
            ValueProperty.Changed.AddClassHandler<SeekSlider>(OnValueChange);
        }
        /// <summary>
        /// Defines the <see cref="ScrollEnd"/> event.
        /// </summary>
        public static readonly RoutedEvent<ValueChangeEventArgs> ValueChangeEvent =
           RoutedEvent.Register<SeekSlider, ValueChangeEventArgs>(nameof(ValueChange), RoutingStrategies.Bubble);
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<ValueChangeEventArgs> ValueChange
        {
            add { AddHandler(ValueChangeEvent, value); }
            remove { RemoveHandler(ValueChangeEvent, value); }
        }
        private static void OnValueChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (sender is SeekSlider slider && !e.IsSameValue())
            {
                var args = new ValueChangeEventArgs(ValueChangeEvent, e.NewValue, e.OldValue);
                slider.RaiseEvent(args);
                if (!args.Handled)
                    args.Handled = true;
            }
        }
    }
}
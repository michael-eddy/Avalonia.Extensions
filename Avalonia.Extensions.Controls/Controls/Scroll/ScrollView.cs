using Avalonia.Controls;
using Avalonia.Extensions.Styles;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System;

namespace Avalonia.Extensions.Controls
{
    public class ScrollView : ScrollViewer, IStyling
    {
        private double lastSize = -1;
        public ScrollView() : base() { }
        Type IStyleable.StyleKey => typeof(ScrollViewer);
        /// <summary>
        /// Defines the <see cref="ScrollTop"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> ScrollTopEvent =
           RoutedEvent.Register<ScrollView, RoutedEventArgs>(nameof(ScrollTop), RoutingStrategies.Bubble);
        public event EventHandler<RoutedEventArgs> ScrollTop
        {
            add { AddHandler(ScrollTopEvent, value); }
            remove { RemoveHandler(ScrollTopEvent, value); }
        }
        /// <summary>
        /// Defines the <see cref="ScrollEnd"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> ScrollEndEvent =
           RoutedEvent.Register<ScrollView, RoutedEventArgs>(nameof(ScrollEnd), RoutingStrategies.Bubble);
        public event EventHandler<RoutedEventArgs> ScrollEnd
        {
            add { AddHandler(ScrollEndEvent, value); }
            remove { RemoveHandler(ScrollEndEvent, value); }
        }
        /// <summary>
        /// Called when a change in scrolling state is detected, such as a change in scroll
        /// position, extent, or viewport size.
        /// </summary>
        /// <param name="e">The event args.</param>
        /// <remarks>
        /// If you override this method, call `base.OnScrollChanged(ScrollChangedEventArgs)` to
        /// ensure that this event is raised.
        /// </remarks>
        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            base.OnScrollChanged(e);
            if (e.Source is ScrollViewer scrollViewer && scrollViewer.Offset.Y > 0)
            {
                if (Content is Control child)
                {
                    if (scrollViewer.Offset.Y == 0 && lastSize != 0)
                    {
                        var args = new RoutedEventArgs(ScrollTopEvent);
                        RaiseEvent(args);
                        if (!args.Handled)
                            args.Handled = true;
                    }
                    else if ((scrollViewer.Offset.Y + Bounds.Height) >= child.Bounds.Height)
                    {
                        var args = new RoutedEventArgs(ScrollEndEvent);
                        RaiseEvent(args);
                        if (!args.Handled)
                            args.Handled = true;
                    }
                    lastSize = scrollViewer.Offset.Y;
                }
            }
        }
    }
}
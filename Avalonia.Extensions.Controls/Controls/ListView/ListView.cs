using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Extensions.Event;
using Avalonia.Extensions.Styles;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Styling;
using Avalonia.Threading;
using PCLUntils.IEnumerables;
using System;
using System.Linq;
using System.Windows.Input;

namespace Avalonia.Extensions.Controls
{
    public class ListView : ListBox, IStyling
    {
        /// <summary>
        /// Defines the <see cref="ItemClick"/> property.
        /// </summary>
        public static readonly RoutedEvent<ViewRoutedEventArgs> ItemClickEvent =
            RoutedEvent.Register<ListView, ViewRoutedEventArgs>(nameof(ItemClick), RoutingStrategies.Bubble);
        /// <summary>
        /// Defines the <see cref="Clickable"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> ClickableProperty =
          AvaloniaProperty.Register<ListView, bool>(nameof(Clickable), true);
        /// <summary>
        /// Defines the <see cref="ScrollTop"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> ScrollTopEvent =
           RoutedEvent.Register<ListView, RoutedEventArgs>(nameof(ScrollTop), RoutingStrategies.Bubble);
        /// <summary>
        /// Defines the <see cref="ScrollTop"/> event.
        /// </summary>
        public static readonly RoutedEvent<SizeRoutedEventArgs> SizeChangeEvent =
           RoutedEvent.Register<ListView, SizeRoutedEventArgs>(nameof(SizeChange), RoutingStrategies.Bubble);
        /// <summary>
        /// Defines the <see cref="ScrollEnd"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> ScrollEndEvent =
           RoutedEvent.Register<ListView, RoutedEventArgs>(nameof(ScrollEnd), RoutingStrategies.Bubble);
        /// <summary>
        /// Defines the <see cref="Command"/> property.
        /// </summary>
        public static readonly DirectProperty<ListView, ICommand> CommandProperty =
             AvaloniaProperty.RegisterDirect<ListView, ICommand>(nameof(Command), content => content.Command, (content, command) => content.Command = command, enableDataValidation: true);
        /// <summary>
        /// Defines the <see cref="ClickMode"/> property.
        /// </summary>
        public static readonly StyledProperty<ClickMode> ClickModeProperty =
            AvaloniaProperty.Register<ClickableView, ClickMode>(nameof(ClickMode), ClickMode.Press);
        /// <summary>
        /// Gets or sets a value indicating how the <see cref="ClickableView"/> should react to clicks.
        /// </summary>
        public ClickMode ClickMode
        {
            get => GetValue(ClickModeProperty);
            set => SetValue(ClickModeProperty, value);
        }
        /// <summary>
        /// Raised when the user clicks the child item.
        /// </summary>
        public event EventHandler<ViewRoutedEventArgs> ItemClick
        {
            add { AddHandler(ItemClickEvent, value); }
            remove { RemoveHandler(ItemClickEvent, value); }
        }
        /// <summary>
        /// is item clickenable,default value is true
        /// </summary>
        public bool Clickable
        {
            get => GetValue(ClickableProperty);
            set => SetValue(ClickableProperty, value);
        }
        /// <summary>
        /// Gets or sets an <see cref="ICommand"/> to be invoked when the child item is clicked.
        /// </summary>
        public ICommand Command
        {
            get => _command;
            set => SetAndRaise(CommandProperty, ref _command, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<RoutedEventArgs> ScrollTop
        {
            add { AddHandler(ScrollTopEvent, value); }
            remove { RemoveHandler(ScrollTopEvent, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<SizeRoutedEventArgs> SizeChange
        {
            add { AddHandler(SizeChangeEvent, value); }
            remove { RemoveHandler(SizeChangeEvent, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<RoutedEventArgs> ScrollEnd
        {
            add { AddHandler(ScrollEndEvent, value); }
            remove { RemoveHandler(ScrollEndEvent, value); }
        }
        private ICommand _command;
        public MouseButton MouseClickButton { get; private set; }
        static ListView()
        {
            ViewProperty.Changed.AddClassHandler<Grid>(OnViewChanged);
            SelectionModeProperty.OverrideMetadata<ListView>(new StyledPropertyMetadata<SelectionMode>(SelectionMode.Multiple));
        }
        private void OnBoundsChange(ListView view, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is Size size)
            {
                var args = new SizeRoutedEventArgs(SizeChangeEvent, size);
                RaiseEvent(args);
                if (!args.Handled)
                    args.Handled = true;
            }
        }
        public ListView()
        {
            SelectionChangedEvent.Raised.Subscribe(OnSelectionChanged);
            ScrollProperty.Changed.AddClassHandler<ListView>(OnScrollChange);
            BoundsProperty.Changed.AddClassHandler<ListView>(OnBoundsChange);
        }
        protected virtual void OnScrollChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is ScrollViewer scrollViewer)
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        }
        private bool trigger = true;
        protected virtual void ScrollEventHandle(ScrollViewer scrollViewer)
        {
            try
            {
                if (scrollViewer.Content is IControl child && child.VisualChildren.FirstOrDefault() is VirtualizingStackPanel virtualizing)
                {
                    var isFirstItem = Items.IsFirst((virtualizing.Children.FirstOrDefault() as ListBoxItem)?.Content);
                    var isLastItem = Items.IsLast((virtualizing.Children.LastOrDefault() as ListBoxItem)?.Content);
                    if (isFirstItem && !trigger)
                    {
                        trigger = true;
                        var args = new RoutedEventArgs(ScrollTopEvent);
                        RaiseEvent(args);
                        if (!args.Handled)
                            args.Handled = true;
                    }
                    else if (isLastItem && !trigger)
                    {
                        trigger = true;
                        var args = new RoutedEventArgs(ScrollEndEvent);
                        RaiseEvent(args);
                        if (!args.Handled)
                            args.Handled = true;
                    }
                    else if (!isFirstItem && !isLastItem)
                    {
                        trigger = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (e.Source is ScrollViewer scrollViewer)
                    ScrollEventHandle(scrollViewer);
            });
        }
        private void OnSelectionChanged((object, RoutedEventArgs) obj)
        {
            if (obj.Item2.Source is ListBoxItem listBoxItem)
                OnContentClick(listBoxItem, MouseButton.Left);
        }
        public static readonly AvaloniaProperty ViewProperty = AvaloniaProperty.Register<ListView, ListViewBase>(nameof(View));
        public ListViewBase View
        {
            get => (ListViewBase)GetValue(ViewProperty);
            set => SetValue(ViewProperty, value);
        }
        private static void OnViewChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            if (d is ListView listView)
            {
                ListViewBase oldView = (ListViewBase)e.OldValue;
                ListViewBase newView = (ListViewBase)e.NewValue;
                if (newView != null)
                {
                    if (newView.IsUsed)
                        throw new InvalidOperationException("View cannot be shared between multiple instances of ListView");
                    newView.IsUsed = true;
                }
                listView.PreviousView = oldView;
                listView.ApplyNewView();
                listView.PreviousView = newView;
                if (oldView != null)
                    oldView.IsUsed = false;
            }
        }
        public Type Defaultstyle { get; private set; }
        Type IStyleable.StyleKey => typeof(ListBox);
        private void ApplyNewView()
        {
            ListViewBase newView = View;
            Defaultstyle = newView?.DefaultStyleKey;
        }
        public ListViewBase PreviousView { get; private set; }
        protected override IItemContainerGenerator CreateItemContainerGenerator() =>
            new ItemsGenerator(this, ContentControl.ContentProperty, ContentControl.ContentTemplateProperty);
        /// <summary>
        /// handle clild item click event,
        /// trigger the <seealso cref="Command"/> and <seealso cref="ItemClickEvent"/>
        /// when child item has been click
        /// </summary>
        /// <param name="viewCell"></param>
        internal virtual void OnContentClick(object control, MouseButton mouseButton)
        {
            if (Clickable && control != null)
            {
                MouseClickButton = mouseButton;
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var args = new ViewRoutedEventArgs(ItemClickEvent, mouseButton, control);
                    RaiseEvent(args);
                    if (control is ListViewItem viewCell)
                    {
                        SelectedItem = viewCell;
                        if (!args.Handled)
                            args.Handled = true;
                    }
                });
            }
        }
    }
}
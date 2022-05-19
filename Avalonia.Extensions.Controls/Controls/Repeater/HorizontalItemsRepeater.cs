using Avalonia.Controls;
using Avalonia.Extensions.Event;
using Avalonia.Extensions.Styles;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using PCLUntils.Assemblly;
using System;
using System.Windows.Input;

namespace Avalonia.Extensions.Controls
{
    /// <summary>
    /// the itemsrepeater layout <seealso cref="this.Orientation"/> as <seealso cref="Orientation.Horizontal"/>
    /// </summary>
    public class HorizontalItemsRepeater : ItemsRepeater, IStyling
    {
        private ICommand _command;
        /// <summary>
        /// create an instance
        /// </summary>
        public HorizontalItemsRepeater()
        {
            DrawLayout();
        }
        /// <summary>
        /// Defines the <see cref="Loaded"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> LoadedEvent =
           RoutedEvent.Register<HorizontalItemsRepeater, RoutedEventArgs>(nameof(Loaded), RoutingStrategies.Bubble);
        /// <summary>
        /// Defines the <see cref="ItemClick"/> property.
        /// </summary>
        public static readonly RoutedEvent<ViewRoutedEventArgs> ItemClickEvent =
            RoutedEvent.Register<HorizontalItemsRepeater, ViewRoutedEventArgs>(nameof(ItemClick), RoutingStrategies.Bubble);
        /// <summary>
        /// Defines the <see cref="SelectedItem"/> property.
        /// </summary>
        public static readonly StyledProperty<ItemsRepeaterContent> SelectedItemProperty =
          AvaloniaProperty.Register<HorizontalItemsRepeater, ItemsRepeaterContent>(nameof(SelectedItem), null);
        /// <summary>
        /// Defines the <see cref="Clickable"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> ClickableProperty =
          AvaloniaProperty.Register<HorizontalItemsRepeater, bool>(nameof(Clickable), true);
        /// <summary>
        /// Defines the <see cref="Spacing"/> property.
        /// </summary>
        public static readonly StyledProperty<double> SpacingProperty =
          AvaloniaProperty.Register<HorizontalItemsRepeater, double>(nameof(Spacing));
        /// <summary>
        /// Defines the <see cref="Command"/> property.
        /// </summary>
        public static readonly DirectProperty<HorizontalItemsRepeater, ICommand> CommandProperty =
             AvaloniaProperty.RegisterDirect<HorizontalItemsRepeater, ICommand>(nameof(Command), content => content.Command,
                 (content, command) => content.Command = command, enableDataValidation: true);
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
        public event EventHandler<RoutedEventArgs> Loaded
        {
            add { AddHandler(LoadedEvent, value); }
            remove { RemoveHandler(LoadedEvent, value); }
        }
        /// <summary>
        /// Gets or sets the clicked child item
        /// </summary>
        public ItemsRepeaterContent SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
        /// <summary>
        ///  Gets or sets a uniform distance (in pixels) between stacked items. It is applied
        ///  in the direction of the StackLayout's Orientation.
        /// </summary>
        public double Spacing
        {
            get => GetValue(SpacingProperty);
            set
            {
                SetValue(SpacingProperty, value);
                DrawLayout();
            }
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
        /// handle clild item click event,
        /// trigger the <seealso cref="Command"/> and <seealso cref="ItemClickEvent"/>
        /// when child item has been click, but <seealso cref="Clickable"/> must be true
        /// </summary>
        internal void OnContentClick(ItemsRepeaterContent itemsRepeaterContent, MouseButton mouseButton)
        {
            if (Clickable && itemsRepeaterContent != null)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var args = new ViewRoutedEventArgs(ItemClickEvent, mouseButton);
                    SelectedItem = itemsRepeaterContent;
                    RaiseEvent(args);
                    if (!args.Handled && Command?.CanExecute(itemsRepeaterContent.CommandParameter) == true)
                    {
                        Command.Execute(itemsRepeaterContent.CommandParameter);
                        args.Handled = true;
                    }
                });
            }
        }
        private void DrawLayout()
        {
            Layout = new StackLayout
            {
                Spacing = Spacing,
                Orientation = Orientation.Horizontal
            };
        }
        protected bool IsLayoutInProgress => this.GetPrivateField<bool>("_isLayoutInProgress");
    }
}
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Extensions.Styles;
using Avalonia.Input;
using Avalonia.VisualTree;
using System;
using System.Linq;

namespace Avalonia.Extensions.Controls
{
    [PseudoClasses(":pressed", ":selected")]
    public class ListViewItem : ListBoxItem, IStyling
    {
        public Type StyleKey => typeof(ListBoxItem);
        private ClickMode ClickMode
        {
            get
            {
                if (Parent is ListView listView)
                    return listView.ClickMode;
                return ClickMode.Press;
            }
        }
        public ListViewItem()
        {
            IsCancelProperty.Changed.Subscribe(IsCancelChanged);
            IsDefaultProperty.Changed.Subscribe(IsDefaultChanged);
        }
        private void IsDefaultChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var content = e.Sender as ListViewItem;
            if (content?.VisualRoot is IInputElement inputRoot)
            {
                if (e.NewValue is true)
                    content.ListenForDefault(inputRoot);
                else
                    content.StopListeningForDefault(inputRoot);
            }
        }
        private void IsCancelChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is ListViewItem content && content?.VisualRoot is IInputElement inputRoot)
            {
                if (e.NewValue is true)
                    content.ListenForCancel(inputRoot);
                else
                    content.StopListeningForCancel(inputRoot);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the clickableview is the default clickableview for the window.
        /// </summary>
        public bool IsDefault
        {
            get => GetValue(IsDefaultProperty);
            set => SetValue(IsDefaultProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="IsDefaultProperty"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsDefaultProperty =
            AvaloniaProperty.Register<ClickableView, bool>(nameof(IsDefault));
        /// <summary>
        /// Gets or sets a value indicating whether the clickableview is the Cancel clickableview for the window.
        /// </summary>
        public bool IsCancel
        {
            get => GetValue(IsCancelProperty);
            set => SetValue(IsCancelProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="IsCancelProperty"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsCancelProperty =
            AvaloniaProperty.Register<ClickableView, bool>(nameof(IsCancel));
        public bool IsPressed
        {
            get => GetValue(IsPressedProperty);
            private set => SetValue(IsPressedProperty, value);
        }
        public static readonly StyledProperty<bool> IsPressedProperty =
           AvaloniaProperty.Register<ClickableView, bool>(nameof(IsPressed));
        /// <summary>
        /// Invokes the <see cref="Click"/> event.
        /// </summary>
        protected virtual void OnClick(MouseButton mouseButton)
        {
            if (Parent is ListView listView)
                listView.OnContentClick(this, MouseButton.Left);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnClick(MouseButton.Left);
                e.Handled = true;
            }
            else if (e.Key == Key.Space)
            {
                if (ClickMode == ClickMode.Press)
                    OnClick(MouseButton.Left);
                IsPressed = true;
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (ClickMode == ClickMode.Release)
                    OnClick(MouseButton.Left);
                IsPressed = false;
                e.Handled = true;
            }
        }
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            var properties = e.GetCurrentPoint(this).Properties;
            if (properties.IsLeftButtonPressed)
            {
                IsPressed = true;
                e.Handled = true;
                if (ClickMode == ClickMode.Press)
                    OnClick(MouseButton.Left);
            }
            else if (properties.IsRightButtonPressed)
            {
                IsPressed = true;
                e.Handled = true;
                if (ClickMode == ClickMode.Press)
                    OnClick(MouseButton.Right);
            }
        }
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (IsPressed)
            {
                IsPressed = false;
                e.Handled = true;
                if (ClickMode == ClickMode.Release && this.GetVisualsAt(e.GetPosition(this)).Any(c => this == c || this.IsVisualAncestorOf(c)))
                {
                    if (e.InitialPressMouseButton == MouseButton.Left)
                        OnClick(MouseButton.Left);
                    else if (e.InitialPressMouseButton == MouseButton.Right)
                        OnClick(MouseButton.Right);
                }
            }
        }
        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
        {
            IsPressed = false;
        }
        protected override void UpdateDataValidation<T>(AvaloniaProperty<T> property, BindingValue<T> value)
        {
            base.UpdateDataValidation(property, value);
            if (value.Type == BindingValueType.BindingError)
                UpdateIsEffectivelyEnabled();
        }
        /// <summary>
        /// Starts listening for the Enter key when the clickableview <see cref="IsDefault"/>.
        /// </summary>
        /// <param name="root">The input root.</param>
        private void ListenForDefault(IInputElement root)
        {
            root.AddHandler(KeyDownEvent, RootDefaultKeyDown);
        }
        /// <summary>
        /// Starts listening for the Escape key when the clickableview <see cref="IsCancel"/>.
        /// </summary>
        /// <param name="root">The input root.</param>
        private void ListenForCancel(IInputElement root)
        {
            root.AddHandler(KeyDownEvent, RootCancelKeyDown);
        }
        /// <summary>
        /// Stops listening for the Enter key when the clickableview is no longer <see cref="IsDefault"/>.
        /// </summary>
        /// <param name="root">The input root.</param>
        private void StopListeningForDefault(IInputElement root)
        {
            root.RemoveHandler(KeyDownEvent, RootDefaultKeyDown);
        }
        /// <summary>
        /// Stops listening for the Escape key when the clickableview is no longer <see cref="IsCancel"/>.
        /// </summary>
        /// <param name="root">The input root.</param>
        private void StopListeningForCancel(IInputElement root)
        {
            root.RemoveHandler(KeyDownEvent, RootCancelKeyDown);
        }
        /// <summary>
        /// Called when a key is pressed on the input root and the clickableview <see cref="IsCancel"/>.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void RootCancelKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && IsVisible && IsEnabled)
                OnClick(MouseButton.Left);
        }
        /// <summary>
        /// Called when a key is pressed on the input root and the clickableview <see cref="IsDefault"/>.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void RootDefaultKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && IsVisible && IsEnabled)
                OnClick(MouseButton.Left);
        }
    }
}
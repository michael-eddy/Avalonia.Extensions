using Avalonia.Extensions.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Extensions.Event
{
    public sealed class ViewRoutedEventArgs : RoutedEventArgs
    {
        public object ClickItem { get; set; }
        public MouseButton ClickMouse { get; }
        public bool IsLeftClick => ClickMouse == MouseButton.Left;
        public bool IsRightClick => ClickMouse == MouseButton.Right;
        public bool GetItemContent<T>(out T content)
        {
            if (ClickItem is ListViewItem item)
            {
                if (item.Content is T obj)
                {
                    content = obj;
                    return true;
                }
            }
            content = default;
            return false;
        }
        public ViewRoutedEventArgs(RoutedEvent args, MouseButton mouseButton) : base(args)
        {
            ClickMouse = mouseButton;
        }
        public ViewRoutedEventArgs(RoutedEvent args, MouseButton mouseButton, object item) : base(args)
        {
            ClickItem = item;
            ClickMouse = mouseButton;
        }
    }
}
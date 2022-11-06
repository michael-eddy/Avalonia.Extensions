using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Extensions.Event
{
    public sealed class ClickEventArgs : RoutedEventArgs
    {
        public Point Point { get; }
        public MouseButton ClickMouse { get; }
        public bool IsLeftClick => ClickMouse == MouseButton.Left;
        public bool IsRightClick => ClickMouse == MouseButton.Right;
        public ClickEventArgs(RoutedEvent args, MouseButton mouseButton, Point pos) : base(args)
        {
            Point = pos;
            ClickMouse = mouseButton;
        }
    }
}
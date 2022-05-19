using Avalonia.Controls;
using Avalonia.Input;
using PCLUntils.Objects;

namespace Avalonia.Extensions.Controls
{
    public class WindowBase : Window
    {
        public WindowBase() : base()
        {
            MousePisition = new PixelPoint();
        }
        public bool Locate
        {
            get => GetValue(LocateProperty);
            set => SetValue(LocateProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="Locate"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> LocateProperty =
            AvaloniaProperty.Register<WindowBase, bool>(nameof(Locate), false);
        public PixelPoint MousePisition { get; private set; }
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            if (Locate)
            {
                var p = e.GetCurrentPoint(this);
                var x = (Position.X + p.Position.X).ToInt32();
                var y = (Position.Y + p.Position.Y).ToInt32();
                MousePisition = new PixelPoint(x, y);
            }
        }
    }
}
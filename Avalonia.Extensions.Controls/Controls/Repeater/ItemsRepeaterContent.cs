using Avalonia.Controls;
using Avalonia.Input;

namespace Avalonia.Extensions.Controls
{
    /// <summary>
    /// the child item panel/layout for <seealso cref="ItemsRepeater"/>
    /// </summary>
    public sealed class ItemsRepeaterContent : ClickableView
    {
        protected override void OnClick(MouseButton mouseButton)
        {
            if (Parent is HorizontalItemsRepeater hRepeater)
                hRepeater.OnContentClick(this, mouseButton);
            else if (Parent is VerticalItemsRepeater vRepeater)
                vRepeater.OnContentClick(this, mouseButton);
            base.OnClick(mouseButton);
        }
    }
}
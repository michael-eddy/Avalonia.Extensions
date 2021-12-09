using Avalonia.Controls;
using System;

namespace Avalonia.Extensions.Controls
{
    public abstract class ViewBase : AvaloniaObject
    {
        protected internal virtual void PrepareItem(ListViewItem item) { }
        protected internal virtual void ClearItem(ListViewItem item) { }
        protected internal virtual Type DefaultStyleKey => typeof(ListBox);
        protected internal virtual Type ItemContainerDefaultStyleKey => typeof(ListBoxItem);
        internal virtual void OnThemeChanged() { }
        internal bool IsUsed { get; set; }
    }
}
using Avalonia.Controls;
using System;

namespace Avalonia.Extensions.Controls
{
    public abstract class ListViewBase : AvaloniaObject
    {
        internal bool IsUsed { get; set; }
        internal virtual void OnThemeChanged() { }
        protected internal virtual void ClearItem(ListViewItem item) { }
        protected internal virtual void PrepareItem(ListViewItem item) { }
        protected internal virtual Type DefaultStyleKey => typeof(ListBox);
        protected internal virtual Type ItemContainerDefaultStyleKey => typeof(ListBoxItem);
    }
}
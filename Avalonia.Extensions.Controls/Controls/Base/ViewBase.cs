using Avalonia.Controls;
using System;

namespace Avalonia.Extensions.Controls
{
    /// <summary>
    /// fork from https://github.com/jhofinger/Avalonia/tree/listview
    /// </summary>
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
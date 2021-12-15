using Avalonia.Controls;
using Avalonia.Extensions.Styles;
using Avalonia.LogicalTree;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Avalonia.Extensions.Controls
{
    public class CheckBoxList : ItemsRepeater, IStyling
    {
        private object _viewportManager;
        public CheckBoxList()
        {
            this.AddStyles(ItemTemplateProperty);
            _viewportManager = this.GetPrivateField("_viewportManager");
        }
        public new IEnumerable<GroupViewItem> Items
        {
            get => this.GetPrivateField<IEnumerable<GroupViewItem>>("_items");
            set
            {
                var _items = this.GetPrivateField<IEnumerable>("_items");
                SetAndRaise(ItemsControl.ItemsProperty, ref _items, value);
            }
        }
        public static new readonly DirectProperty<CheckBoxList, IEnumerable> ItemsProperty =
            ItemsControl.ItemsProperty.AddOwner<CheckBoxList>(o => o.Items, (o, v) => o.Items = (IEnumerable<GroupViewItem>)v);
    }
}
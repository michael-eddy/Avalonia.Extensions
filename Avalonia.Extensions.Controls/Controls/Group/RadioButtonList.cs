using Avalonia.Controls;
using Avalonia.Extensions.Styles;
using Avalonia.Metadata;
using Avalonia.Styling;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Extensions.Controls
{
    public class RadioButtonList : ItemsRepeater, IStyling
    {
        private string GroupId { get; }
        public RadioButtonList()
        {
            GroupId = Guid.NewGuid().ToString("N");
            this.AddStyles(ItemTemplateProperty);
        }
        public new IEnumerable<GroupViewItem> Items
        {
            get => this.GetPrivateField<IEnumerable<GroupViewItem>>("_items");
            set
            {
                var _items = this.GetPrivateField<IEnumerable>("_items");
                if (value != null)
                {
                    foreach (var item in value)
                        item.Id = GroupId;
                }
                SetAndRaise(ItemsProperty, ref _items, value);
            }
        }
        public static new readonly DirectProperty<RadioButtonList, IEnumerable> ItemsProperty =
            ItemsControl.ItemsProperty.AddOwner<RadioButtonList>(o => o.Items, (o, v) => o.Items = (IEnumerable<GroupViewItem>)v);
    }
}
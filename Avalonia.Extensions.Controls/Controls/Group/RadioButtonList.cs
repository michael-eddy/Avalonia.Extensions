using Avalonia.Controls;
using Avalonia.Extensions.Styles;
using Avalonia.Metadata;
using System;
using System.Collections.Generic;

namespace Avalonia.Extensions.Controls
{
    public class RadioButtonList : ItemsRepeater, IStyling
    {
        private string GroupId { get; }
        public RadioButtonList()
        {
            GroupId = Guid.NewGuid().ToString("N"); 
            AffectsMeasure<RadioButtonList>(ItemsProperty, ItemTemplateProperty);
        }
        [Content]
        public new IEnumerable<GroupBindingModel> Items
        {
            get => (IEnumerable<GroupBindingModel>)base.Items;
            set
            {
                if (value != null)
                {
                    foreach(var item in value)
                        item.Id = GroupId;
                }
                SetValue(ItemsRepeater.ItemsProperty, value);
            }
        }
        public static new readonly DirectProperty<RadioButtonList, IEnumerable<GroupBindingModel>> ItemsProperty =
            AvaloniaProperty.RegisterDirect<RadioButtonList, IEnumerable<GroupBindingModel>>(nameof(Items), o => o.Items, (o, v) => o.Items = v);
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.AddStyles(ItemTemplateProperty);
        }
    }
}
using Avalonia.Controls;
using Avalonia.Extensions.Styles;
using Avalonia.Metadata;
using System.Collections.Generic;

namespace Avalonia.Extensions.Controls
{
    public class CheckBoxList : ItemsRepeater, IStyling
    {
        public CheckBoxList() { }
        [Content]
        public new IEnumerable<GroupBindingModel> Items
        {
            get => (IEnumerable<GroupBindingModel>)base.Items;
            set => SetValue(ItemsRepeater.ItemsProperty, value);
        }
        public static new readonly DirectProperty<CheckBoxList, IEnumerable<GroupBindingModel>> ItemsProperty =
            AvaloniaProperty.RegisterDirect<CheckBoxList, IEnumerable<GroupBindingModel>>(nameof(Items), o => o.Items, (o, v) => o.Items = v);
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.AddStyles(ItemTemplateProperty);
        }
    }
}
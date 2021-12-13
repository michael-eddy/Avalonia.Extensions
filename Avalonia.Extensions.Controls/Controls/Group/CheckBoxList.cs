using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Extensions.Styles;
using System;

namespace Avalonia.Extensions.Controls
{
    public class CheckBoxList : ItemsRepeater, IStyling
    {
        private string GroupId { get; }
        public CheckBoxList()
        {
            GroupId = Guid.NewGuid().ToString("N");
            ItemsProperty.Changed.AddClassHandler<RadioButtonList>(OnItemsChanged);
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.AddStyles<IDataTemplate>(ItemTemplateProperty);
        }
        private void OnItemsChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {

        }
    }
}
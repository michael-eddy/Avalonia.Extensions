using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Extensions.Styles;
using System;

namespace Avalonia.Extensions.Controls
{
    public class RadioButtonList : ItemsRepeater, IStyling
    {
        private string GroupId { get; }
        public RadioButtonList()
        {
            GroupId = Guid.NewGuid().ToString("N");
            ItemsProperty.Changed.AddClassHandler<RadioButtonList>(OnItemsChanged);
        }
        private void OnItemsChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {

        }
    }
}
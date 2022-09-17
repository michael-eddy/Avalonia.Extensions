using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Data;
using System;

namespace Avalonia.Extensions.Controls
{
    internal class ItemsGenerator : ItemContainerGenerator
    {
        private string ChildItem { get; }
        public ItemsGenerator(IControl owner, AvaloniaProperty contentProperty, AvaloniaProperty contentTemplateProperty) : base(owner)
        {
            Contract.Requires<ArgumentNullException>(owner != null);
            Contract.Requires<ArgumentNullException>(contentProperty != null);
            ContentProperty = contentProperty;
            ContentTemplateProperty = contentTemplateProperty;
            ChildItem = $"{ owner.GetType().Name }Item";
        }
        public override Type ContainerType
        {
            get
            {
                if (ChildItem.Equals("ListViewItem"))
                    return typeof(ListViewItem);
                else
                    return typeof(GridViewItem);
            }
        }
        protected AvaloniaProperty ContentProperty { get; }
        protected AvaloniaProperty ContentTemplateProperty { get; }
        protected override IControl CreateContainer(object item)
        {
            if (item is ListViewItem container)
                return container;
            else
            {
                var result = ChildItem.Equals("ListViewItem") ? new ListViewItem() : new GridViewItem();
                if (ContentTemplateProperty != null)
                    result.SetValue(ContentTemplateProperty, ItemTemplate, BindingPriority.Style);
                result.SetValue(ContentProperty, item, BindingPriority.Style);
                if (item is not IControl)
                    result.DataContext = item;
                return result;
            }
        }
        public override bool TryRecycle(int oldIndex, int newIndex, object item)
        {
            var container = ContainerFromIndex(oldIndex);
            if (container == null)
                throw new IndexOutOfRangeException("Could not recycle container: not materialized.");
            container.SetValue(ContentProperty, item);
            if (item is not IControl)
                container.DataContext = item;
            var info = MoveContainer(oldIndex, newIndex, item);
            RaiseRecycled(new ItemContainerEventArgs(info));
            return true;
        }
    }
}
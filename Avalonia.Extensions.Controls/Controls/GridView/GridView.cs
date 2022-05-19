using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Extensions.Styles;
using Avalonia.Interactivity;
using Avalonia.Layout;
using PCLUntils.IEnumerables;

namespace Avalonia.Extensions.Controls
{
    /// <summary>
    /// the uwp like "GridView",you need to set "ColumnNum" for columns count
    /// </summary>
    public class GridView : ListView, IStyling
    {
        /// <summary>
        /// Defines the <see cref="ColumnNum"/> property.
        /// </summary>
        public static readonly StyledProperty<int> ColumnNumProperty =
          AvaloniaProperty.Register<GridView, int>(nameof(ColumnNum), 1);
        /// <summary>
        /// get or set column number.
        /// default value is 1.if the value smaller than 1 it's means depend layout by item controls
        /// </summary>
        public int ColumnNum
        {
            get => GetValue(ColumnNumProperty);
            set => SetValue(ColumnNumProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="ChildHorizontalContentAlignment"/> property.
        /// </summary>
        public static readonly StyledProperty<HorizontalAlignment> ChildHorizontalContentAlignmentProperty =
            AvaloniaProperty.Register<GridView, HorizontalAlignment>(nameof(ChildHorizontalContentAlignment));
        /// <summary>
        /// Gets or sets the horizontal alignment of the content within the control.
        /// </summary>
        public HorizontalAlignment ChildHorizontalContentAlignment
        {
            get => GetValue(ChildHorizontalContentAlignmentProperty);
            set => SetValue(ChildHorizontalContentAlignmentProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="ChildVerticalContentAlignment"/> property.
        /// </summary>
        public static readonly StyledProperty<VerticalAlignment> ChildVerticalContentAlignmentProperty =
            AvaloniaProperty.Register<GridView, VerticalAlignment>(nameof(ChildVerticalContentAlignment));
        /// <summary>
        /// Gets or sets the vertical alignment of the content within the control.
        /// </summary>
        public VerticalAlignment ChildVerticalContentAlignment
        {
            get => GetValue(ChildVerticalContentAlignmentProperty);
            set => SetValue(ChildVerticalContentAlignmentProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="HorizontalAlignment"/> property.
        /// </summary>
        public static readonly StyledProperty<HorizontalAlignment> ChildHorizontalAlignmentProperty =
            AvaloniaProperty.Register<GridView, HorizontalAlignment>(nameof(ChildHorizontalAlignment));
        /// <summary>
        /// Gets or sets the element's preferred horizontal alignment in its parent.
        /// </summary>
        public HorizontalAlignment ChildHorizontalAlignment
        {
            get => GetValue(HorizontalAlignmentProperty);
            set => SetValue(HorizontalAlignmentProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="VerticalAlignment"/> property.
        /// </summary>
        public static readonly StyledProperty<VerticalAlignment> ChildVerticalAlignmentProperty =
            AvaloniaProperty.Register<GridView, VerticalAlignment>(nameof(ChildVerticalAlignment));
        /// <summary>
        /// Gets or sets the element's preferred vertical alignment in its parent.
        /// </summary>
        public VerticalAlignment ChildVerticalAlignment
        {
            get => GetValue(VerticalAlignmentProperty);
            set => SetValue(VerticalAlignmentProperty, value);
        }
        /// <summary>
        /// create a instance
        /// </summary>
        public GridView()
        {
            this.AddStyles(ItemsPanelProperty, ColumnNum);
            ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Auto);
            ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Disabled);
            ColumnNumProperty.Changed.AddClassHandler<GridView>(OnColumnNumChanged);
            ChildVerticalAlignmentProperty.Changed.AddClassHandler<GridView>(OnChildVerticalAlignmentChange);
            ChildHorizontalAlignmentProperty.Changed.AddClassHandler<GridView>(OnChildHorizontalAlignmentChange);
            ChildVerticalContentAlignmentProperty.Changed.AddClassHandler<GridView>(OnChildVerticalContentAlignmentChange);
            ChildHorizontalContentAlignmentProperty.Changed.AddClassHandler<GridView>(OnChildHorizontalContentAlignmentChange);
        }
        private void OnColumnNumChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue())
                this.AddStyles(ItemsPanelProperty, ColumnNum);
        }
        private void OnChildVerticalAlignmentChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue())
            {
                for (var index = 0; index < LogicalChildren.Count; index++)
                {
                    if (LogicalChildren.ElementAt(index) is ListBoxItem listBoxItem)
                        listBoxItem.VerticalAlignment = ChildVerticalAlignment;
                }
            }
        }
        private void OnChildHorizontalAlignmentChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue())
            {
                for (var index = 0; index < LogicalChildren.Count; index++)
                {
                    if (LogicalChildren.ElementAt(index) is ListBoxItem listBoxItem)
                        listBoxItem.HorizontalAlignment = ChildHorizontalAlignment;
                }
            }
        }
        private void OnChildVerticalContentAlignmentChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue())
            {
                for (var index = 0; index < LogicalChildren.Count; index++)
                {
                    if (LogicalChildren.ElementAt(index) is ListBoxItem listBoxItem)
                        listBoxItem.VerticalContentAlignment = ChildVerticalContentAlignment;
                }
            }
        }
        private void OnChildHorizontalContentAlignmentChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue())
            {
                for (var index = 0; index < LogicalChildren.Count; index++)
                {
                    if (LogicalChildren.ElementAt(index) is ListBoxItem listBoxItem)
                        listBoxItem.HorizontalContentAlignment = ChildHorizontalContentAlignment;
                }
            }
        }
        private bool scrollTopEnable = false;
        protected override void ScrollEventHandle(ScrollViewer scrollViewer)
        {
            var anchorPoint = scrollViewer.Offset.Y;
            if (anchorPoint == 0 && scrollTopEnable)
            {
                scrollTopEnable = false;
                var args = new RoutedEventArgs(ScrollTopEvent);
                RaiseEvent(args);
                if (!args.Handled)
                    args.Handled = true;
            }
            else if (anchorPoint > 0)
            {
                scrollTopEnable = true;
                var scrollHeight = anchorPoint + scrollViewer.Viewport.Height;
                if (scrollHeight == scrollViewer.Extent.Height)
                {
                    var args = new RoutedEventArgs(ScrollEndEvent);
                    RaiseEvent(args);
                    if (!args.Handled)
                        args.Handled = true;
                }
            }
        }
    }
}
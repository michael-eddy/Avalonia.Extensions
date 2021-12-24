using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using System;
using System.Collections.Generic;

namespace Avalonia.Extensions.Controls
{
    public class CheckBoxList : UserControl
    {
        private ItemsRepeater repeater;
        private IEnumerable<GroupViewItem> _items;
        public CheckBoxList()
        {
            _items = new AvaloniaList<GroupViewItem>();
            ItemsProperty.Changed.AddClassHandler<CheckBoxList>(OnItemsChange);
            OrientationProperty.Changed.AddClassHandler<CheckBoxList>(OrOrientationChange);
        }
        private void OnItemsChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (repeater != null)
                repeater.Items = _items;
        }
        private void OrOrientationChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            DrawLayout();
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            repeater = new ItemsRepeater();
            repeater.Items = _items;
            repeater.ItemTemplate = new FuncDataTemplate<GroupViewItem>((x, _) =>
              new CheckBox
              {
                  [!ContentProperty] = new Binding("Content"),
                  [!ToggleButton.IsCheckedProperty] = new Binding(),
                  [!ToggleButton.IsCheckedProperty] = new Binding("Check"),
              });
            DrawLayout();
            Content = repeater;
        }
        /// <summary>
        ///  Gets or sets a uniform distance (in pixels) between stacked items. It is applied
        ///  in the direction of the StackLayout's Orientation.
        /// </summary>
        public double Spacing
        {
            get => GetValue(SpacingProperty);
            set
            {
                SetValue(SpacingProperty, value);
                DrawLayout();
            }
        }
        /// <summary>
        /// Defines the <see cref="Spacing"/> property.
        /// </summary>
        public static readonly StyledProperty<double> SpacingProperty =
          AvaloniaProperty.Register<CheckBoxList, double>(nameof(Spacing), 8);
        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<CheckBoxList, Orientation>(nameof(Orientation), Orientation.Horizontal);
        public IEnumerable<GroupViewItem> Items
        {
            get => _items;
            set => SetAndRaise(ItemsProperty, ref _items, value);
        }
        public static readonly DirectProperty<CheckBoxList, IEnumerable<GroupViewItem>> ItemsProperty =
            AvaloniaProperty.RegisterDirect<CheckBoxList, IEnumerable<GroupViewItem>>(nameof(Items), o => o.Items, (o, v) => o.Items = v);
        private void DrawLayout()
        {
            if (repeater != null)
            {
                repeater.Layout = new StackLayout
                {
                    Spacing = Spacing,
                    Orientation = Orientation
                };
            }
        }
    }
}
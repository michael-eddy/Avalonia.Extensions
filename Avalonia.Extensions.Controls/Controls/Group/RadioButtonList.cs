using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Avalonia.Extensions.Controls
{
    public class RadioButtonList : UserControl
    {
        private string GroupId { get; }
        private ItemsRepeater repeater;
        internal GroupCommand Command;
        private IEnumerable<GroupViewItem> _items;
        public RadioButtonList()
        {
            Command = new GroupCommand();
            Command.ExecuteCallback += Command_ExecuteCallback;
            GroupId = Guid.NewGuid().ToString("N");
            _items = new AvaloniaList<GroupViewItem>();
            ItemsProperty.Changed.AddClassHandler<RadioButtonList>(OnItemsChange);
            OrientationProperty.Changed.AddClassHandler<RadioButtonList>(OrOrientationChange);
        }
        private void Command_ExecuteCallback(object sender, EventArgs e)
        {

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
            var binding = new Binding();
            binding.Source = this;
            binding.Path = "Command";
            repeater.ItemTemplate = new FuncDataTemplate<GroupViewItem>((x, _) => new RadioButton
            {
                [!ContentProperty] = new Binding("Content"),
                [!ToggleButton.CommandProperty] = binding,
                [!ToggleButton.IsCheckedProperty] = new Binding("Check")
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
          AvaloniaProperty.Register<RadioButtonList, double>(nameof(Spacing), 8);
        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<RadioButtonList, Orientation>(nameof(Orientation), Orientation.Horizontal);
        public IEnumerable<GroupViewItem> Items
        {
            get => _items;
            set
            {
                if (value != null)
                {
                    foreach (var item in value)
                        item.Id = GroupId;
                }
                SetAndRaise(ItemsProperty, ref _items, value);
            }
        }
        public static readonly DirectProperty<RadioButtonList, IEnumerable<GroupViewItem>> ItemsProperty =
            AvaloniaProperty.RegisterDirect<RadioButtonList, IEnumerable<GroupViewItem>>(nameof(Items), o => o.Items, (o, v) => o.Items = v);
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
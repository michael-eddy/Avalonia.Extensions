using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Extensions.Event;
using Avalonia.Extensions.Model;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Threading;
using PCLUntils.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Controls
{
    public class PopupMenu : Window
    {
        private ListView ListBox { get; }
        /// <summary>
        /// Defines the <see cref="Items"/> property.
        /// </summary>
        public static readonly DirectProperty<PopupMenu, IList> ItemsProperty =
          AvaloniaProperty.RegisterDirect<PopupMenu, IList>(nameof(Items), o => o.Items, (o, v) => o.Items = v);
        /// <summary>
        /// Defines the <see cref="ItemTemplate"/> property.
        /// </summary>
        public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty =
            AvaloniaProperty.Register<PopupMenu, IDataTemplate>(nameof(ItemTemplate));
        /// <summary>
        /// Gets or sets the items to display.
        /// </summary>
        [Content]
        public IList Items
        {
            get => _items;
            set => SetAndRaise(ItemsProperty, ref _items, value);
        }
        /// <summary>
        /// Gets or sets the data template used to display the items in the control.
        /// </summary>
        public IDataTemplate ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }
        private IList _items;
        private bool _isFocus = true;
        public event EventHandler<ItemClickEventArgs> ItemClick;
        public PopupMenu()
        {
            Width = 80;
            Height = 60;
            Topmost = true;
            Focusable = true;
            ShowInTaskbar = false;
            ListBox = new ListView();
            ListBox.SizeChange += ListBox_SizeChange;
            SystemDecorations = SystemDecorations.None;
            ListBox.VirtualizationMode = ItemVirtualizationMode.None;
            ItemsProperty.Changed.AddClassHandler<PopupMenu>(OnItemsChange);
            ItemTemplateProperty.Changed.AddClassHandler<PopupMenu>(OnItemTemplateChanged);
        }
        private void ListBox_SizeChange(object sender, SizeRoutedEventArgs e)
        {
            Width = e.Width;
        }
        private void OnItemTemplateChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is IDataTemplate template)
                ListBox.ItemTemplate = template;
        }
        private void OnItemsChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is IList<string> arrayString)
            {
                ListBox.Items = arrayString.Select(x => new BindingModel(x)).ToList();
                ItemTemplate = new FuncDataTemplate<BindingModel>((x, _) => new TextBlock { [!TextBlock.TextProperty] = new Binding("Content") });
            }
            else if (e.NewValue is IList array)
                ListBox.Items = array;
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            ListBox.SelectionChanged += ListBox_SelectionChanged;
            Content = ListBox;
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is ListBox listBox)
            {
                try
                {
                    var item = listBox.SelectedItem;
                    var index = listBox.SelectedIndex;
                    var args = new ItemClickEventArgs(item, index);
                    ItemClick?.Invoke(this, args);
                }
                finally
                {
                    Close();
                }
            }
        }
        protected override void OnPointerEnter(PointerEventArgs e)
        {
            _isFocus = true;
            base.OnPointerEnter(e);
        }
        protected override void OnPointerLeave(PointerEventArgs e)
        {
            _isFocus = false;
            base.OnPointerLeave(e);
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (!_isFocus)
                    Close();
                else
                {
                    _isFocus = false;
                    await Task.Delay(200);
                    FocusManager.Instance.Focus(this);
                }
            });
        }
        public void Show(IControl control)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if ((control.TransformedBounds as dynamic).Clip is Rect rect)
                {
                    var window = control.GetWindow(out bool hasBase);
                    if (!hasBase)
                    {
                        int x = (rect.X + window.Position.X).ToInt32(),
                            y = (rect.Y + window.Position.Y).ToInt32();
                        Position = new PixelPoint(x, y);
                    }
                    else if (window is WindowBase windowBase)
                    {
                        int x = windowBase.MousePisition.X,
                            y = windowBase.MousePisition.Y;
                        Position = new PixelPoint(x, y);
                    }
                }
            });
            base.Show();
        }
        public new void Close()
        {
            ListBox.SelectionChanged -= ListBox_SelectionChanged;
            base.Close();
        }
    }
}
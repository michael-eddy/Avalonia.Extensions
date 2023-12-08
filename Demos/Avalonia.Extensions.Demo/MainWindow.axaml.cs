using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Event;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;

namespace Avalonia.Controls.Demo
{
    public partial class MainWindow : AeroWindow
    {
        private ObservableCollection<object> Collection { get; set; }
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            InitializeComponent();
            Locate = true;
            Width = 800;
            Height = 600;
            DataContext = new MainViewModel();
            this.AttachDevTools();
            Loaded += OnLoaded;
        }
        protected override bool MoveDragEnable => true;
        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            listView.ScrollTop += ListView_ScrollTop;
            listView.ScrollEnd += ListView_ScrollEnd;
            splitListView.ScrollTop += ListView_ScrollTop;
            splitListView.ScrollEnd += ListView_ScrollEnd;
            splitListView.ItemClick += SplitListView_ItemRightClick;
            btnStart.Click += BtnStart_Click;
            btnShow.Click += BtnShow_Click;
            btnShow2.Click += BtnShow_Click;
            Collection = new ObservableCollection<object>
            {
                new { Url = "http://s1.hdslb.com/bfs/static/passport/static/img/rl_top.35edfde.png" },
                new { Url = "https://i0.hdslb.com/bfs/live/c8e6d780a3182c37a96e79f4ed26fcb576f2520a.png" },
                new { Url = "http://s1.hdslb.com/bfs/static/passport/static/img/rl_top.35edfde.png" },
                new { Url = "https://i0.hdslb.com/bfs/live/c8e6d780a3182c37a96e79f4ed26fcb576f2520a.png" },
                new { Url = "http://s1.hdslb.com/bfs/static/passport/static/img/rl_top.35edfde.png" },
                new { Url = "https://i0.hdslb.com/bfs/live/c8e6d780a3182c37a96e79f4ed26fcb576f2520a.png" },
                new { Url = "http://s1.hdslb.com/bfs/static/passport/static/img/rl_top.35edfde.png" },
                new { Url = "https://i0.hdslb.com/bfs/live/c8e6d780a3182c37a96e79f4ed26fcb576f2520a.png" },
                new { Url = "http://s1.hdslb.com/bfs/static/passport/static/img/rl_top.35edfde.png" },
                new { Url = "https://i0.hdslb.com/bfs/live/c8e6d780a3182c37a96e79f4ed26fcb576f2520a.png" },
                new { Url = "http://s1.hdslb.com/bfs/static/passport/static/img/rl_top.35edfde.png" },
                new { Url = "https://i0.hdslb.com/bfs/live/c8e6d780a3182c37a96e79f4ed26fcb576f2520a.png" }
            };
            imgList.ItemsSource = Collection;
            scrollView.ScrollEnd += ScrollView_ScrollEnd;
            scrollView.ScrollTop += ScrollView_ScrollTop;
            //playerView.Play("http://vfx.mtime.cn/Video/2019/03/18/mp4/190318231014076505.mp4");
            //audio.Play("http://downsc.chinaz.net/Files/DownLoad/sound1/201906/11582.mp3");
            //webView.Navigate("bing.com");
        }
        private void ListView_ScrollTop(object? sender, RoutedEventArgs e)
        {
            MessageBox.Show("提示", "到顶了");
        }
        private void ListView_ScrollEnd(object? sender, RoutedEventArgs e)
        {
            MessageBox.Show("提示", "到底了");
        }
        private void BtnShow_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button control)
            {
                switch (control.Name)
                {
                    case "btnShow":
                        {
                            PopupMenu popupMenu = new PopupMenu{ Opacity = 0 };
                            popupMenu.Items = new[] { "1234", "1234", "1234", "1234" };
                            popupMenu.ItemClick += PopupMenu_ItemClick;
                            popupMenu.Show(control);
                            break;
                        }
                    case "btnShow2":
                        {
                            PopupMenu popupMenu = new PopupMenu{ Opacity = 0 };
                            popupMenu.Items = new[] { new CustomBindingModel("1234"), new CustomBindingModel("1234"),
                                    new CustomBindingModel("1234"), new CustomBindingModel("1234") };
                            popupMenu.ItemTemplate = new FuncDataTemplate<CustomBindingModel>((x, _) => new TextBlock { [!TextBlock.TextProperty] = new Binding("Content") });
                            popupMenu.ItemClick += PopupMenu_ItemClick;
                            popupMenu.Show(control);
                            break;
                        }
                }
            }
        }
        private void PopupMenu_ItemClick(object? sender, ItemClickEventArgs e)
        {
            MessageBox.Show("tips", "PopupMenu -> " + e.Item.ToString());
        }
        private void BtnStart_Click(object? sender, RoutedEventArgs e)
        {
            progressRing.IsActive = !progressRing.IsActive;
        }
        private void SplitListView_ItemRightClick(object? sender, ViewRoutedEventArgs e)
        {
            var content = e.ClickItem.ToString();
            if (e.ClickMouse == MouseButton.Right)
                MessageBox.Show("tips", "SplitListView -> Right Click : " + content);
            else
                MessageBox.Show("tips", "SplitListView -> Left Click : " + content);
        }
        private void ScrollView_ScrollTop(object? sender, RoutedEventArgs e)
        {
            MessageBox.Show("tips", "ScrollView -> Scroll Top");
        }
        private void ScrollView_ScrollEnd(object? sender, RoutedEventArgs e)
        {
            MessageBox.Show("tips", "ScrollView -> Scroll End");
        }
        private void OnNotifyPopupClick(object sender, RoutedEventArgs e)
        {
            this.ShowToast("大大大大大大大大大大大大大大大大大大");
            PopupToast.Show("大大大大大大大大大大大大大大大大大大");
        }
        private void OnNotifyClick(object sender, RoutedEventArgs e)
        {
            var text = ((Button)sender).CommandParameter?.ToInt32();
            ShowPosition position = ShowPosition.BottomLeft;
            ScollOrientation orientation = ScollOrientation.Horizontal;
            switch (text)
            {
                case 0:
                    {
                        notifyIcon.Add();
                        break;
                    }
                case 1:
                    {
                        position = ShowPosition.TopLeft;
                        orientation = ScollOrientation.Horizontal;
                        break;
                    }
                case 2:
                    {
                        position = ShowPosition.TopRight;
                        orientation = ScollOrientation.Horizontal;
                        break;
                    }
                case 3:
                    {
                        position = ShowPosition.BottomLeft;
                        orientation = ScollOrientation.Horizontal;
                        break;
                    }
                case 4:
                    {
                        position = ShowPosition.BottomRight;
                        orientation = ScollOrientation.Horizontal;
                        break;
                    }
                case 5:
                    {
                        position = ShowPosition.BottomLeft;
                        orientation = ScollOrientation.Vertical;
                        break;
                    }
                case 6:
                    {
                        position = ShowPosition.BottomRight;
                        orientation = ScollOrientation.Vertical;
                        break;
                    }
            }
            NotifyWindow window = new NotifyWindow{ Opacity = 0 };
            window.Content = new TextBlock { Text = "大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大大" };
            var options = new NotifyOptions(position, new Size(160, 60), orientation);
            window.Show(options);
        }
    }
}
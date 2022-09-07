using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Danmaku;
using Avalonia.Markup.Xaml;
using System;

namespace Avalonia.Danmaku.Demo
{
    public class MainWindow : AeroWindow
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            Width = 800;
            InitializeComponent();
            this.AttachDevTools();
        }
        private void InitializeComponent()
        {
            var uri = new Uri("file:///D:\\200887808.xml");
            var danmaku = this.FindControl<DanmakuView>("danmaku");
            danmaku.Load(uri);
            var danmakuView = this.FindControl<DanmakuNativeView>("danmakuView");
            danmakuView.LoadFile(uri);
            danmakuView.Start();
        }
    }
}
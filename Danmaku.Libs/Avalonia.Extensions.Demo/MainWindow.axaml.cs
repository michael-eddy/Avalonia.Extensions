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
            var danmaku = this.FindControl<DanmakuView>("danmaku");
            danmaku.Load("D:\\200887808.xml");
            var danmakuView = this.FindControl<DanmakuNativeView>("danmakuView");
            danmakuView.LoadFile(new Uri("file:///D:\\200887808.xml"));
            danmakuView.Start();
        }
    }
}
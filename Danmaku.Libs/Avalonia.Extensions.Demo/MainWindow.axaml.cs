using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Danmaku;
using Avalonia.Markup.Xaml;

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
            danmaku.Load("200887808.xml");
        }
    }
}
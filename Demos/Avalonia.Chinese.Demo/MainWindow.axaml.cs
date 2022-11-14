using Avalonia.Extensions.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.Extensions.Chinese.Demo
{
    public class MainWindow : AeroWindow
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            Width = 800;
            Height = 600;
        }
    }
}
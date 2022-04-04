using System.Windows;

namespace Danmaku.Wpf
{
    public partial class DanmakuView : Window
    {
        public DanmakuView()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var instance = LibLoader.WTF_CreateInstance();

        }
    }
}
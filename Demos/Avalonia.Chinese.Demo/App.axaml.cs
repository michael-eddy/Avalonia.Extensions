using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Extensions.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.Extensions.Chinese.Demo
{
    public class App : ApplicationBase
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow();
            base.OnFrameworkInitializationCompleted();
        }
    }
}
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Extensions.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Reactive;

namespace Avalonia.Controls.Demo
{
    public class App : ApplicationBase
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ExceptionHandler);
        }
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow();
            Console.WriteLine(FontManager.Current.DefaultFontFamilyName);
            Console.WriteLine(FontManager.Current.PlatformImpl.GetDefaultFontFamilyName());
            Console.WriteLine(string.Join(';', FontManager.Current.PlatformImpl.GetInstalledFontFamilyNames()));
            base.OnFrameworkInitializationCompleted();
        }
        private void ExceptionHandler(Exception exception)
        {

        }
    }
}
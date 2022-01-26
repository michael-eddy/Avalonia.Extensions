using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Extensions.Controls;
using Avalonia.Markup.Xaml;
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
            base.OnFrameworkInitializationCompleted();
        }
        private void ExceptionHandler(Exception exception)
        {

        }
    }
}
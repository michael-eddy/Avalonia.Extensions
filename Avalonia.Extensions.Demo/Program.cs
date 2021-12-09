using Avalonia.Extensions.Controls;
using Avalonia.Threading;
using ReactiveUI;

namespace Avalonia.Controls.Demo
{
    class Program
    {
        public static void Main(string[] args)
        {
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
            AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .UseDoveExtensions()
                    .UseChineseInputSupport()
                    .LogToTrace()
            .StartWithClassicDesktopLifetime(args);
        }
    }
}
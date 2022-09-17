using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Media;
using Avalonia.Threading;
using ReactiveUI;

namespace Avalonia.Danmaku.Demo
{
    class Program
    {
        public static void Main(string[] args)
        {
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
            AppBuilder.Configure<App>()
                    .LogToTrace()
                    .UsePlatformDetect()
                    .UseDoveExtensions()
            .StartWithClassicDesktopLifetime(args);
        }
    }
}
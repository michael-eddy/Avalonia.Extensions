using Avalonia.Extensions.Controls;
using Avalonia.Extensions.WebView;
using Avalonia.Extensions.Media;
using Avalonia.Threading;
using ReactiveUI;
using System;

namespace Avalonia.Controls.Demo
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
            AppBuilder.Configure<App>()
                    .LogToTrace()
                    .UsePlatformDetect()
                    .UseDoveExtensions()
                    .UseChineseInputSupport()
                    //.UseWebView(true)
                    .UseVideoView().UseAudioControl()
            .StartWithClassicDesktopLifetime(args);
        }
    }
}
using Avalonia.Extensions.Controls;

namespace Avalonia.Extensions.Chinese.Demo
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AppBuilder.Configure<App>()
                    .LogToTrace()
                    .UsePlatformDetect()
                    .UseChineseInputSupport()
            .StartWithClassicDesktopLifetime(args);
        }
    }
}
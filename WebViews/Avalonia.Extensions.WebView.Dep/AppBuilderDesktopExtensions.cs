using Avalonia.Controls;
using Avalonia.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.WebView
{
    public static class AppBuilderDesktopExtensions
    {
        public static TAppBuilder InitCefDep<TAppBuilder>(this TAppBuilder builder)
           where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup((_) =>
            {
                try
                {
                    string projectPath = Path.GetDirectoryName(typeof(Application).Assembly.Location);
                    var cefPath = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
                    Path.Combine(projectPath, "Contents", "Frameworks", "Chromium Embedded Framework.framework") : Path.Combine(projectPath, "cef");
                    if (!Directory.Exists(cefPath))
                        DownloadTask.Instance.Start();
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
                    throw;
                }
            });
            return builder;
        }
    }
}
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using CefNet;
using System;

namespace Avalonia.Extensions.WebView
{
    public static class AppBuilderDesktopExtensions
    {
        internal static CefNetApplication CefApp { get; private set; }
        public static TAppBuilder UseWebView<TAppBuilder>(this TAppBuilder builder, string cefPath, CefSettings cefSettings)
           where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup((_) =>
            {
                try
                {
                    CefApp = new CefNetApplication();
                    CefApp.Initialize(cefPath, cefSettings);
                    if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        desktop.Exit += Desktop_Exit;
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
                    throw;
                }
            });
            return builder;
        }
        private static void Desktop_Exit(object sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            try
            {
                CefApp.Shutdown();
                CefApp.Dispose();
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(sender, ex.Message);
            }
        }
    }
}
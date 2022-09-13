using Avalonia.Controls;
using Avalonia.Extensions.WebView.Dep;
using Avalonia.Logging;
using CefNet;
using System;
using System.IO;

namespace Avalonia.Extensions.Controls
{
    public static class AppBuilderDesktopExtensions
    {
        /// <summary>
        /// initializing webview control
        /// </summary>
        /// <typeparam name="TAppBuilder"></typeparam>
        /// <param name="builder"></param>
        /// <param name="initAuto">automatically initializing dependencies</param>
        /// <param name="eventHandler">Occurs when a new message is received from a different process. Do not keep a reference to or attempt to access the message outside of an event handler.</param>
        /// <returns></returns>
        public static TAppBuilder UseWebView<TAppBuilder>(this TAppBuilder builder, bool initAuto = true, EventHandler<CefProcessMessageReceivedEventArgs> eventHandler = null)
           where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup((_) =>
            {
                try
                {
                    string projectPath = Path.GetDirectoryName(typeof(Application).Assembly.Location);
                    var cefPath = PlatformInfo.IsMacOS ? Path.Combine(projectPath, "Contents", "Frameworks", "Chromium Embedded Framework.framework") : Path.Combine(projectPath, "cef");
                    if (!Directory.Exists(cefPath) && initAuto)
                        DownloadTask.Instance.Start();
                    var settings = new CefSettings();
                    settings.NoSandbox = true;
                    settings.ExternalMessagePump = true;
                    settings.UncaughtExceptionStackSize = 8;
                    settings.MultiThreadedMessageLoop = false;
                    settings.WindowlessRenderingEnabled = true;
                    settings.LogSeverity = CefLogSeverity.Warning;
                    settings.ResourcesDirPath = Path.Combine(cefPath, "Resources");
                    settings.LocalesDirPath = Path.Combine(cefPath, "Resources", "locales");
                    var app = new CefNetApplication();
                    if (eventHandler != null)
                        app.CefProcessMessageReceived += eventHandler;
                    app.Initialize(PlatformInfo.IsMacOS ? cefPath : Path.Combine(cefPath, "Release"), settings);
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
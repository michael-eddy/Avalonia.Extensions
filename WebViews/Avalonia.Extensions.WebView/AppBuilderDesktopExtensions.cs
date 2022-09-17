using Avalonia.Controls;
using Avalonia.Logging;
using CefNet;
using CefNet.Input;
using System;
using System.IO;

namespace Avalonia.Extensions.WebView
{
    public static class AppBuilderDesktopExtensions
    {
        /// <summary>
        /// initializing webview control
        /// </summary>
        /// <typeparam name="TAppBuilder"></typeparam>
        /// <param name="builder"></param>
        /// <param name="eventHandler">Occurs when a new message is received from a different process. Do not keep a reference to or attempt to access the message outside of an event handler.</param>
        /// <returns></returns>
        public static TAppBuilder UseWebView<TAppBuilder>(this TAppBuilder builder, EventHandler<CefProcessMessageReceivedEventArgs> eventHandler = null)
           where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            try
            {
                string projectPath = Path.GetDirectoryName(typeof(Application).Assembly.Location);
                var cefPath = PlatformInfo.IsMacOS ? Path.Combine(projectPath, "Contents", "Frameworks", "Chromium Embedded Framework.framework") : Path.Combine(projectPath, "cef");
                var settings = new CefSettings();
                settings.NoSandbox = true;
                settings.UncaughtExceptionStackSize = 8;
                settings.WindowlessRenderingEnabled = true;
                settings.LogSeverity = CefLogSeverity.Warning;
                var externalMessagePump = PlatformInfo.IsMacOS;
                settings.LogFile = Path.Combine(projectPath, "cef.log");
                settings.ExternalMessagePump = externalMessagePump;
                settings.MultiThreadedMessageLoop = !externalMessagePump;
                settings.ResourcesDirPath = Path.Combine(cefPath, "Resources");
                settings.LocalesDirPath = Path.Combine(cefPath, "Resources", "locales");
                var app = new CefNetApplication();
                if (eventHandler != null)
                    app.CefProcessMessageReceived += eventHandler;
                app.Initialize(PlatformInfo.IsMacOS ? cefPath : Path.Combine(cefPath, "Release"), settings);
                KeycodeConverter.Default = new FixChineseInptKeycodeConverter();
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
                throw;
            }
            return builder;
        }
    }
}
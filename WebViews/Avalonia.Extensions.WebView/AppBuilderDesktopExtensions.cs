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
        public static TAppBuilder UseWebView<TAppBuilder>(this TAppBuilder builder, bool autoDownload = false, EventHandler<CefProcessMessageReceivedEventArgs> eventHandler = null)
           where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            try
            {
                string projectPath = Path.GetDirectoryName(typeof(Application).Assembly.Location);
                var cefPath = PlatformInfo.IsMacOS ? Path.Combine(projectPath, "Contents", "Frameworks", "Chromium Embedded Framework.framework") : Path.Combine(projectPath, "cef");
                if (!Directory.Exists(cefPath) && autoDownload)
                    DownloadTask.Instance.Start();
                var app = new CefApplication();
                if (eventHandler != null)
                    app.CefProcessMessageReceived += eventHandler;
                app.Initialize(cefPath, projectPath);
                KeycodeConverter.Default = new FixChineseInptKeycodeConverter();
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
                throw;
            }
            return builder;
        }
        public static TAppBuilder UseWebView<TAppBuilder>(this TAppBuilder builder, string cefPath, CefSettings cefSettings, EventHandler<CefProcessMessageReceivedEventArgs> eventHandler = null)
                       where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            try
            {
                var app = new CefApplication();
                if (eventHandler != null)
                    app.CefProcessMessageReceived += eventHandler;
                app.Initialize(cefPath, cefSettings);
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
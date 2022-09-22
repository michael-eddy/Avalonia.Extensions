using Avalonia.Logging;
using CefNet;
using System;
using System.IO;

namespace Avalonia.Extensions.WebView
{
    public sealed class CefApplication : CefNetApplication
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            base.OnBeforeCommandLineProcessing(processType, commandLine);
            commandLine.AppendSwitchWithValue("disable-gpu", "1");
            commandLine.AppendSwitchWithValue("disable-gpu-compositing", "1");
            commandLine.AppendSwitchWithValue("disable-gpu-vsync", "1");
            commandLine.AppendSwitchWithValue("disable-gpu-shader-disk-cache", "1");
        }
        public void Initialize(string cefPath, string projectPath)
        {
            try
            {
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
                Initialize(PlatformInfo.IsMacOS ? cefPath : Path.Combine(cefPath, "Release"), settings);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
    }
}
using Avalonia.Logging;
using Avalonia.Platform;
using System;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Controls
{
    public abstract class ApplicationBase : Application
    {
        public override void RegisterServices()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    AvaloniaLocator.CurrentMutable.Bind<IFontManagerImpl>().ToConstant(new FontManagerImpl());
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                }
            }
            base.RegisterServices();
        }
    }
}
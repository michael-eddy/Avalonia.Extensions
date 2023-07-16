using Avalonia.Logging;
using Avalonia.Platform;
using System;

namespace Avalonia.Extensions.Controls
{
    public abstract class ApplicationBase : Application
    {
        public override void RegisterServices()
        {
            Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, "RegisterServices");
            try
            {
                AvaloniaLocator.CurrentMutable.Bind<IFontManagerImpl>().ToConstant(new FontManagerImpl());
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, "RegisterServices:" + ex.Message);
            }
            base.RegisterServices();
        }
    }
}
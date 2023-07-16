using Avalonia.Logging;
using System;

namespace Avalonia.Extensions.Controls
{
    public static class AppBuilderDesktopExtensions
    {
        /// <summary>
        /// set chinese support fontfamily for controls
        /// </summary>
        public static AppBuilder UseChineseInputSupport(this AppBuilder builder)
        {
            try
            {
                builder.ConfigureFonts(fontManager => fontManager.AddFontCollection(new WqyFontCollection()));
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, "UseChineseInputSupport Error:" + ex.Message);
            }
            return builder;
        }
    }
}
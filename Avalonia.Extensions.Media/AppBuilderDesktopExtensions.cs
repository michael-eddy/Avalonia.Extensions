using Avalonia.Controls;
using Avalonia.Logging;
using System;

namespace Avalonia.Extensions.Controls
{
    public static class AppBuilderDesktopExtensions
    {
        public static TAppBuilder UseMedia<TAppBuilder>(this TAppBuilder builder, string? libvlcDirectoryPath = null)
           where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup((_) =>
            {
                try
                {
                    LibVLCSharp.Shared.Core.Initialize(libvlcDirectoryPath);
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
                }
            });
            return builder;
        }
    }
}
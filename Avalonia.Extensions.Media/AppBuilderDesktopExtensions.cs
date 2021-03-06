using Avalonia.Controls;
using Avalonia.Logging;
using ManagedBass;
using System;

namespace Avalonia.Extensions.Media
{
    public static class AppBuilderDesktopExtensions
    {
        internal static bool IsAudioInit { get; private set; } = false;
        internal static bool IsVideoInit { get; private set; } = false;
        public static TAppBuilder UseAudioControl<TAppBuilder>(this TAppBuilder builder)
           where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup((_) =>
            {
                try
                {
                    if (!Bass.Init())
                    {
                        IsAudioInit = false;
                        string msg = "cannot initialize device";
                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, msg);
                        throw new InvalidOperationException(msg);
                    }
                    else
                        IsAudioInit = true;
                }
                catch (Exception ex)
                {
                    IsAudioInit = false;
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
                    throw;
                }
            });
            return builder;
        }
        public static TAppBuilder UseVideoView<TAppBuilder>(this TAppBuilder builder, string? libvlcDirectoryPath = null)
           where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup((_) =>
            {
                try
                {
                    LibVLCSharp.Shared.Core.Initialize(libvlcDirectoryPath);
                    IsVideoInit = true;
                }
                catch (Exception ex)
                {
                    IsVideoInit = false;
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
                    throw;
                }
            });
            return builder;
        }
    }
}
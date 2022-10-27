using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Markup.Xaml.Styling;
using ManagedBass;
using PCLUntils;
using PCLUntils.Objects;
using PCLUntils.Plantform;
using System;
using System.IO;

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
                    if (InitDll(builder) && !Bass.Init())
                    {
                        IsAudioInit = false;
                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, "BASS : cannot initialize device");
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
                    InitXamlStyle(builder);
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
        private static void InitXamlStyle(object builder)
        {
            try
            {
                StyleInclude styleInclude = new StyleInclude(new Uri("avares://Avalonia.Extensions.Media/Styles"));
                styleInclude.Source = new Uri($"avares://Avalonia.Extensions.Media/Styles/Xaml/FFmpegView.xaml");
                Application.Current.Styles.Add(styleInclude);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
            }
        }
        private static bool InitDll(object builder)
        {
            bool canInit = true;
            try
            {
                string sourceFileName = string.Empty, dllPath = string.Empty;
                switch (PlantformUntils.System)
                {
                    case Platforms.Linux:
                        {
                            dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libbass.so");
                            if (!File.Exists(dllPath))
                            {
                                var platform = $"linux-{PlantformUntils.ArchitectureString}";
                                sourceFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libBass", platform, "libbass.so");
                            }
                            break;
                        }
                    case Platforms.MacOS:
                        {
                            dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libbass.dylib");
                            if (!File.Exists(dllPath))
                                sourceFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libBass", "osx", "libbass.dylib");
                            break;
                        }
                    case Platforms.Windows:
                        {
                            dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bass.dll");
                            if (!File.Exists(dllPath))
                            {
                                var platform = $"win-{PlantformUntils.ArchitectureString}";
                                if (platform.Equals("win-arm", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    canInit = false;
                                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, "Bass cannot run in win-arm platform.Stop init.");
                                }
                                else
                                    sourceFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libBass", platform, "bass.dll");
                            }
                            break;
                        }
                }
                if (sourceFileName.IsNotEmpty() && canInit)
                {
                    if (File.Exists(sourceFileName))
                        File.Copy(sourceFileName, dllPath, true);
                    else
                        canInit = false;
                }
            }
            catch (Exception ex)
            {
                canInit = false;
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
            }
            return canInit;
        }
    }
}
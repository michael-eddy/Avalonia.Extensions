using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Markup.Xaml.Styling;
using FFmpeg.AutoGen;
using ManagedBass;
using PCLUntils;
using PCLUntils.Objects;
using PCLUntils.Plantform;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Media
{
    public static class AppBuilderDesktopExtensions
    {
        internal static bool IsAudioInit { get; private set; } = false;
        internal static bool IsVideoInit { get; private set; } = false;
        internal static bool IsFFmpegInit { get; private set; } = false;
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
                }
            });
            return builder;
        }
        public static unsafe TAppBuilder UseFFmpeg<TAppBuilder>(this TAppBuilder builder, string? libffmpegDirectoryPath = null)
           where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup((_) =>
            {
                try
                {
                    if (libffmpegDirectoryPath.IsEmpty())
                    {
                        var platform = string.Empty;
                        switch (PlantformUntils.System)
                        {
                            case Platforms.Linux:
                                platform = $"linux-{PlantformUntils.ArchitectureString}";
                                break;
                            case Platforms.MacOS:
                                platform = $"osx-{PlantformUntils.ArchitectureString}";
                                break;
                            case Platforms.Windows:
                                platform = PlantformUntils.IsArmArchitecture ? "win-arm64" : "win-x86";
                                break;
                        }
                        libffmpegDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libffmpeg", platform);
                    }
                    if (Directory.Exists(libffmpegDirectoryPath))
                    {
                        Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(builder, $"FFmpeg binaries found in: {libffmpegDirectoryPath}");
                        ffmpeg.RootPath = libffmpegDirectoryPath;
                        ffmpeg.avdevice_register_all();
                        ffmpeg.avformat_network_init();
                        ffmpeg.av_log_set_level(ffmpeg.AV_LOG_VERBOSE);
                        av_log_set_callback_callback logCallback = (p0, level, format, vl) =>
                        {
                            if (level > ffmpeg.av_log_get_level()) return;
                            var lineSize = 1024;
                            var lineBuffer = stackalloc byte[lineSize];
                            var printPrefix = 1;
                            ffmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
                            var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);
                            Console.Write(line);
                        };
                        ffmpeg.av_log_set_callback(logCallback);
                        IsFFmpegInit = true;
                    }
                    else
                    {
                        IsFFmpegInit = false;
                        Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(builder, $"cannot found FFmpeg binaries from path:\"{libffmpegDirectoryPath}\"");
                    }
                }
                catch (Exception ex)
                {
                    IsFFmpegInit = false;
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
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
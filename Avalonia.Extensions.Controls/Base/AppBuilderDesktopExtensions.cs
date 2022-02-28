using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Controls
{
    public static class AppBuilderDesktopExtensions
    {
        public static TAppBuilder UseDoveExtensions<TAppBuilder>(this TAppBuilder builder) where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup((_) =>
            {
                ApplyIcon(builder);
                Core.Instance.Init();
                try
                {
                    AvaloniaLocator.CurrentMutable.GetService<IAssetLoader>().SetDefaultAssembly(typeof(Core).Assembly);
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(builder, ex.Message);
                }
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        try
                        {
                            while (desktop.MainWindow != null)
                            {
                                Task.Delay(80).GetAwaiter().GetResult();
                                desktop.MainWindow.Closed += MainWindow_Closed;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
                        }
                    });
                }
            });
            return builder;
        }
        private static void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    if (desktop.Windows.Count(win => win is MessageBox || win is PopupMenu || win is PopupToast || win is NotifyWindow) == desktop.Windows.Count)
                        desktop.Shutdown();
                }
                Core.Instance.Dispose();
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(sender, ex.Message);
            }
        }
        private static void ApplyIcon(object builder)
        {
            try
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var style = new Style();
                    style.Selector = default(Selector).OfType(typeof(SymbolIcon));
                    style.Setters.Add(new Setter(TemplatedControl.FontFamilyProperty,
                        new FontFamily("avares://Avalonia.Extensions.Controls/Assets/Fonts#SegMDL2")));
                    Application.Current.Styles.Add(style);
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, ex.Message);
            }
        }
    }
}
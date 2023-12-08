using Avalonia.Controls;
using Avalonia.Extensions.Model;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using PCLUntils;
using PCLUntils.Assemblly;
using PCLUntils.IEnumerables;
using PCLUntils.Plantform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Avalonia.Extensions.Controls
{
    public static class ControlUtils
    {
        public static IntPtr GetHwnd(this Window window)
        {
            IntPtr hwnd = IntPtr.Zero;
            try
            {
                var platformHandle = ((IWindowImpl)((TopLevel)window.GetVisualRoot())?.PlatformImpl).GetPrivateProperty<IPlatformHandle>("Handle");
                hwnd = platformHandle?.Handle ?? IntPtr.Zero;
            }
            catch { }
            return hwnd;
        }
        public static Size GetScreenSize(this Window window, ScreenType type)
        {
            Size rect = default;
            try
            {
                switch (PlantformUntils.System)
                {
                    case Platforms.Windows:
                        {
                            var monitor = Base.Win32API.MonitorFromWindow(window.GetHwnd(), Base.Win32API.MONITOR_DEFAULT_TONEAREST);
                            if (monitor != IntPtr.Zero)
                            {
                                var monitorInfo = new NativeMonitorInfo();
                                Base.Win32API.GetMonitorInfo(monitor, monitorInfo);
                                if (type == ScreenType.WorkArea)
                                    rect = new Size(monitorInfo.Work.Right - monitorInfo.Work.Left, monitorInfo.Work.Bottom - monitorInfo.Work.Top);
                                else
                                    rect = new Size(monitorInfo.Monitor.Right - monitorInfo.Monitor.Left, monitorInfo.Monitor.Bottom - monitorInfo.Monitor.Top);
                            }
                            break;
                        }
                    default:
                        {
                            PixelRect pixelRect = default;
                            if (type == ScreenType.WorkArea)
                                pixelRect = window.Screens.Primary.WorkingArea;
                            else
                                pixelRect = window.Screens.Primary.Bounds;
                            rect = new Size(pixelRect.Width, pixelRect.Height);
                            break;
                        }
                }
            }
            catch { }
            return rect;
        }
        public static bool IsSameValue(this AvaloniaPropertyChangedEventArgs args) => args?.OldValue == args?.NewValue;
        public static void ApplyTheme(this StyledElement element, Uri sourceUri)
        {
            if (!Core.Instance.InnerClasses.Contains(sourceUri))
                return;
            try
            {
                using var stream = AssetLoader.Open(sourceUri);
                var bytes = new byte[stream.Length];
                stream.Read(bytes);
                var xaml = Encoding.UTF8.GetString(bytes);
                var styles = AvaloniaRuntimeXamlLoader.Parse<Styling.Styles>(xaml);
                element.UpdateStyles(styles);
                bytes = null;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(element, ex.Message);
            }
        }
        internal static void UpdateStyles(this StyledElement element, Styling.Styles styles)
        {
            if (styles != null && element != null)
            {
                for (var idx = 0; idx < styles.Count; idx++)
                {
                    if (styles.ElementAt(idx) is Style style && style.Setters != null && style.Setters.Count > 0)
                    {
                        foreach (var setter in style.Setters.Cast<Setter>())
                        {
                            try
                            {
                                element.SetValue(setter.Property, setter.Value);
                            }
                            catch { }
                        }
                    }
                }
            }
        }
        public static void ShowToast(this Window window, string content) => PopupToast.Show(content, window);
        public static Size MeasureString(this string text, double fontSize, SKTypeface typeface)
        {
            try
            {
                using SKPaint paint = new SKPaint();
                paint.Typeface = typeface;
                paint.Style = SKPaintStyle.Fill;
                paint.TextSize = Convert.ToSingle(fontSize);
                SKRect result = new SKRect();
                paint.MeasureText(text, ref result);
                var width = Convert.ToSingle(Math.Ceiling(result.Size.Width));
                var height = Convert.ToSingle(Math.Ceiling(result.Size.Height));
                return new Size(width, height);
            }
            catch { }
            return new Size();
        }
        public static IEnumerable<T> FindControls<T>(this Panel control, bool isLoop = false) where T : Control
        {
            Contract.Requires<ArgumentNullException>(control != null);
            foreach (var childControl in control.Children)
            {
                if (childControl is T obj)
                    yield return obj;
                if (childControl is Panel panel)
                {
                    var array = panel.FindControls<T>(isLoop);
                    foreach (var item in array)
                        yield return item;
                }
            }
        }
        public static void AddStyles(this StyledElement styling, AvaloniaProperty avaloniaProperty, params object[] parms)
        {
            try
            {
                if (styling is Control control)
                {
                    string typeName = control.GetType().Name;
                    var sourceUri = new Uri($"avares://Avalonia.Extensions.Controls/Styles/Xaml/{typeName}.xml");
                    if (!control.Resources.ContainsKey(typeName) && Core.Instance.InnerClasses.Contains(sourceUri))
                    {
                        using var stream = AssetLoader.Open(sourceUri);
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        string xaml = parms != null && parms.Length > 0 ? string.Format(Encoding.UTF8.GetString(bytes), parms)
                            : Encoding.UTF8.GetString(bytes);
                        var target = AvaloniaRuntimeXamlLoader.Parse(xaml);
                        control.SetValue(avaloniaProperty, target);
                        bytes = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(styling, ex.Message);
            }
        }
        public static Window GetWindow(this Control control, out bool hasBase)
        {
            hasBase = false;
            Window window = null;
            try
            {
                var parent = control.Parent;
                while (window == null)
                {
                    if (parent.Parent is Window win)
                    {
                        window = win;
                        hasBase = win is WindowBase windowBase && windowBase.Locate;
                    }
                    else
                        parent = parent.Parent;
                }
            }
            catch { }
            return window;
        }
        internal static double GetParagraphOffsetX(double lineWidth, double paragraphWidth, TextAlignment textAlignment)
        {
            if (double.IsPositiveInfinity(paragraphWidth))
                return 0.0;
            return textAlignment switch
            {
                TextAlignment.Center => (paragraphWidth - lineWidth) / 2.0,
                TextAlignment.Right => paragraphWidth - lineWidth,
                _ => 0.0,
            };
        }
    }
}
using Avalonia.Controls;
using Avalonia.Extensions.Styles;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using PCLUntils.IEnumerables;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Avalonia.Extensions.Controls
{
    public static class ControlUtils
    {
        public static bool IsSameValue(this AvaloniaPropertyChangedEventArgs args) => args.OldValue == args.NewValue;
        internal static void UpdateStyles(this StyledElement element, Styling.Styles styles)
        {
            if (styles != null && element != null)
            {
                for (var idx = 0; idx < styles.Count; idx++)
                {
                    if (styles.ElementAt(idx) is Style style && style.Setters != null && style.Setters.Count > 0)
                    {
                        foreach (Setter setter in style.Setters)
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
        public static SizeF MeasureString(this string text, double fontSize, SKTypeface typeface)
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
                return new SizeF(width, height);
            }
            catch { }
            return new SizeF();
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
        public static void AddStyles(this IStyling styling, AvaloniaProperty avaloniaProperty) => styling.AddStyles(avaloniaProperty);
        public static void AddStyles(this IStyling styling, AvaloniaProperty avaloniaProperty, params object[] parms) => styling.AddStyles(avaloniaProperty, parms);
        public static void AddResource(this IStyling styling) => styling.AddResource();
        public static Window GetWindow(this IControl control, out bool hasBase)
        {
            hasBase = false;
            Window window = null;
            try
            {
                IControl parent = control.Parent;
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
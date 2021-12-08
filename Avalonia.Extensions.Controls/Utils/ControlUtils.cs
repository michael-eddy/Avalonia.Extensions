using Avalonia.Controls;
using Avalonia.Extensions.Styles;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace Avalonia.Extensions.Controls
{
    public static class ControlUtils
    {
        private const double Epsilon = 0.00000153;
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
        internal static string TypeName(this IControl control)
        {
            return control.GetType().Name;
        }
        internal static bool AreClose(Size size1, Size size2)
        {
            return AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height);
        }
        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
                return true;
            double delta = value1 - value2;
            return (delta < Epsilon) && (delta > -Epsilon);
        }
        public static SizeF MeasureString(this IWindowImpl impl, string content, Font font, float maxWidth = 0)
        {
            if (impl != null)
            {
                var graphic = impl.GetGraphics();
                StringFormat sf = StringFormat.GenericTypographic;
                sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                if (maxWidth != 0)
                    return graphic.MeasureString(content.Trim(), font, new SizeF(maxWidth, 0), sf);
                else
                    return graphic.MeasureString(content.Trim(), font, PointF.Empty, sf);
            }
            return default;
        }
        public static SizeF MeasureString(this string text, Font font, double maxwidth)
        {
            var p = Graphics.FromImage(new Bitmap(1, 1)).MeasureString(text, font,
                Convert.ToInt32(maxwidth * 96f / 100f));
            return new SizeF(p.Width * 100f / 96f, p.Height * 100f / 96f);
        }
        public static object InvokePrivateMethod(this Control control, string methodName, object[] parameters = null)
        {
            try
            {
                var type = control.GetType();
                MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic
                    | BindingFlags.Instance);
                if (methodInfo == null && type.BaseType != null)
                    methodInfo = type.BaseType.GetMethod(methodName, BindingFlags.NonPublic
                    | BindingFlags.Instance);
                return methodInfo?.Invoke(control, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        public static void InitStyle(this IStyling styling)
        {
            try
            {
                styling.AddResource();
            }
            catch { }
        }
        internal static Window GetWindow(this IControl control, out bool hasBase)
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
        internal static Font GetFont(this TextBlock textBlock)
        {
            var fontFamily = textBlock.FontFamily.Name;
            var fontSize = Convert.ToSingle(textBlock.FontSize);
            return new Font(fontFamily, fontSize);
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
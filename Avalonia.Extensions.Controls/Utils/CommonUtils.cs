using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Platform;
using System;
using System.Drawing;
using System.Linq;

namespace Avalonia.Extensions.Controls
{
    public static class CommonUtils
    {
        public static Graphics GetGraphics(this IWindowImpl impl)
        {
            try
            {
                return Graphics.FromHwnd(impl.Handle.Handle);
            }
            catch
            {
                Bitmap bitmap = new Bitmap(1, 1);
                return Graphics.FromImage(bitmap);
            }
        }
        public static int Upper(this double value)
        {
            return Convert.ToInt32(Math.Ceiling(value));
        }
        public static ResourceInclude AsResource(this string url)
        {
            var uri = new Uri(url);
            return uri.AsResource();
        }
        public static ResourceInclude AsResource(this Uri uri)
        {
            try
            {
                return new ResourceInclude { Source = uri };
            }
            catch { }
            return default;
        }
        internal static double ToRadians(this double angle)
        {
            return Math.PI * angle / 180;
        }
        public static bool SmallerThan(this PixelPoint pixelPoint, PixelPoint point, bool inCludeEquals = false)
        {
            if (inCludeEquals && pixelPoint.X <= point.X && pixelPoint.Y <= point.Y)
                return true;
            else
                return pixelPoint.X < point.X && pixelPoint.Y < point.Y;
        }
        public static bool BiggerThan(this PixelPoint pixelPoint, PixelPoint point, bool inCludeEquals = false)
        {
            if (inCludeEquals && pixelPoint.X >= point.X && pixelPoint.Y >= point.Y)
                return true;
            else
                return pixelPoint.X > point.X && pixelPoint.Y > point.Y;
        }
        internal static int ToInt32(this object obj)
        {
            try
            {
                if (obj is int result)
                    return result;
                else if (obj is double d && double.IsNaN(d))
                    return 0;
                else
                {
                    if (int.TryParse(obj.ToString(), out result))
                        return result;
                    else
                        return Convert.ToInt32(obj.ToString());
                }
            }
            catch
            {
                try
                {
                    var num = obj.ToString().Split('.').FirstOrDefault();
                    return string.IsNullOrEmpty(num) ? 0 : int.Parse(num);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// only target is Initialized
        /// </summary>
        /// <param name="visual">target</param>
        /// <returns>size</returns>
        public static double ActualWidth(this Visual visual)
        {
            return visual.Bounds.Width;
        }
        /// <summary>
        /// only target is Initialized
        /// </summary>
        /// <param name="visual">target</param>
        /// <returns>size</returns>
        public static double ActualHeight(this Visual visual)
        {
            return visual.Bounds.Height;
        }
    }
}
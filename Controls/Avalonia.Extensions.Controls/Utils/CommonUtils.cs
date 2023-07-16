using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using System;
using System.Reflection;
using System.Text;

namespace Avalonia.Extensions.Controls
{
    public static class CommonUtils
    {
        public static object GetProperty(this object obj, string propertyName)
        {
            Type type = obj.GetType();
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
            var field = type.GetProperty(propertyName, bindingAttr);
            return field?.GetValue(obj);
        }
        public static byte[] GetBytes(this Encoding encoding, string str, int length)
        {
            byte[] bytesResult = new byte[length]; try
            {
                var tempBytes = encoding.GetBytes(str);
                Array.Copy(tempBytes, bytesResult, tempBytes.Length);
            }
            catch { }
            return bytesResult;
        }
        public static void Shutdown(this Application application, int exitCode = 0)
        {
            if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                lifetime.Shutdown(exitCode);
        }
        public static void TryShutdown(this Application application, int exitCode = 0)
        {
            if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                lifetime.TryShutdown(exitCode);
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
                return new ResourceInclude(uri) { Source = uri };
            }
            catch { }
            return default;
        }
        internal static double ToRadians(this double angle) => Math.PI * angle / 180;
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
        /// <summary>
        /// only target is Initialized
        /// </summary>
        /// <param name="visual">target</param>
        /// <returns>size</returns>
        public static double ActualWidth(this Visual visual) => visual.Bounds.Width;
        /// <summary>
        /// only target is Initialized
        /// </summary>
        /// <param name="visual">target</param>
        /// <returns>size</returns>
        public static double ActualHeight(this Visual visual) => visual.Bounds.Height;
    }
}
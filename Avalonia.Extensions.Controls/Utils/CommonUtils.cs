using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.MarkupExtensions;
using System;

namespace Avalonia.Extensions.Controls
{
    public static class CommonUtils
    {
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
                return new ResourceInclude { Source = uri };
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
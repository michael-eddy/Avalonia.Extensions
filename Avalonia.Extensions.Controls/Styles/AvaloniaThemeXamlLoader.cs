using Avalonia.Extensions.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using System;
using System.Diagnostics;
using System.Text;

namespace Avalonia.Extensions.Styles
{
    internal static class AvaloniaThemeXamlLoader
    {
        public static void ApplyTheme(this StyledElement element, Uri sourceUri)
        {
            if (!Core.Instance.InnerClasses.Contains(sourceUri))
                return;
            try
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                using var stream = assets.Open(sourceUri);
                var bytes = new byte[stream.Length];
                stream.Read(bytes);
                var xaml = Encoding.UTF8.GetString(bytes);
                var styles = AvaloniaRuntimeXamlLoader.Parse<Styling.Styles>(xaml);
                element.UpdateStyles(styles);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
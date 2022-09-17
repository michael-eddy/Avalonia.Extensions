using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using System;
using System.Text;

namespace Avalonia.Extensions.Styles
{
    public interface IStyling : IStyleable
    {
        void AddStyles(AvaloniaProperty avaloniaProperty) => AddStyles(avaloniaProperty, null);
        void AddStyles(AvaloniaProperty avaloniaProperty, params object[] parms)
        {
            try
            {
                if (this is Control control)
                {
                    string typeName = GetType().Name, xaml = string.Empty;
                    var sourceUri = new Uri($"avares://Avalonia.Extensions.Controls/Styles/Xaml/{typeName}.xml");
                    if (!control.Resources.ContainsKey(typeName) && Core.Instance.InnerClasses.Contains(sourceUri))
                    {
                        using var stream = Core.Instance.AssetLoader.Open(sourceUri);
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        if (parms != null && parms.Length > 0)
                            xaml = string.Format(Encoding.UTF8.GetString(bytes), parms);
                        else
                            xaml = Encoding.UTF8.GetString(bytes);
                        var target = AvaloniaRuntimeXamlLoader.Parse(xaml);
                        control.SetValue(avaloniaProperty, target);
                        bytes = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(this, ex.Message);
            }
        }
        void AddResource()
        {
            try
            {
                if (this is Control control)
                {
                    string typeName = GetType().Name,
                    sourceUrl = $"avares://Avalonia.Extensions.Controls/Styles/Xaml/{typeName}.xml";
                    var sourceUri = new Uri(sourceUrl);
                    if (!control.Resources.ContainsKey(typeName) && Core.Instance.InnerClasses.Contains(sourceUri))
                    {
                        control.Resources.Add(typeName, sourceUrl.AsResource());
                        control.ApplyTheme(sourceUri);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(this, ex.Message);
            }
        }
    }
}
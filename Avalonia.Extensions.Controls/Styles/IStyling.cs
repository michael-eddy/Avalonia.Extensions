using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Styling;
using Avalonia.Threading;
using System;
using System.Diagnostics;

namespace Avalonia.Extensions.Styles
{
    public interface IStyling : IStyleable
    {
        void AddResource()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
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
                    Debug.WriteLine(ex.Message);
                }
            });
        }
    }
}
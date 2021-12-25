﻿using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.Text;

namespace Avalonia.Extensions.Styles
{
    public interface IStyling : IStyleable
    {
        void AddStyles(AvaloniaProperty avaloniaProperty)
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
                            using var stream = Core.Instance.AssetLoader.Open(sourceUri);
                            var bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, bytes.Length);
                            var xaml = Encoding.UTF8.GetString(bytes);
                            var target = AvaloniaRuntimeXamlLoader.Parse(xaml);
                            control.SetValue(avaloniaProperty, target);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }
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
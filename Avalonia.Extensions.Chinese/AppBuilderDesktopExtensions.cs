﻿using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.Extensions.Controls
{
    public static class AppBuilderDesktopExtensions
    {
        /// <summary>
        /// set chinese support fontfamily for controls
        /// </summary>
        /// <param name="supportContols">if default, it just works on <seealso cref="TextBox"/> and <seealso cref="TextPresenter"/></param>
        public static TAppBuilder UseChineseInputSupport<TAppBuilder>(this TAppBuilder builder, params Type[] supportContols)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup((_) =>
            {
                try
                {
                    var contols = default(IEnumerable<Type>);
                    if (supportContols == null || supportContols.Count() == 0)
                        contols = new[] { typeof(TextBox), typeof(TextPresenter) };
                    else
                        contols = supportContols;
                    foreach (var supportContol in contols)
                    {
                        if (supportContol.FullName.StartsWith("Avalonia.Controls", StringComparison.OrdinalIgnoreCase) ||
                            supportContol.FullName.StartsWith("Avalonia.Extensions.Controls", StringComparison.OrdinalIgnoreCase))
                        {
                            var style = new Style();
                            style.Selector = default(Selector).OfType(supportContol);
                            style.Setters.Add(new Setter(TemplatedControl.FontFamilyProperty,
                                new FontFamily("avares://Avalonia.Extensions.Chinese/Assets/Fonts#WenQuanYi Micro Hei")));
                            Application.Current.Styles.Add(style);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(builder, ex.Message);
                    throw ex;
                }
            });
            return builder;
        }
    }
}
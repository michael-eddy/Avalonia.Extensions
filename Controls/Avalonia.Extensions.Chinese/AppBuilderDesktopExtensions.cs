using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Linq;
using System.Text;

namespace Avalonia.Extensions.Controls
{
    public static class AppBuilderDesktopExtensions
    {
        /// <summary>
        /// set chinese support fontfamily for controls
        /// </summary>
        /// <param name="supportContols">if default, it just works on <seealso cref="TextBox"/>、<seealso cref="TextPresenter"/> and <seealso cref="TextBlock"/></param>
        public static TAppBuilder UseChineseInputSupport<TAppBuilder>(this TAppBuilder builder, params Type[] supportContols)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            builder.AfterSetup((_) =>
            {
                try
                {
                    var contols = new[] { typeof(TextBox), typeof(TextPresenter), typeof(TextBlock) };
                    if (supportContols == null || supportContols.Count() == 0)
                        supportContols = contols;
                    foreach (var supportContol in supportContols)
                    {
                        if (supportContol.FullName.StartsWith("Avalonia.Controls", StringComparison.OrdinalIgnoreCase) ||
                            supportContol.FullName.StartsWith("Avalonia.Extensions.Controls", StringComparison.OrdinalIgnoreCase))
                        {
                            var style = new Style();
                            style.Selector = default(Selector).OfType(supportContol);
                            style.Setters.Add(new Setter(TemplatedControl.FontFamilyProperty, new FontFamily(FontManagerImpl.FONT_LOCATION)));
                            Application.Current.Styles.Add(style);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(builder, "UseChineseInputSupport Error:" + ex.Message);
                }
            });
            return builder;
        }
    }
}
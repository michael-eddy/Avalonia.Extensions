using Avalonia.Collections;
using Avalonia.Logging;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using PCLUntils.Assemblly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace Avalonia.Extensions.Controls
{
    public sealed class Core
    {
        private static Core instance;
        public static Core Instance
        {
            get
            {
                instance ??= new Core();
                return instance;
            }
        }
        private Core()
        {
            InnerClasses = new List<Uri>();
            Transparent = new SolidColorBrush(Colors.Transparent);
        }
        public ThemeMode GetThemeType()
        {
            ThemeMode mode = ThemeMode.Light;
            try
            {
                var styles = Application.Current.Styles[0]?.Children.GetPrivateField<AvaloniaList<IStyle>>("_styles");
                foreach (var style in styles)
                {
                    if (style is StyleInclude include)
                    {
                        if (include.Source.ToString().Contains("basedark", StringComparison.CurrentCultureIgnoreCase))
                        {
                            mode = ThemeMode.Dark;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return mode;
        }
        internal void Init()
        {
            try
            {
                var assets = AssetLoader.GetAssets(new Uri("avares://Avalonia.Extensions.Controls/Styles/Xaml"),
                      new Uri("avares://Avalonia.Extensions.Controls"));
                var enumerator = assets.GetEnumerator();
                while (enumerator.MoveNext())
                    InnerClasses.Add(enumerator.Current);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
        internal List<Uri> InnerClasses { get; private set; }
        internal bool IsEnglish => !CultureInfo.CurrentCulture.Name.Contains("zh", StringComparison.CurrentCultureIgnoreCase);
        private HttpClient HttpClient { get; set; }
        public HttpClient GetClient()
        {
            try
            {
                if (HttpClient == null)
                {
                    var clientHandler = new HttpClientHandler();
                    clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    clientHandler.ServerCertificateCustomValidationCallback += (_, _, _, _) => true;
                    HttpClient = new HttpClient(clientHandler);
                }
                return HttpClient;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                throw ex;
            }
        }
        internal void Dispose()
        {
            try
            {
                InnerClasses.Clear();
                HttpClient?.Dispose();
                InnerClasses = null;
            }
            catch { }
        }
        internal SolidColorBrush Transparent { get; }
    }
}
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using Color = Avalonia.Media.Color;

namespace Avalonia.Extensions.Controls
{
    public sealed class Core
    {
        private static Core instance;
        public static Core Instance
        {
            get
            {
                if (instance == null)
                    instance = new Core();
                return instance;
            }
        }
        private Core()
        {
            InnerClasses = new List<Uri>();
            Transparent = new SolidColorBrush(Colors.Transparent);
        }
        internal void Init()
        {
            try
            {
                AssetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
                var assets = AssetLoader.GetAssets(new Uri("avares://Avalonia.Extensions.Controls/Styles/Xaml"),
                      new Uri("avares://Avalonia.Extensions.Controls"));
                var enumerator = assets.GetEnumerator();
                while (enumerator.MoveNext())
                    InnerClasses.Add(enumerator.Current);
            }
            catch(Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
        internal List<Uri> InnerClasses { get; private set; }
        internal bool IsEnglish => !CultureInfo.CurrentCulture.Name.Contains("zh", StringComparison.CurrentCultureIgnoreCase);
        public IAssetLoader AssetLoader { get; private set; }
        private HttpClient HttpClient { get; set; }
        public HttpClient GetClient()
        {
            try
            {
                if (HttpClient == null)
                {
                    var clientHandler = new HttpClientHandler();
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
                HttpClient.Dispose();
                InnerClasses = null;
                _primaryBrush = null;
            }
            catch { }
        }
        internal SolidColorBrush Transparent { get; }
        private SolidColorBrush _primaryBrush;
        internal SolidColorBrush PrimaryBrush
        {
            get
            {
                if (_primaryBrush == null)
                    _primaryBrush = new SolidColorBrush(Color.FromRgb(139, 68, 172));
                return _primaryBrush;
            }
        }
    }
}
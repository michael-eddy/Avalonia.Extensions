using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Media;
using Avalonia.Logging;
using System;
using System.Net.Http;

namespace Avalonia.Extensions.Threading
{
    internal sealed class BitmapThread
    {
        private bool Loading = false;
        private HttpClient HttpClient { get; }
        private IBitmapSource Owner { get; }
        public BitmapThread(IBitmapSource owner)
        {
            Owner = owner;
            HttpClient = Core.Instance.GetClient();
        }
        public bool Create(Uri uri, out string message)
        {
            message = string.Empty;
            switch (uri.Scheme)
            {
                case "http":
                case "https":
                    {
                        Create(uri.AbsoluteUri);
                        return true;
                    }
                case "avares":
                    {
                        try
                        {
                            var assets = Core.Instance.AssetLoader;
                            Owner.SetBitmapSource(assets.Open(uri));
                        }
                        catch { }
                        return true;
                    }
                default:
                    {
                        message = "unsupport URI scheme.only support HTTP/HTTPS or avares://";
                        return false;
                    }
            }
        }
        private async void Create(string url)
        {
            if (!Loading)
            {
                Loading = true;
                try
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        HttpResponseMessage hr = await HttpClient.GetAsync(url);
                        hr.EnsureSuccessStatusCode();
                        var stream = await hr.Content.ReadAsStreamAsync();
                        Owner.SetBitmapSource(stream);
                    }
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(Owner, ex.Message);
                }
                Loading = false;
            }
        }
    }
}
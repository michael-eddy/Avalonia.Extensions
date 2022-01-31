using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Media;
using Avalonia.Logging;
using System;
using System.Net.Http;
using System.Threading;

namespace Avalonia.Extensions.Threading
{
    internal sealed class BitmapThread
    {
        private HttpClient HttpClient { get; }
        private IBitmapSource Owner { get; }
        internal event CompleteEventHandler CompleteEvent;
        internal delegate void CompleteEventHandler(object sender, bool success, string message);
        public BitmapThread(IBitmapSource owner)
        {
            Owner = owner;
            HttpClient = Core.Instance.GetClient();
        }
        public void Run(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "http":
                case "https":
                    {
                        ThreadPool.QueueUserWorkItem(HttpLoader, uri);
                        break;
                    }
                case "avares":
                    {
                        ThreadPool.QueueUserWorkItem(AssetsLoader, uri);
                        break;
                    }
                default:
                    throw new NotSupportedException("unsupport URI scheme.only support HTTP/HTTPS or avares://");
            }
        }
        private void AssetsLoader(object state)
        {
            if (state is Uri uri)
            {
                try
                {
                    var assets = Core.Instance.AssetLoader;
                    var stream = assets.Open(uri);
                    Owner.SetBitmapSource(stream);
                }
                catch (Exception ex)
                {
                    CompleteEvent?.Invoke(this, false, ex.Message);
                }
            }
        }
        private void HttpLoader(object state)
        {
            if (state is Uri uri)
            {
                var url = uri.AbsoluteUri;
                try
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        HttpResponseMessage hr = HttpClient.GetAsync(url).GetAwaiter().GetResult();
                        hr.EnsureSuccessStatusCode();
                        var stream = hr.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                        Owner.SetBitmapSource(stream);
                        CompleteEvent?.Invoke(this, true, string.Empty);
                    }
                    else
                        CompleteEvent?.Invoke(this, false, "URL cannot be NULL or EMPTY.");
                }
                catch (Exception ex)
                {
                    CompleteEvent?.Invoke(this, false, ex.Message);
                    Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(Owner, ex.Message);
                }
            }
        }
    }
}
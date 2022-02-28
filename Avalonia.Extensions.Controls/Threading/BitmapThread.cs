using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Media;
using Avalonia.Logging;
using Avalonia.Threading;
using System;
using System.Net.Http;

namespace Avalonia.Extensions.Threading
{
    internal sealed class BitmapThread
    {
        private IBitmapSource Owner { get; }
        private HttpClient HttpClient { get; }
        internal event CompleteEventHandler CompleteEvent;
        internal delegate void CompleteEventHandler(object sender, bool success, string message);
        public BitmapThread(IBitmapSource owner)
        {
            Owner = owner;
            HttpClient = Core.Instance.GetClient();
        }
        public void Run(Uri uri)
        {
            try
            {
                switch (uri.Scheme)
                {
                    case "http":
                    case "https":
                        {
                            Dispatcher.UIThread.InvokeAsync(() =>
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
                                    {
                                        CompleteEvent?.Invoke(this, false, "URL cannot be NULL or EMPTY.");
                                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(Owner, "URL cannot be NULL or EMPTY.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CompleteEvent?.Invoke(this, false, ex.Message);
                                    Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(Owner, ex.Message);
                                }
                            }, DispatcherPriority.ApplicationIdle);
                            break;
                        }
                    case "avares":
                        {
                            Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                try
                                {
                                    var stream = Core.Instance.AssetLoader.Open(uri);
                                    Owner.SetBitmapSource(stream);
                                }
                                catch (Exception ex)
                                {
                                    CompleteEvent?.Invoke(this, false, ex.Message);
                                }
                            }, DispatcherPriority.ApplicationIdle);
                            break;
                        }
                    default:
                        {
                            Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(Owner, "unsupport URI scheme.only support HTTP/HTTPS or avares://");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
    }
}
using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Media;
using Avalonia.Logging;
using Avalonia.Threading;
using System;
using System.IO;
using System.Net.Http;

namespace Avalonia.Extensions.Threading
{
    internal sealed class BitmapThread
    {
        private IBitmapSource Owner { get; }
        private HttpClient HttpClient { get; }
        public BitmapThread(IBitmapSource owner)
        {
            Owner = owner;
            HttpClient = Core.Instance.GetClient();
        }
        public void Run(Uri uri)
        {
            if (uri == null) return;
            try
            {
                switch (uri.Scheme)
                {
                    case "http":
                    case "https":
                        {
                            Dispatcher.UIThread.InvokeAsync(async () =>
                            {
                                var url = uri.AbsoluteUri;
                                try
                                {
                                    if (!string.IsNullOrEmpty(url))
                                    {
                                        var hr = await HttpClient.GetAsync(url);
                                        hr.EnsureSuccessStatusCode();
                                        var stream = await hr.Content.ReadAsStreamAsync();
                                        Owner.SetBitmapSource(stream);
                                        Owner.ResultSet(true, string.Empty);
                                    }
                                    else
                                    {
                                        Owner.ResultSet(false, "URL cannot be NULL or EMPTY.");
                                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(Owner, "URL cannot be NULL or EMPTY.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Owner.ResultSet(false, ex.Message);
                                    Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(Owner, ex.Message);
                                }
                            }, DispatcherPriority.ApplicationIdle);
                            break;
                        }
                    case "file":
                        {
                            Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                try
                                {
                                    FileInfo fileInfo = new FileInfo(uri.AbsolutePath);
                                    if (fileInfo.Exists)
                                    {
                                        var stream = fileInfo.OpenRead();
                                        Owner.SetBitmapSource(stream);
                                    }
                                    else
                                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(Owner, $"file 【{fileInfo.FullName}】 not found");
                                }
                                catch (Exception ex)
                                {
                                    Owner.ResultSet(false, ex.Message);
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
                                    Owner.ResultSet(false, ex.Message);
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
                Owner.ResultSet(false, ex.Message);
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
    }
}
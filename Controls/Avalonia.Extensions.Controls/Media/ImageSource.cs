using Avalonia.Extensions.Controls;
using Avalonia.Logging;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using PCLUntils.Objects;
using System;
using System.IO;
using System.Net.Http;

namespace Avalonia.Extensions.Media
{
    internal static class ImageSource
    {
        static HttpClient HttpClient => Core.Instance.GetClient();
        public static Bitmap ToBitmap(this Draw control, Uri uri)
        {
            Bitmap bitmap = null;
            if (uri != null)
            {
                try
                {
                    switch (uri.Scheme)
                    {
                        case "http":
                        case "https":
                            {
                                var url = uri.AbsoluteUri;
                                try
                                {
                                    if (!string.IsNullOrEmpty(url))
                                    {
                                        var hr = HttpClient.GetAsync(url).Result;
                                        hr.EnsureSuccessStatusCode();
                                        var stream = hr.Content.ReadAsStreamAsync().Result;
                                        bitmap = CreateBitmap(control, stream);
                                    }
                                    else
                                    {
                                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(control, "URL cannot be NULL or EMPTY.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(control, ex.Message);
                                }
                                break;
                            }
                        case "file":
                            {
                                try
                                {
                                    FileInfo fileInfo = new FileInfo(uri.AbsolutePath);
                                    if (fileInfo.Exists)
                                    {
                                        var stream = fileInfo.OpenRead();
                                        bitmap = new Bitmap(stream);
                                    }
                                    else
                                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(control, $"file 【{fileInfo.FullName}】 not found");
                                }
                                catch (Exception ex)
                                {
                                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(control, ex.Message);
                                }
                                break;
                            }
                        case "avares":
                            {
                                try
                                {
                                    var stream = Core.Instance.AssetLoader.Open(uri);
                                    bitmap = new Bitmap(stream);
                                }
                                catch (Exception ex)
                                {
                                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(control, ex.Message);
                                }
                                break;
                            }
                        default:
                            {
                                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(control, "unsupport URI scheme.only support HTTP/HTTPS or avares://");
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(control, ex.Message);
                }
            }
            return bitmap;
        }
        public static void ToStream(this IBitmapSource owner, Uri uri)
        {
            if (uri == null)
                return;
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
                                        owner.SetBitmapSource(stream);
                                        owner.ResultSet(true, string.Empty);
                                    }
                                    else
                                    {
                                        owner.ResultSet(false, "URL cannot be NULL or EMPTY.");
                                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(owner, "URL cannot be NULL or EMPTY.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    owner.ResultSet(false, ex.Message);
                                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(owner, ex.Message);
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
                                        owner.SetBitmapSource(stream);
                                    }
                                    else
                                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(owner, $"file 【{fileInfo.FullName}】 not found");
                                }
                                catch (Exception ex)
                                {
                                    owner.ResultSet(false, ex.Message);
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
                                    owner.SetBitmapSource(stream);
                                }
                                catch (Exception ex)
                                {
                                    owner.ResultSet(false, ex.Message);
                                }
                            }, DispatcherPriority.ApplicationIdle);
                            break;
                        }
                    default:
                        {
                            Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(owner, "unsupport URI scheme.only support HTTP/HTTPS or avares://");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                owner.ResultSet(false, ex.Message);
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(owner, ex.Message);
            }
        }
        private static Bitmap CreateBitmap(Draw control, Stream stream)
        {
            Bitmap bitmap;
            int w = control.Width.ToInt32(true), h = control.Height.ToInt32(true);
            if ((double.IsNaN(control.Width) || w == 0) && (double.IsNaN(control.Height) || h == 0))
                bitmap = new Bitmap(stream);
            else if (!double.IsNaN(control.Width) && w != 0)
                bitmap = Bitmap.DecodeToWidth(stream, w, control.InterpolationMode);
            else
                bitmap = Bitmap.DecodeToHeight(stream, h, control.InterpolationMode);
            return bitmap;
        }
    }
}
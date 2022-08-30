using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Logging;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Avalonia.Extensions.Danmaku
{
    public class DanmakuNativeView : Panel, IDisposable
    {
        private long cursor = 0;
        private List<DanmakuModel> danmakus;
        private HttpClient HttpClient { get; }
        private readonly DispatcherTimer timer;
        static DanmakuNativeView()
        {
            BoundsProperty.Changed.AddClassHandler<DanmakuNativeView>(OnBoundsChange);
        }
        public DanmakuNativeView()
        {
            HttpClient = Core.Instance.GetClient();
            timer = new DispatcherTimer(TimeSpan.FromMilliseconds(80), DispatcherPriority.Background, OnTimerCallback);
        }
        private void OnTimerCallback(object sender, EventArgs e)
        {
            try
            {
                var appends = danmakus.Where(x => x.Time >= cursor && x.Time < (cursor += 80));
                if (appends != null)
                {
                    foreach (var append in appends)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
        private static void OnBoundsChange(DanmakuNativeView view, AvaloniaPropertyChangedEventArgs arg)
        {
        }
        public void LoadXml(string xml)
        {
            cursor = 0;
            if (!string.IsNullOrEmpty(xml))
                danmakus = DanmakuParser.Instance.ParseBiliBiliXml(xml);
        }
        public void Start()
        {
            if (danmakus != null && danmakus.Count > 0)
                timer.Start();
        }
        public void ReStart()
        {
            cursor = 0;
            Start();
        }
        public void SeekTo(long position) => cursor = position;
        public void Stop() => timer.Stop();
        public void LoadFile(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "file":
                    {
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            try
                            {
                                FileInfo fileInfo = new FileInfo(uri.ToString().Replace("file:///", ""));
                                if (fileInfo.Exists)
                                {
                                    using var fs = fileInfo.OpenRead();
                                    ReadStream(fs);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
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
                                ReadStream(Core.Instance.AssetLoader.Open(uri));
                            }
                            catch (Exception ex)
                            {
                                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                            }
                        }, DispatcherPriority.ApplicationIdle);
                        break;
                    }
                case "http":
                case "https":
                    {
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            try
                            {
                                HttpResponseMessage hr = HttpClient.GetAsync(uri).GetAwaiter().GetResult();
                                hr.EnsureSuccessStatusCode();
                                var stream = hr.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                                ReadStream(stream);
                            }
                            catch (Exception ex)
                            {
                                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                            }
                        }, DispatcherPriority.ApplicationIdle);
                        break;
                    }
            }
        }
        private void ReadStream(Stream stream)
        {
            try
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes);
                LoadXml(Encoding.UTF8.GetString(bytes));
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
        void IDisposable.Dispose()
        {
            Stop();
            danmakus = null;
        }
    }
}
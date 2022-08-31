﻿using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Danmaku.Controls.Native;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Threading;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Avalonia.Extensions.Danmaku
{
    public class DanmakuNativeView : Panel, IDisposable
    {
        internal readonly Random random;
        private readonly HttpClient httpClient;
        private readonly DispatcherTimer timer;
        private List<DanmakuModel> danmakus;
        private readonly DispatcherTimer clearTimer;
        internal float Timeline { get; private set; } = 0;
        internal int PartWidth { get; private set; } = 0;
        internal int ActualHeight { get; private set; } = 0;
        internal (int Top, int Center, int Bottom) RangeHeight { get; private set; } = (0, 0, 0);
        static DanmakuNativeView()
        {
            BoundsProperty.Changed.AddClassHandler<DanmakuNativeView>(OnBoundsChange);
        }
        public DanmakuNativeView()
        {
            random = new Random(36);
            httpClient = Core.Instance.GetClient();
            timer = new DispatcherTimer(TimeSpan.FromMilliseconds(80), DispatcherPriority.Background, OnTimerCallback);
            clearTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(200), DispatcherPriority.Background, OnClearTimerCallback);
        }
        private void OnClearTimerCallback(object sender, EventArgs e)
        {
            clearTimer.Stop();
            try
            {
                for (var idx = 0; idx < Children.Count; idx++)
                {
                    if (Children[idx] is DanmakuScrollView view)
                    {
                        if (view.IsCompleted)
                            Children.Remove(view);
                    }
                    if (Children[idx] is DanmakuTipsView tipsView)
                    {
                        if (Timeline - tipsView.startCursor >= 5)
                            Children.Remove(tipsView);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            clearTimer.Start();
        }
        private void OnTimerCallback(object sender, EventArgs e)
        {
            try
            {
                float start = Timeline, end = Timeline += 0.08F;
                var appends = danmakus.Where(x => x.Time >= start && x.Time < end);
                if (appends != null)
                {
                    foreach (var append in appends)
                    {
                        if (append.Location == DanmakuLocation.Roll)
                        {
                            var tb = new DanmakuScrollView();
                            tb.SetData(this, append);
                            Children.Add(tb);
                        }
                        else if (append.Location != DanmakuLocation.Other)
                        {
                            var tb = new DanmakuTipsView();
                            tb.SetData(this, append);
                            Children.Add(tb);
                        }
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
            if (arg.NewValue is Rect rect)
            {
                var p = Convert.ToInt32(rect.Height / 3);
                view.RangeHeight = (p, p * 2, p * 3);
                view.ActualHeight = Convert.ToInt32(rect.Height);
                view.PartWidth = Convert.ToInt32(rect.Width / 2);
            }
        }
        public void LoadXml(string xml)
        {
            Timeline = 0;
            if (!string.IsNullOrEmpty(xml))
            {
                danmakus = DanmakuParser.Instance.ParseBiliBiliXml(xml);
                Start();
            }
        }
        public void Start()
        {
            if (danmakus != null && danmakus.Count > 0)
            {
                timer.Start();
                clearTimer.Start();
            }
        }
        public void ReStart()
        {
            Timeline = 0;
            Start();
        }
        public void SeekTo(long position) => Timeline = position;
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
                                HttpResponseMessage hr = httpClient.GetAsync(uri).GetAwaiter().GetResult();
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
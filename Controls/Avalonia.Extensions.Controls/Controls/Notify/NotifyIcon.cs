using Avalonia.Controls;
using Avalonia.Extensions.Event;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Threading;
using PCLUntils;
using PCLUntils.Plantform;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Controls
{
    public class NotifyIcon : Control
    {
        private readonly INotifyIcon notifyIcon;
        private readonly HttpClient httpClient;
        static NotifyIcon()
        {
            IconProperty.Changed.AddClassHandler<NotifyIcon>(OnIconUpdate);
            IconTipProperty.Changed.AddClassHandler<NotifyIcon>(OnUpdate);
            IconTitleProperty.Changed.AddClassHandler<NotifyIcon>(OnUpdate);
            IconMessageProperty.Changed.AddClassHandler<NotifyIcon>(OnUpdate);
        }
        public NotifyIcon()
        {
            httpClient = Core.Instance.GetClient();
            switch (PlantformUntils.System)
            {
                case Platforms.Windows:
                    notifyIcon = new NotifyIconWin(this);
                    break;
            }
        }
        public bool Add() => notifyIcon.Add();
        public bool Update() => notifyIcon.Update();
        public bool Show() => notifyIcon.Show();
        public bool Hide() => notifyIcon.Hide();
        public static readonly StyledProperty<string?> IconTipProperty =
            AvaloniaProperty.Register<NotifyIcon, string?>(nameof(IconTip));
        public string? IconTip
        {
            get => GetValue(IconTipProperty);
            set => SetValue(IconTipProperty, value);
        }
        public static readonly StyledProperty<string?> IconTitleProperty =
            AvaloniaProperty.Register<NotifyIcon, string?>(nameof(IconTitle));
        public string? IconTitle
        {
            get => GetValue(IconTitleProperty);
            set => SetValue(IconTitleProperty, value);
        }
        public static readonly StyledProperty<string?> IconMessageProperty =
            AvaloniaProperty.Register<NotifyIcon, string?>(nameof(IconMessage));
        public string? IconMessage
        {
            get => GetValue(IconMessageProperty);
            set => SetValue(IconMessageProperty, value);
        }
        public static readonly StyledProperty<Uri?> IconProperty =
            AvaloniaProperty.Register<NotifyIcon, Uri?>(nameof(Icon));
        public Uri? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly RoutedEvent<ClickEventArgs> ClickEvent =
            RoutedEvent.Register<NotifyIcon, ClickEventArgs>(nameof(Click), RoutingStrategies.Bubble);
        /// <summary>
        /// Raised when the user clicks the icon.
        /// </summary>
        public event EventHandler<ClickEventArgs> Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
        public static readonly RoutedEvent<RoutedEventArgs> MinEvent =
            RoutedEvent.Register<NotifyIcon, RoutedEventArgs>(nameof(Min), RoutingStrategies.Bubble);
        /// <summary>
        /// Raised when the user clicks the icon.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Min
        {
            add { AddHandler(MinEvent, value); }
            remove { RemoveHandler(MinEvent, value); }
        }
        public async Task<Stream> GetBytes(Uri uri)
        {
            Stream stream = default;
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
                                var hr = await httpClient.GetAsync(url);
                                hr.EnsureSuccessStatusCode();
                                stream = await hr.Content.ReadAsStreamAsync();
                            }
                            else
                                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, "URL cannot be NULL or EMPTY.");
                        }
                        catch (Exception ex)
                        {
                            Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(this, ex.Message);
                        }
                        break;
                    }
                case "file":
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(uri.AbsolutePath);
                            if (fileInfo.Exists)
                                stream = fileInfo.OpenRead();
                            else
                                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, $"file 【{fileInfo.FullName}】 not found");
                        }
                        catch (Exception ex)
                        {
                            Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                        }
                        break;
                    }
                case "avares":
                    {
                        try
                        {
                            stream = Core.Instance.AssetLoader.Open(uri);
                        }
                        catch (Exception ex)
                        {
                            Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                        }
                        break;
                    }
                default:
                    {
                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, "unsupport URI scheme.only support HTTP/HTTPS or avares://");
                        break;
                    }
            }
            return stream;
        }
        public void MouseClick(MouseButton mouseButton, Point pos)
        {
            try
            {
                var e = new ClickEventArgs(ClickEvent, mouseButton, pos);
                RaiseEvent(e);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
        public void MinHandle()
        {
            try
            {
                var e = new RoutedEventArgs(MinEvent);
                RaiseEvent(e);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
        private static void OnUpdate(NotifyIcon sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                sender.notifyIcon.Update();
        }
        private static async void OnIconUpdate(NotifyIcon sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is Uri uri)
            {
                await sender.notifyIcon.GetHIcon(uri);
                sender.notifyIcon.Update();
            }
        }
    }
}
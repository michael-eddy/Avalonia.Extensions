using Avalonia.Controls;
using Avalonia.Extensions.Base;
using PCLUntils;
using PCLUntils.Plantform;
using System;

namespace Avalonia.Extensions.Controls
{
    public class NotifyIcon : Control
    {
        private readonly INotifyIcon notifyIcon;
        static NotifyIcon()
        {
            IconTipProperty.Changed.AddClassHandler<NotifyIcon>(OnUpdate);
            IconTitleProperty.Changed.AddClassHandler<NotifyIcon>(OnUpdate);
            IconHwndProperty.Changed.AddClassHandler<NotifyIcon>(OnUpdate);
            IconMessageProperty.Changed.AddClassHandler<NotifyIcon>(OnUpdate);
        }
        public NotifyIcon()
        {
            switch (PlantformUntils.System)
            {
                case Platforms.Windows:
                    notifyIcon = new NotifyIconWin(this);
                    break;
            }
        }
        public bool Add(Window window) => notifyIcon.Add(window.PlatformImpl.Handle.Handle);
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
        public static readonly StyledProperty<IntPtr?> IconHwndProperty =
            AvaloniaProperty.Register<NotifyIcon, IntPtr?>(nameof(IconHwnd));
        public IntPtr? IconHwnd
        {
            get => GetValue(IconHwndProperty);
            set => SetValue(IconHwndProperty, value);
        }
        private static void OnUpdate(NotifyIcon sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                sender.notifyIcon.Update();
        }
    }
}
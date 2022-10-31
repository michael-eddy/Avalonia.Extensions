using Avalonia.Logging;
using Avalonia.Win32;
using PCLUntils.Assemblly;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static Avalonia.Extensions.Base.Win32API;

namespace Avalonia.Extensions.Controls
{
    internal class NotifyIconWin : INotifyIcon
    {
        private readonly NotifyIcon Owner;
        private NotifyIconState state = NotifyIconState.None;
        public NotifyIconWin(NotifyIcon owner)
        {
            Owner = owner;
        }
        private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }
        private readonly WndProc delegWndProc = WndProc;
        public void License(IntPtr ptr)
        {
            if (messageWin == IntPtr.Zero)
            {
                var proc = Marshal.GetFunctionPointerForDelegate(delegWndProc);
                CreateNoneWindow($"DoveMessageWindow{Guid.NewGuid()}", ptr, proc);
            }
        }
        public NotifyIconData notifyIcon;
        public bool Add(IntPtr ptr)
        {
            try
            {
                License(ptr);
                notifyIcon = new NotifyIconData();
                notifyIcon.cbSize = Marshal.SizeOf(notifyIcon);
                notifyIcon.hWnd = ptr;
                notifyIcon.uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP;
                notifyIcon.uCallbackMessage = WM_NOTIFY_TRAY;
                notifyIcon.hIcon = Owner.IconHwnd ?? IntPtr.Zero;
                notifyIcon.uTimeoutAndVersion = NOTIFYICON_VERSION;
                notifyIcon.dwInfoFlags = NIIF_INFO;
                notifyIcon.szTip = Owner.IconTip;
                notifyIcon.szInfoTitle = Owner.IconTitle;
                notifyIcon.szInfo = Owner.IconMessage;
                if (ShellNotifyIcon(NIM_ADD, ref notifyIcon))
                {
                    state = NotifyIconState.Show;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return false;
        }
        public bool Update()
        {
            try
            {
                if (state != NotifyIconState.None)
                {
                    notifyIcon.hIcon = Owner.IconHwnd ?? IntPtr.Zero;
                    notifyIcon.szTip = Owner.IconTip;
                    notifyIcon.szInfoTitle = Owner.IconTitle;
                    notifyIcon.szInfo = Owner.IconMessage;
                    return ShellNotifyIcon(NIM_MODIFY, ref notifyIcon);
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return false;
        }
        public bool Hide()
        {
            try
            {
                if (state == NotifyIconState.Show)
                {
                    state = NotifyIconState.Hide;
                    return ShellNotifyIcon(NIM_DELETE, ref notifyIcon);
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return false;
        }
        public bool Show()
        {
            try
            {
                if (state == NotifyIconState.Hide)
                    return ShellNotifyIcon(NIM_ADD, ref notifyIcon);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return false;
        }
    }
}
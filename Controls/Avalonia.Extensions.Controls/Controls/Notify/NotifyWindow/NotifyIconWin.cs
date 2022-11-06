using Avalonia.Input;
using Avalonia.Logging;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Avalonia.Extensions.Base.Win32API;

namespace Avalonia.Extensions.Controls
{
    internal class NotifyIconWin : INotifyIcon
    {
        private static NotifyIcon Owner;
        private NotifyIconState state = NotifyIconState.None;
        public NotifyIconWin(NotifyIcon owner)
        {
            Owner = owner;
        }
        private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_NOTIFYICON:
                    GetCursorPos(out POINT pos);
                    var p = new Point(pos.X, pos.Y);
                    switch (lParam.ToInt64())
                    {
                        case WM_LBUTTONDBLCLK:
                            Owner.MouseClick(MouseButton.Left, p);
                            break;
                        case WM_RBUTTONDOWN:
                            Owner.MouseClick(MouseButton.Right, p);
                            break;
                    }
                    break;
                case WM_SYSCOMMAND:
                    switch (wParam.ToInt64())
                    {
                        case SC_MINIMIZE:
                            Owner.MinHandle();
                            break;
                        case SC_CLOSE:
                            Owner?.Hide();
                            break;
                    }
                    break;
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }
        private IntPtr hwnd;
        private readonly WndProc delegWndProc = WndProc;
        public void License()
        {
            try
            {
                if (messageWin == IntPtr.Zero)
                {
                    var proc = Marshal.GetFunctionPointerForDelegate(delegWndProc);
                    hwnd = CreateNoneWindow($"DoveMessageWindow{Guid.NewGuid()}", proc);
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
        public NotifyIconData notifyIcon;
        public bool Add()
        {
            License();
            try
            {
                notifyIcon = new NotifyIconData();
                notifyIcon.cbSize = Marshal.SizeOf(notifyIcon);
                notifyIcon.hWnd = hwnd;
                notifyIcon.uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP;
                notifyIcon.uCallbackMessage = WM_NOTIFYICON;
                notifyIcon.hIcon = hIcon;
                notifyIcon.uTimeoutOrVersion = NOTIFYICON_VERSION;
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
        private IntPtr hIcon = IntPtr.Zero;
        public async Task<bool> GetHIcon(Uri uri)
        {
            try
            {
                var bytes = await Owner.GetBytes(uri);
                hIcon = new System.Drawing.Icon(bytes).Handle;
                return true;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, "GetHIcon Failed:" + ex.Message);
                return false;
            }
        }
        public bool Update()
        {
            try
            {
                if (state != NotifyIconState.None)
                {
                    notifyIcon.hIcon = hIcon;
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
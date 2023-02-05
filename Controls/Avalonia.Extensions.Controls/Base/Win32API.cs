using Avalonia.Extensions.Model;
using Avalonia.Logging;
using System;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Base
{
    public sealed class Win32API
    {
        internal const int WM_NOTIFYICON = 0x1BF52;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int NOTIFYICON_VERSION = 0x03;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int NIIF_INFO = 0x01;
        public const int NIF_MESSAGE = 0x01;
        public const int NIF_ICON = 0x02;
        public const int NIF_TIP = 0x04;
        public const int NIM_ADD = 0x00;
        public const int NIM_MODIFY = 0x01;
        public const int NIM_DELETE = 0x02;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int CS_HREDRAW = 0x0002;
        public const int CS_VREDRAW = 0x0001;
        internal const int MONITOR_DEFAULT_TONEAREST = 0x00000002;
        public struct POINT
        {
            public int X;
            public int Y;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WNDCLASSEX
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public int style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpszClassName;
            public IntPtr hIconSm;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct NotifyIconData
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uID;
            public int uFlags;
            public int uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;
            public int dwState;
            public int dwStateMask;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szInfo;
            public int uTimeoutOrVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szInfoTitle;
            public int dwInfoFlags;
        }
        internal static IntPtr messageWin { get; private set; } = IntPtr.Zero;
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, NativeMonitorInfo lpmi);
        [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegisterClassExW")]
        public static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, int lpClassName, string lpWindowName, int dwStyle, int x, int y,
           int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
        public static IntPtr CreateNoneWindow(string lpClassName, IntPtr delegWndProc)
        {
            if (messageWin == IntPtr.Zero)
            {
                var instance = GetModuleHandle(null);
                WNDCLASSEX wndClassEx = new WNDCLASSEX();
                wndClassEx.cbSize = Marshal.SizeOf(wndClassEx);
                wndClassEx.hInstance = instance;
                wndClassEx.style = CS_HREDRAW | CS_VREDRAW;
                wndClassEx.lpfnWndProc = delegWndProc;
                wndClassEx.lpszClassName = lpClassName;
                wndClassEx.cbClsExtra = 0;
                wndClassEx.cbWndExtra = 0;
                var atom = RegisterClassEx(ref wndClassEx);
                if (atom != 0)
                {
                    messageWin = CreateWindowEx(0, atom, null, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, instance, IntPtr.Zero);
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(null, "GetLastError Result: " + GetLastError());
                }
            }
            return messageWin;
        }
        [DllImport("shell32.dll", CharSet = CharSet.Auto, EntryPoint = "Shell_NotifyIcon")]
        public static extern bool ShellNotifyIcon(int message, ref NotifyIconData pnid);
        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}
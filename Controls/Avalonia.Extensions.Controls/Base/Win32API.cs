using Avalonia.Logging;
using System;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Base
{
    public sealed class Win32API
    {
        public const int WM_NOTIFY_TRAY = 0x1BF52;
        public const int NOTIFYICON_VERSION = 0x03;
        public const int NIIF_INFO = 0x01;
        public const int NIF_MESSAGE = 0x01;
        public const int NIF_ICON = 0x02;
        public const int NIF_TIP = 0x04;
        public const int NIM_ADD = 0x00;
        public const int NIM_MODIFY = 0x01;
        public const int NIM_DELETE = 0x02;
        public const int CS_HREDRAW = 0x0002;
        public const int CS_VREDRAW = 0x0001;
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
        [StructLayout(LayoutKind.Sequential)]
        public struct NotifyIconData
        {
            /// <summary>
            /// 以字节为单位的这个结构的大小
            /// </summary>
            public int cbSize;
            /// <summary>
            /// 接收托盘图标通知消息的窗口句柄
            /// </summary>
            public IntPtr hWnd;
            /// <summary>
            /// 应用程序定义的该图标的ID号
            /// </summary>
            public int uID;
            /// <summary>
            /// 设置该图标的属性
            /// </summary>
            public int uFlags;
            /// <summary>
            /// 应用程序定义的消息ID号，此消息传递给hWnd 
            /// </summary>
            public int uCallbackMessage;
            /// <summary>
            /// 图标的句柄
            /// </summary>
            public IntPtr hIcon;
            /// <summary>
            /// 鼠标停留在图标上显示的提示信息
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
            public string szTip;
            /// <summary>
            /// 提示的超时值（几秒后自动消失）和版本
            /// </summary>
            public int uTimeoutAndVersion;
            /// <summary>
            /// 气泡提示框的标题
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public string szInfoTitle;
            /// <summary>
            ///  类型标志，有INFO、WARNING、ERROR，更改此值将影响气泡提示框的图标类型
            /// </summary>
            public int dwInfoFlags;
            /// <summary>
            /// 气泡提示框的提示内容
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xFF)]
            public string szInfo;
        }
        public struct POINT
        {
            public int X;
            public int Y;
        }
        internal static IntPtr messageWin { get; private set; } = IntPtr.Zero;
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegisterClassExW")]
        public static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateWindowEx(int dwExStyle, int lpClassName, string lpWindowName, int dwStyle, int x, int y,
           int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
        internal static void CreateNoneWindow(string lpClassName, IntPtr hInstance, IntPtr delegWndProc)
        {
            WNDCLASSEX wndClassEx = new WNDCLASSEX();
            wndClassEx.cbSize = Marshal.SizeOf(wndClassEx);
            wndClassEx.hInstance = hInstance;
            wndClassEx.style = CS_HREDRAW | CS_VREDRAW;
            wndClassEx.lpfnWndProc = delegWndProc;
            wndClassEx.lpszClassName = lpClassName;
            wndClassEx.cbClsExtra = 0;
            wndClassEx.cbWndExtra = 0;
            var atom = RegisterClassEx(ref wndClassEx);
            if (atom != 0)
            {
                messageWin = CreateWindowEx(0, atom, null, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(null, "GetLastError Result: " + GetLastError());
            }
        }
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint RegisterWindowMessage(string lpString);
        [DllImport("shell32.dll", CharSet = CharSet.Auto, EntryPoint = "Shell_NotifyIcon")]
        public static extern bool ShellNotifyIcon(int message, ref NotifyIconData pnid);
        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}
using System;
using System.Runtime.InteropServices;

namespace Danmaku.Wpf
{
    partial class WINAPI
    {
        internal static class USER32
        {
            const string MODULENAME = nameof(USER32);
            public const int GWL_STYLE = -16;
            public const int GWL_EXSTYLE = -20;
            public const int WS_EX_TRANSPARENT = 0x00000020;
            public const int WS_EX_TOOLWINDOW = 0x00000080;
            public const int WS_EX_LAYERED = 0x00080000;
            public const int WS_EX_NOREDIRECTIONBITMAP = 0x00200000;
            public enum ExtendedWindowStyles
            {
                Transparent = WS_EX_TRANSPARENT,
                ToolWindow = WS_EX_TOOLWINDOW,
                Layered = WS_EX_LAYERED,
                NoRedirectionBitmap = WS_EX_NOREDIRECTIONBITMAP
            }
            public enum WindowStylesKind
            {
                Styles = GWL_STYLE,
                ExStyles = GWL_EXSTYLE
            }
            [DllImport(MODULENAME, SetLastError = true)]
            public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
            public static int GetWindowStyles(IntPtr hWnd, WindowStylesKind kind) => GetWindowLong(hWnd, (int)kind);
            public static ExtendedWindowStyles GetExtendedWindowStyles(IntPtr hWnd)
                => (ExtendedWindowStyles)GetWindowStyles(hWnd, WindowStylesKind.ExStyles);
            [DllImport(MODULENAME, SetLastError = true)]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
            public static int SetWindowStyles(IntPtr hWnd, WindowStylesKind kind, int styles) => SetWindowLong(hWnd, (int)kind, styles);
            public static ExtendedWindowStyles SetExtendedWindowStyles(IntPtr hWnd, ExtendedWindowStyles styles)
                => (ExtendedWindowStyles)SetWindowStyles(hWnd, WindowStylesKind.ExStyles, (int)styles);
        }
    }
}
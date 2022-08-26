using Avalonia.Platform;
using Avalonia.Platform.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Danmaku
{
    public sealed class LibraryApi
    {
        internal sealed class Windows
        {
            public const uint WM_CLOSE = 0x0010;
            public const uint WM_DESTROY = 0x0002;
            public const int COLOR_WINDOW = 5;
            public const int IDC_ARROW = 32512;
            public const int IDI_APPLICATION = 32512;
            [StructLayout(LayoutKind.Sequential)]
            public struct SETTEXTEX
            {
                public uint Flags;
                public uint Codepage;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct WNDCLASSEX
            {
                public uint cbSize;
                public uint style;
                [MarshalAs(UnmanagedType.FunctionPtr)]
                public WndProc lpfnWndProc;
                public int cbClsExtra;
                public int cbWndExtra;
                public IntPtr hInstance;
                public IntPtr hIcon;
                public IntPtr hCursor;
                public IntPtr hbrBackground;
                public string lpszMenuName;
                public string lpszClassName;
                public IntPtr hIconSm;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct COPYDATASTRUCT
            {
                public IntPtr dwData;
                public int cbData;
                [MarshalAs(UnmanagedType.LPStr)]
                public string lpData;
            }
            public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
            [DllImport("kernel32.dll")]
            public static extern IntPtr LoadLibrary(string lib);
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool DestroyWindow(IntPtr hwnd);
            [DllImport("kernel32.dll")]
            public static extern IntPtr GetModuleHandle(string lpModuleName);
            [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageW")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, ref SETTEXTEX wParam, byte[] lParam);
            [DllImport("kernel32.dll")]
            public static extern uint GetLastError();
            [DllImport("user32.dll", EntryPoint = "PostQuitMessage")]
            public static extern void PostQuitMessage(int nExitCode);
            [DllImport("user32.dll", EntryPoint = "RegisterClassEx")]
            [return: MarshalAs(UnmanagedType.U2)]
            public static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpwcx);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadIcon(IntPtr hInstance, int lpIconName);
            [DllImport("user32.dll", EntryPoint = "LoadCursor")]
            public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr CreateWindowEx(int dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y,
                int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
            [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
            public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
            [DllImport("User32.dll", EntryPoint = "PostMessage")]
            public static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);
            public static readonly WndProc DefWindowProc = _DefWindowProc;
            [DllImport("user32.dll", EntryPoint = "DefWindowProc")]
            private static extern IntPtr _DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        }
        internal sealed class Linux
        {
            private static Task<bool> s_gtkTask;
            public static IPlatformHandle CreateGtkView(IntPtr parentXid)
            {
                if (s_gtkTask == null)
                    s_gtkTask = Gtk.StartGtk();
                if (!s_gtkTask.Result)
                    return null;
                return Glib.RunOnGlibThread(() =>
                {
                    using var title = new Utf8Buffer("Embedded");
                    var widget = Gtk.gtk_file_chooser_dialog_new(title, IntPtr.Zero, GtkFileChooserAction.SelectFolder, IntPtr.Zero);
                    Gtk.gtk_widget_realize(widget);
                    var xid = Gtk.gdk_x11_window_get_xid(Gtk.gtk_widget_get_window(widget));
                    Gtk.gtk_window_present(widget);
                    return new LinuxViewHandle(widget, xid);
                }).Result;
            }
        }
        internal sealed class MacOS
        {

        }
    }
}
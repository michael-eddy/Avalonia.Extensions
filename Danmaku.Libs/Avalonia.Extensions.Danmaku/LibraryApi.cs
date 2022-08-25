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
            [StructLayout(LayoutKind.Sequential)]
            public struct SETTEXTEX
            {
                public uint Flags;
                public uint Codepage;
            }
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
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr CreateWindowEx(int dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y,
                int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
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
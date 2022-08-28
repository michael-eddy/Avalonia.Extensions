using Avalonia.Platform;
using Avalonia.Platform.Interop;
using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Danmaku
{
    public sealed class SystemApi
    {
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
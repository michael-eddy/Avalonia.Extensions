using Avalonia.Controls.Platform;
using System;
using static Avalonia.Extensions.Danmaku.Gtk;
using static Avalonia.Extensions.Danmaku.Glib;

namespace Avalonia.Extensions.Danmaku
{
    sealed class LinuxViewHandle : INativeControlHostDestroyableControlHandle
    {
        private readonly IntPtr _widget;
        public LinuxViewHandle(IntPtr widget, IntPtr xid)
        {
            _widget = widget;
            Handle = xid;
        }
        public IntPtr Handle { get; }
        public string HandleDescriptor => "XID";
        public void Destroy()
        {
            RunOnGlibThread(() =>
            {
                gtk_widget_destroy(_widget);
                return 0;
            }).Wait();
        }
    }
}
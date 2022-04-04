using Avalonia.Platform;
using MonoMac.AppKit;
using System;

namespace Avalonia.Controls.Danmaku
{
    class MacOSViewHandle : IPlatformHandle, IDisposable
    {
        private NSView _view;
        public MacOSViewHandle(NSView view)
        {
            _view = view;
        }
        public IntPtr Handle => _view?.Handle ?? IntPtr.Zero;
        public string HandleDescriptor => "NSView";
        public void Dispose()
        {
            _view.Dispose();
            _view = null;
        }
    }
}
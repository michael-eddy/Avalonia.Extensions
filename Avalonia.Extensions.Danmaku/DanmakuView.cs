using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Platform;
using System;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Danmaku
{
    public class DanmakuView : NativeControlHost
    {
        private IntPtr _pIntPtr;
        private IPlatformHandle? _platformHandle = null;
        /// <summary>
        /// Defines the <see cref="X"/> property.
        /// </summary>
        public static readonly StyledProperty<int> XProperty = AvaloniaProperty.Register<DanmakuView, int>(nameof(X));
        public int X
        {
            get => GetValue(XProperty);
            set => SetValue(XProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="Y"/> property.
        /// </summary>
        public static readonly StyledProperty<int> YProperty = AvaloniaProperty.Register<DanmakuView, int>(nameof(Y));
        public int Y
        {
            get => GetValue(YProperty);
            set => SetValue(YProperty, value);
        }
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            _platformHandle = base.CreateNativeControlCore(parent);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                LibraryApi.Windows.LoadLibrary("Libs\\libwtfdanmaku.dll");
                _pIntPtr = LibraryApi.Windows.CreateWindowEx(0, "WTFWindow_Create", "libwtfdanmaku",
                      0x800000 | 0x10000000 | 0x40000000 | 0x800000 | 0x10000 | 0x0004,
                      X, Y, Width.ToInt32(), Height.ToInt32(),
                      parent.Handle, IntPtr.Zero, LibraryApi.Windows.GetModuleHandle(null), IntPtr.Zero);
                if (_pIntPtr == IntPtr.Zero)
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, "create windows hwnd failed");
                _platformHandle = new PlatformHandle(_pIntPtr, "HWND");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _platformHandle = new MacOSViewHandle(default);
            }
            else
                _platformHandle = LibraryApi.Linux.CreateGtkView(parent.Handle);
            return _platformHandle;
        }
        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            base.DestroyNativeControlCore(control);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                LibraryApi.Windows.DestroyWindow(control.Handle);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                ((MacOSViewHandle)control).Dispose();
            else
            {

            }
            if (_platformHandle != null)
                _platformHandle = null;
        }
        public void Load(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {

            }
            else
            {

            }
        }
    }
}
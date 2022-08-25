using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Platform;
using PCLUntils;
using PCLUntils.Plantform;
using System;
using System.Diagnostics;
using System.IO;

namespace Avalonia.Extensions.Danmaku
{
    public class DanmakuView : NativeControlHost
    {
        private IntPtr _pIntPtr;
        private Process _danmakuPlayer;
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
            try
            {
                switch (PlantformUntils.Platform)
                {
                    case Platforms.Windows:
                        {
                            LibraryApi.Windows.LoadLibrary("Danmaku.Windows.dll");
                            _pIntPtr = LibraryApi.Windows.CreateWindowEx(0, "DanmakuView", "libwtfdanmaku",
                                  0x800000 | 0x10000000 | 0x40000000 | 0x800000 | 0x10000 | 0x0004,
                                  X, Y, Width.ToInt32(), Height.ToInt32(),
                                  parent.Handle, IntPtr.Zero, LibraryApi.Windows.GetModuleHandle(null), IntPtr.Zero);
                            if (_pIntPtr == IntPtr.Zero)
                                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, "create windows hwnd failed");
                            _platformHandle = new PlatformHandle(_pIntPtr, "HWND");
                            break;
                        }
                    case Platforms.MacOS:
                        {
                            _platformHandle = new MacOSViewHandle(default);
                            break;
                        }
                    case Platforms.Linux:
                        {
                            _platformHandle = LibraryApi.Linux.CreateGtkView(parent.Handle);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Visual)?.Log(this, ex.Message);
            }
            return _platformHandle;
        }
        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            try
            {
                switch (PlantformUntils.Platform)
                {
                    case Platforms.MacOS:
                        ((MacOSViewHandle)control).Dispose();
                        break;
                    case Platforms.Windows:
                        LibraryApi.Windows.DestroyWindow(control.Handle);
                        break;
                    case Platforms.Linux:
                        {
                            _danmakuPlayer?.Kill();
                            _danmakuPlayer = null;
                            base.DestroyNativeControlCore(control);
                            break;
                        }
                    default:
                        base.DestroyNativeControlCore(control);
                        break;
                }
                if (_platformHandle != null)
                    _platformHandle = null;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Visual)?.Log(this, ex.Message);
            }
            if (_platformHandle != null)
                _platformHandle = null;
        }
        public void Load(string path)
        {
            try
            {
                switch (PlantformUntils.Platform)
                {
                    case Platforms.MacOS:
                        {
                            break;
                        }
                    case Platforms.Windows:
                        {
                            break;
                        }
                    case Platforms.Linux:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
    }
}
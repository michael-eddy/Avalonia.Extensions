using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Platform;
using PCLUntils;
using PCLUntils.Plantform;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static Avalonia.Extensions.Danmaku.LibraryApi.Windows;

namespace Avalonia.Extensions.Danmaku
{
    public class DanmakuView : NativeControlHost
    {
        private IntPtr winPtr;
        private Process _danmakuPlayer;
        private static bool init = false;
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
                            string path = Path.Combine(Environment.CurrentDirectory, "Danmaku.Windows.exe");
                            LoadLibrary(path);
                            IntPtr hInstance = GetModuleHandle(null);
                            WNDCLASSEX wc = new WNDCLASSEX
                            {
                                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
                                style = 0,
                                lpfnWndProc = WindowProc,
                                cbClsExtra = 0,
                                cbWndExtra = 0,
                                hInstance = hInstance,
                                hIcon = LoadIcon(IntPtr.Zero, IDI_APPLICATION),
                                hCursor = LoadCursor(IntPtr.Zero, IDC_ARROW),
                                hbrBackground = (IntPtr)(COLOR_WINDOW + 1),
                                lpszMenuName = "DanmakuView",
                                lpszClassName = "DanmakuView",
                                hIconSm = LoadIcon(IntPtr.Zero, IDI_APPLICATION)
                            };
                            var windowClass = RegisterClassEx(ref wc);
                            if (windowClass == 0)
                                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, "RegisterClassEx failed.");
                            else
                            {
                                winPtr = CreateWindowEx(0, "DanmakuView", null,
                                      0x800000 | 0x10000000 | 0x40000000 | 0x800000 | 0x10000 | 0x0004, X, Y, Width.ToInt32(), Height.ToInt32(),
                                      parent.Handle, IntPtr.Zero, hInstance, IntPtr.Zero);
                                if (winPtr == IntPtr.Zero)
                                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, "create windows hwnd failed");
                                else
                                {
                                    var code = ShowWindow(winPtr, 5);
                                }
                            }
                            _platformHandle = new PlatformHandle(winPtr, "HWND");
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
        private readonly WndProc WindowProc = _WindowProc;
        private static IntPtr _WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_CLOSE:
                    DestroyWindow(hWnd);
                    break;
                case WM_DESTROY:
                    PostQuitMessage(0);
                    break;
                default:
                    {
                        try
                        {
                            return DefWindowProc(hWnd, msg, wParam, lParam);
                        }
                        finally
                        {
                            if (!init)
                            {
                                init = true;
                                COPYDATASTRUCT data = new COPYDATASTRUCT();
                                PostMessage(hWnd, 0x03, 0, ref data);
                            }
                        }
                    }
            }
            return IntPtr.Zero;
        }
        public void Play()
        {
            try
            {
                COPYDATASTRUCT data = new COPYDATASTRUCT();
                PostMessage(winPtr, 0x02, 0, ref data);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Animations)?.Log(this, ex.Message);
            }
        }
        public void Pause()
        {
            try
            {
                COPYDATASTRUCT data = new COPYDATASTRUCT();
                PostMessage(winPtr, 0x01, 0, ref data);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Animations)?.Log(this, ex.Message);
            }
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
                        DestroyWindow(control.Handle);
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
    }
}
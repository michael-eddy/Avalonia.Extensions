using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Platform;
using PCLUntils;
using PCLUntils.Plantform;
using System;
using System.Diagnostics;
using System.Text;
using static Avalonia.Extensions.Danmaku.SystemApi.Windows;

namespace Avalonia.Extensions.Danmaku
{
    public class DanmakuView : NativeControlHost
    {
        private IntPtr window;
        private Process _danmakuPlayer;
        private IntPtr wtf = IntPtr.Zero;
        private static bool wtfInited = false;
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
                            var handle = GetModuleHandle(null);
                            window = LibLoader.WTFWindow_Create(handle, 1);
                            LibLoader.WTFWindow_SetCustomWndProc(window, MyWndProc);
                            LibLoader.WTFWindow_Initialize(window, WS_EX_NOREDIRECTIONBITMAP, 1280, 720, "Danmaku");
                            LibLoader.WTFWindow_SetHitTestOverEnabled(window, 0);
                            LibLoader.WTFWindow_RunMessageLoop(window);
                            _platformHandle = new PlatformHandle(window, "HWND");
                            break;
                        }
                    case Platforms.MacOS:
                        {
                            _platformHandle = new MacOSViewHandle(default);
                            break;
                        }
                    case Platforms.Linux:
                        {
                            _platformHandle = SystemApi.Linux.CreateGtkView(parent.Handle);
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
        private IntPtr MyWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_SIZE:
                    if ((wParam.ToInt64() == SIZE_RESTORED || wParam.ToInt64() == SIZE_MAXIMIZED) && wtf != IntPtr.Zero)
                        LibLoader.WTF_Resize(wtf, (uint)(lParam.ToInt32() & 0xffff), (uint)(lParam.ToInt32() >> 16) & 0xffff);
                    break;
                case WM_LBUTTONDOWN:
                    if (!wtfInited)
                    {
                        wtf = LibLoader.WTF_CreateInstance();
                        LibLoader.WTF_InitializeWithHwnd(wtf, hWnd);
                        LibLoader.WTF_SetFontName(wtf, "SimHei");
                        LibLoader.WTF_SetFontWeight(wtf, 700);
                        LibLoader.WTF_SetFontScaleFactor(wtf, 1.0f);
                        LibLoader.WTF_SetDanmakuStyle(wtf, WTF_DANMAKU_STYLE_OUTLINE);
                        LibLoader.WTF_SetCompositionOpacity(wtf, 0.9f);
                        //LibLoader.WTF_SetDanmakuTypeVisibility(wtf, WTF_DANMAKU_TYPE_SCROLLING_VISIBLE | WTF_DANMAKU_TYPE_TOP_VISIBLE | WTF_DANMAKU_TYPE_BOTTOM_VISIBLE);
                        LibLoader.WTF_LoadBilibiliFile(wtf, Encoding.ASCII.GetBytes("D:\\200887808.xml"));
                    }
                    wtfInited = true;
                    LibLoader.WTF_Start(wtf);
                    break;
                case WM_RBUTTONDOWN:
                    if (wtf != IntPtr.Zero)
                        LibLoader.WTF_Pause(wtf);
                    break;
                case WM_DESTROY:
                    if (wtf != IntPtr.Zero)
                    {
                        if (LibLoader.WTF_IsRunning(wtf) != 0)
                            LibLoader.WTF_Stop(wtf);
                        LibLoader.WTF_ReleaseInstance(wtf);
                        wtf = IntPtr.Zero;
                        wtfInited = false;
                    }
                    break;
                case WM_DPICHANGED:
                    {
                        uint dpi = (uint)(wParam.ToInt32() & 0xffff);
                        if (wtf != null && LibLoader.WTF_IsRunning(wtf) != 0)
                            LibLoader.WTF_SetDpi(wtf, dpi, dpi);
                        break;
                    }
            }
            return LibLoader.WTFWindow_DefaultWindowProc(window, hWnd, msg, wParam, lParam);
        }
        public void Play()
        {
            try
            {
                COPYDATASTRUCT data = new COPYDATASTRUCT();
                PostMessage(window, 0x02, 0, ref data);
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
                PostMessage(window, 0x01, 0, ref data);
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
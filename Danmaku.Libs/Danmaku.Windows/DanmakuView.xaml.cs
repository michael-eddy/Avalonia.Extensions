using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using static Danmaku.WINAPI.USER32;

namespace Danmaku
{
    public partial class DanmakuView : Window, IDanmakuWindow
    {
        private IntPtr _wtf;
        public DanmakuView()
        {
            InitializeComponent();
        }
        ~DanmakuView()
        {
            (this as IDanmakuWindow).Dispose();
        }
        private void WtfDanmakuWindow_Load(object sender, EventArgs e)
        {
            ShowInTaskbar = false;
            Topmost = true;
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            var exStyles = GetExtendedWindowStyles(hwnd);
            SetExtendedWindowStyles(hwnd, exStyles | ExtendedWindowStyles.Layered | ExtendedWindowStyles.Transparent | ExtendedWindowStyles.ToolWindow);
            var hs = PresentationSource.FromVisual(this) as HwndSource;
            hs.AddHook(new HwndSourceHook(WndProc));
            CreateWTF();
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((HwndMsg)msg)
            {
                case HwndMsg.Pause:
                    LibLoader.WTF_Pause(_wtf);
                    break;
                case HwndMsg.Play:
                    LibLoader.WTF_Start(_wtf);
                    break;
            }
            return IntPtr.Zero;
        }
        private void CreateWTF()
        {
            _wtf = LibLoader.WTF_CreateInstance();
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            LibLoader.WTF_InitializeWithHwnd(_wtf, hwnd);
            LibLoader.WTF_SetFontName(_wtf, "SimHei");
            LibLoader.WTF_SetFontScaleFactor(_wtf, 1.4f);
            LibLoader.WTF_SetCompositionOpacity(_wtf, 0.85f);
        }
        private void DestroyWTF()
        {
            if (_wtf != IntPtr.Zero)
            {
                if (LibLoader.WTF_IsRunning(_wtf) != 0)
                    LibLoader.WTF_Stop(_wtf);
                LibLoader.WTF_Terminate(_wtf);
                LibLoader.WTF_ReleaseInstance(_wtf);
                _wtf = IntPtr.Zero;
            }
        }
        void IDanmakuWindow.Show()
        {
            Show();
            LibLoader.WTF_Start(_wtf);
        }
        void IDisposable.Dispose()
        {
            if (_wtf != IntPtr.Zero)
                DestroyWTF();
        }
        void IDanmakuWindow.Close() => Close();
        void IDanmakuWindow.AddDanmaku(DanmakuType type, string comment, uint color) =>
            LibLoader.WTF_AddDanmaku(_wtf, (int)type, 0, comment, 25, (int)color, 0, 0);
        void IDanmakuWindow.AddLiveDanmaku(DanmakuType type, string comment, uint color) =>
            LibLoader.WTF_AddLiveDanmaku(_wtf, (int)type, 0, comment, 25, (int)color, 0, 0);
        void IDanmakuWindow.OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_wtf != IntPtr.Zero)
                LibLoader.WTF_SetFontScaleFactor(_wtf, 1.4f);
        }
        private void Window_Closing(object sender, CancelEventArgs e) => DestroyWTF();
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_wtf != IntPtr.Zero)
                LibLoader.WTF_Resize(_wtf, (uint)e.NewSize.Width, (uint)e.NewSize.Height);
        }
    }
}
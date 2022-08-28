using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Platform;
using PCLUntils;
using PCLUntils.Plantform;
using System;
using System.Threading;

namespace Avalonia.Extensions.Danmaku
{
    public partial class DanmakuView : NativeControlHost
    {
        private IntPtr wtf = IntPtr.Zero;
        private IPlatformHandle? _platformHandle = null;
        static DanmakuView()
        {
            WidthProperty.Changed.AddClassHandler<DanmakuView>((view, e) => view.Resize(e.NewValue.ToInt32(), view.Height));
            HeightProperty.Changed.AddClassHandler<DanmakuView>((view, e) => view.Resize(view.Width, e.NewValue.ToInt32()));
            BoundsProperty.Changed.AddClassHandler<DanmakuView>((view, e) =>
            {
                if (e.NewValue is Rect rect)
                    view.Resize(rect.Width, rect.Height);
            });
        }
        protected void Resize(double width, double height)
        {
            if (wtf != IntPtr.Zero)
            {
                Thread.Sleep(50);
                LibLoader.WTF_Resize(wtf, (uint)width.ToInt32(), (uint)height.ToInt32());
            }
        }
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
                            wtf = LibLoader.WTF_CreateInstance();
                            LibLoader.WTF_InitializeWithHwnd(wtf, parent.Handle);
                            LibLoader.WTF_SetFontName(wtf, "SimHei");
                            LibLoader.WTF_SetFontWeight(wtf, 700);
                            LibLoader.WTF_SetFontScaleFactor(wtf, 1.0f);
                            LibLoader.WTF_SetDanmakuStyle(wtf, 1);
                            LibLoader.WTF_SetCompositionOpacity(wtf, 0.9f);
                            _platformHandle = new PlatformHandle(wtf, "HWND");
                            break;
                        }
                    default:
                        Logger.TryGet(LogEventLevel.Error, LogArea.Visual)?.Log(this, "now it just only support windows!");
                        break;
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
                    case Platforms.Windows:
                        DestoryWindows();
                        base.DestroyNativeControlCore(control);
                        break;
                    default:
                        base.DestroyNativeControlCore(control);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Visual)?.Log(this, ex.Message);
            }
            finally
            {
                _platformHandle = null;
            }
        }
    }
}
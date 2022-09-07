using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Logging;
using Avalonia.Platform;
using PCLUntils;
using PCLUntils.Plantform;
using System;
using System.Net.Http;
using System.Threading;

namespace Avalonia.Extensions.Danmaku
{
    public partial class DanmakuView : NativeControlHost, IDanmakuView
    {
        private IntPtr wtf = IntPtr.Zero;
        private readonly HttpClient httpClient;
        private IPlatformHandle? _platformHandle = null;
        static DanmakuView()
        {
            WidthProperty.Changed.AddClassHandler<DanmakuView>(OnWidthChange);
            HeightProperty.Changed.AddClassHandler<DanmakuView>(OnHeightChange);
            BoundsProperty.Changed.AddClassHandler<DanmakuView>(OnBoundsChange);
            FontScaleProperty.Changed.AddClassHandler<DanmakuView>(OnFontScaleChange);
            FontNameProperty.Changed.AddClassHandler<DanmakuView>(OnFontNameChange);
            FontWeightProperty.Changed.AddClassHandler<DanmakuView>(OnFontWeightChange);
            CompositionOpacityProperty.Changed.AddClassHandler<DanmakuView>(OnCompositionOpacity);
        }
        public DanmakuView()
        {
            httpClient = Core.Instance.GetClient();
        }
        private void Resize(double width, double height)
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
        public static readonly StyledProperty<DanmakuStyle> StyleProperty = AvaloniaProperty.Register<DanmakuView, DanmakuStyle>(nameof(Style), DanmakuStyle.OutLine);
        public DanmakuStyle Style
        {
            get => GetValue(StyleProperty);
            set => SetValue(StyleProperty, value);
        }
        public static readonly StyledProperty<string> FontNameProperty = AvaloniaProperty.Register<DanmakuView, string>(nameof(FontName), "SimHei");
        public string FontName
        {
            get => GetValue(FontNameProperty);
            set => SetValue(FontNameProperty, value);
        }
        public static readonly StyledProperty<int> FontWeightProperty = AvaloniaProperty.Register<DanmakuView, int>(nameof(FontWeight), 700);
        public int FontWeight
        {
            get => GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }
        public static readonly StyledProperty<float> FontScaleProperty = AvaloniaProperty.Register<DanmakuView, float>(nameof(FontScale), 1f);
        public float FontScale
        {
            get => GetValue(FontScaleProperty);
            set => SetValue(FontScaleProperty, value);
        }
        public static readonly StyledProperty<float> CompositionOpacityProperty = AvaloniaProperty.Register<DanmakuView, float>(nameof(CompositionOpacity), .9f);
        public float CompositionOpacity
        {
            get => GetValue(CompositionOpacityProperty);
            set => SetValue(CompositionOpacityProperty, value);
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
                            Init();
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
        private void Init()
        {
            LibLoader.WTF_SetFontName(wtf, FontName);
            LibLoader.WTF_SetFontWeight(wtf, FontWeight);
            LibLoader.WTF_SetFontScaleFactor(wtf, FontScale);
            LibLoader.WTF_SetDanmakuStyle(wtf, (int)Style);
            LibLoader.WTF_SetCompositionOpacity(wtf, CompositionOpacity);
            Load();
        }
        private void ReInit()
        {
            switch (PlantformUntils.Platform)
            {
                case Platforms.Windows:
                    Destory();
                    Init();
                    break;
            }
        }
        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            try
            {
                switch (PlantformUntils.Platform)
                {
                    case Platforms.Windows:
                        Destory();
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
        private static void OnHeightChange(DanmakuView view, AvaloniaPropertyChangedEventArgs e) => view.Resize(view.Width, e.NewValue.ToInt32());
        private static void OnWidthChange(DanmakuView view, AvaloniaPropertyChangedEventArgs e) => view.Resize(e.NewValue.ToInt32(), view.Height);
        private static void OnBoundsChange(DanmakuView view, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is Rect rect)
                view.Resize(rect.Width, rect.Height);
        }
        private static void OnFontScaleChange(DanmakuView view, AvaloniaPropertyChangedEventArgs e) => view.ReInit();
        private static void OnFontNameChange(DanmakuView view, AvaloniaPropertyChangedEventArgs e) => view.ReInit();
        private static void OnFontWeightChange(DanmakuView view, AvaloniaPropertyChangedEventArgs e) => view.ReInit();
        private static void OnCompositionOpacity(DanmakuView view, AvaloniaPropertyChangedEventArgs e) => view.ReInit();
    }
}
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Extensions.Styles;
using Avalonia.Input;
using Avalonia.Styling;
using PCLUntils;
using PCLUntils.Plantform;
using System;

namespace Avalonia.Extensions.Controls
{
    public class AeroWindow : WindowBase, IStyling
    {
        private double moveWidth = 0;
        Type IStyleable.StyleKey => typeof(Window);
        static AeroWindow()
        {
            BoundsProperty.Changed.AddClassHandler<AeroWindow>(OnBoundsChange);
        }
        private static void OnBoundsChange(AeroWindow sender, AvaloniaPropertyChangedEventArgs e)
            => sender.moveWidth = sender.Bounds.Width - 148;
        public AeroWindow()
        {
            ExtendClientAreaToDecorationsHint = true;
            ExtendClientAreaTitleBarHeightHint = -1;
            TransparencyLevelHint = new[] { WindowTransparencyLevel.AcrylicBlur };
            this.GetObservable(WindowStateProperty)
                .Subscribe(x =>
                {
                    PseudoClasses.Set(":maximized", x == WindowState.Maximized);
                    PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
                });
            this.GetObservable(IsExtendedIntoWindowDecorationsProperty)
                .Subscribe(x =>
                {
                    if (!x)
                    {
                        SystemDecorations = SystemDecorations.Full;
                        TransparencyLevelHint = new[] { WindowTransparencyLevel.Blur };
                    }
                });
        }
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            ExtendClientAreaChromeHints = Platform.ExtendClientAreaChromeHints.PreferSystemChrome
                | Platform.ExtendClientAreaChromeHints.OSXThickTitleBar;
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Icon = default;
            Background = default;
        }
        protected virtual bool MoveDragEnable => false;
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (PlantformUntils.System == Platforms.Windows)
            {
                var point = e.GetCurrentPoint(this);
                if (MoveDragEnable && point.Properties.IsLeftButtonPressed &&
                    (point.Position.Y > 30 || (0 > point.Position.X && point.Position.X < moveWidth)))
                    BeginMoveDrag(e);
            }
        }
    }
}
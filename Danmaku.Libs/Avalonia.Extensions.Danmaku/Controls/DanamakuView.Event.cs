namespace Avalonia.Extensions.Danmaku
{
    public partial class DanmakuView
    {
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
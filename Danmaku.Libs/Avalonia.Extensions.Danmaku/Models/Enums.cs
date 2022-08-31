namespace Avalonia.Extensions.Danmaku
{
    public enum DanmakuStyle : int
    {
        OutLine = 1,
        Projection = 2
    }
    public enum DanmakuType : int
    {
        Scrolling = 1,
        Bottom = 4,
        Top = 5,
        Reserve = 6,
        Position = 7,
        Advanced = 8
    }
    public enum DanmakuTypeVisable : int
    {
        Scroll = 1,
        Bottom = 2,
        Top = 4,
        Reserve = 8,
        Position = 16,
        Adavanced = 32
    }
    public enum DanmakuLocation
    {
        /// <summary>
        /// 滚动弹幕Model1-3
        /// </summary>
        Roll = -1,
        /// <summary>
        /// 顶部弹幕Model5
        /// </summary>
        Top = 0,
        /// <summary>
        /// 底部弹幕Model4
        /// </summary>
        Bottom = 1,
        /// <summary>
        /// 定位弹幕Model7
        /// </summary>
        Position = 2,
        /// <summary>
        /// 其它暂未支持的类型
        /// </summary>
        Other = 3
    }
}
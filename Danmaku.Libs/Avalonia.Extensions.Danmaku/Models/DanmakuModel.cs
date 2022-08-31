using Avalonia.Media;

namespace Avalonia.Extensions.Danmaku
{
    public sealed class DanmakuModel
    {
        public string Text { get; set; }
        /// <summary>
        /// 弹幕大小
        /// </summary>
        public double Size { get; set; }
        /// <summary>
        /// 弹幕颜色
        /// </summary>
        public SolidColorBrush Color { get; set; }
        /// <summary>
        /// 弹幕出现时间
        /// </summary>
        public double Time { get; set; }
        /// <summary>
        /// 弹幕发送时间
        /// </summary>
        public string SendTime { get; set; }
        /// <summary>
        /// 弹幕池
        /// </summary>
        public string Pool { get; set; }
        /// <summary>
        /// 弹幕发送人ID
        /// </summary>
        public string SendID { get; set; }
        /// <summary>
        /// 弹幕ID
        /// </summary>
        public string RowID { get; set; }
        /// <summary>
        /// 弹幕出现位置
        /// </summary>
        public DanmakuLocation Location { get; set; }
    }
}
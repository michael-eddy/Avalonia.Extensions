using System.Collections.Generic;

namespace Avalonia.Extensions.Chinese.PinYin
{
    public class PingYinModel
    {
        public PingYinModel()
        {
            TotalPingYin = new List<string>();
            FirstPingYin = new List<string>();
            ListPinYin = new List<string>();
        }
        /// <summary>
        /// 全拼
        /// </summary>
        public List<string> TotalPingYin { get; set; }
        /// <summary>
        /// 首拼
        /// </summary>
        public List<string> FirstPingYin { get; set; }
        /// <summary>
        /// 列表拼音
        /// </summary>
        public List<string> ListPinYin { get; set; }
    }
}
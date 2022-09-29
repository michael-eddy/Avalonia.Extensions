using Avalonia.Logging;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace Avalonia.Extensions.Danmaku
{
    public sealed class DanmakuParser
    {
        private static readonly DanmakuParser instance = new DanmakuParser();
        public static DanmakuParser Instance => instance;
        private DanmakuParser() { }
        public List<DanmakuModel> ParseBiliBiliXml(string xmlStr)
        {
            List<DanmakuModel> danmakus = new List<DanmakuModel>();
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xmlStr = Regex.Replace(xmlStr, @"[\x00-\x08]|[\x0B-\x0C]|[\x0E-\x1F]", "");
                xdoc.LoadXml(xmlStr);
                foreach (XmlNode item in xdoc.DocumentElement.ChildNodes)
                {
                    if (item.Attributes["p"] != null)
                    {
                        try
                        {
                            string node = item.Attributes["p"]?.Value;
                            if (!string.IsNullOrEmpty(node))
                            {
                                string[] danmaku = node.Split(',');
                                var location = danmaku[1] switch
                                {
                                    "7" => DanmakuLocation.Position,
                                    "4" => DanmakuLocation.Bottom,
                                    "5" => DanmakuLocation.Top,
                                    _ => DanmakuLocation.Roll,
                                };
                                if (double.TryParse(danmaku[0], out double time) && double.TryParse(danmaku[2], out double size))
                                {
                                    danmakus.Add(new DanmakuModel
                                    {
                                        Time = time,
                                        Location = location,
                                        Size = size,
                                        Color = new SolidColorBrush(danmaku[3].ToColor(true)),
                                        SendTime = danmaku[4],
                                        Pool = danmaku[5],
                                        SendID = danmaku[6],
                                        RowID = danmaku[7],
                                        Text = item.InnerText
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, $"ParseBiliBiliXml:{ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return danmakus;
        }
    }
}
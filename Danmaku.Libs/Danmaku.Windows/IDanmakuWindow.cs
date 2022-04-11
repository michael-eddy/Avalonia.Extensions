using System;
using System.ComponentModel;

namespace Danmaku
{
    public enum HwndMsg : int
    {
        Pause = 0x01,
        Play = 0x02
    }
    interface IDanmakuWindow : IDisposable
    {
        void Show();
        void Close();
        void AddDanmaku(DanmakuType type, string comment, uint color);
        void AddLiveDanmaku(DanmakuType type, string comment, uint color);
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
    }
}
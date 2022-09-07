using System;
using System.Text;

namespace Avalonia.Extensions.Danmaku
{
    public interface IDanmakuView
    {
        void Start();
        void SeekTo(long position);
        void Stop();
        void Load(Uri uri);
        void Load(string xml, Encoding encoding);
        void Destory();
    }
}
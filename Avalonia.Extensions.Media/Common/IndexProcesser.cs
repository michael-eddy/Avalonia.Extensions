using Avalonia.Media;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.Extensions.Media
{
    internal sealed class IndexProcesser
    {
        public int Index { get; private set; } = -1;
        public List<MusicPlayItem> PlayUrls { get; }
        public IndexProcesser()
        {
            PlayUrls = new List<MusicPlayItem>();
        }
        public void Add((string, string) playInfo)
        {
            var item = new MusicPlayItem(PlayUrls.Count, playInfo);
            PlayUrls.Add(item);
        }
        public bool Switch(int index, out string url)
        {
            url = string.Empty;
            try
            {
                if (index > -1 && index + 1 < PlayUrls.Count)
                {
                    var item = PlayUrls.FirstOrDefault(x => x.Index == Index);
                    item.Color = new SolidColorBrush(Colors.WhiteSmoke);
                    Index = index;
                    item = PlayUrls.FirstOrDefault(x => x.Index == Index);
                    if (item != null && !string.IsNullOrEmpty(item.Url))
                    {
                        url = item.Url;
                        item.Color = new SolidColorBrush(Colors.DarkCyan);
                        return true;
                    }
                    else
                    {
                        url = string.Empty;
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool GetUrl(out string url)
        {
            try
            {
                MusicPlayItem item;
                if (Index > -1)
                {
                    item = PlayUrls.FirstOrDefault(x => x.Index == Index);
                    item.Color = new SolidColorBrush(Colors.WhiteSmoke);
                }
                if (Index + 1 < PlayUrls.Count)
                {
                    Index++;
                    item = PlayUrls.FirstOrDefault(x => x.Index == Index);
                    if (item != null && !string.IsNullOrEmpty(item.Url))
                    {
                        url = item.Url;
                        item.Color = new SolidColorBrush(Colors.DarkCyan);
                        return true;
                    }
                    else
                    {
                        url = string.Empty;
                        return false;
                    }
                }
                else
                {
                    url = string.Empty;
                    return false;
                }
            }
            catch
            {
                url = string.Empty;
                return false;
            }
        }
        public bool Prev(out string url)
        {
            MusicPlayItem item;
            if (Index > -1)
            {
                item = PlayUrls.FirstOrDefault(x => x.Index == Index);
                item.Color = new SolidColorBrush(Colors.WhiteSmoke);
            }
            if (Index - 1 > -1)
            {
                Index--;
                item = PlayUrls.FirstOrDefault(x => x.Index == Index);
                if (item != null && !string.IsNullOrEmpty(item.Url))
                {
                    url = item.Url;
                    item.Color = new SolidColorBrush(Colors.DarkCyan);
                    return true;
                }
                else
                {
                    url = string.Empty;
                    return false;
                }
            }
            else
            {
                url = string.Empty;
                return false;
            }
        }
    }
}
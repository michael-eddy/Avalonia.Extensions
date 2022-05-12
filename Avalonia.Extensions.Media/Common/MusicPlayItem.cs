using Avalonia.Media;

namespace Avalonia.Extensions.Media
{
    public sealed class MusicPlayItem
    {
        public MusicPlayItem(int idx, (string Name, string Url) playInfo)
        {
            Index = idx;
            Url = playInfo.Url;
            Name = playInfo.Name;
        }
        public int Index { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public SolidColorBrush Color { get; set; }
    }
}
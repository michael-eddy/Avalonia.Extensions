using System.Collections.Generic;

namespace Avalonia.Extensions.Media
{
    public sealed class MusicPlayerOptions
    {
        public PixelPoint? Position { get; set; }
        public bool EnableDebugLogs { get; set; } = true;
        public Dictionary<string, string> Headers { get; set; }
        public (string UserAgentName, string UserAgentValue)? UserAgent { get; set; }
    }
}
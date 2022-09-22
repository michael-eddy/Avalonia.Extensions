using System.Collections.Generic;

namespace Avalonia.Extensions.WebView.Models
{
    internal sealed class VersionData
    {
        public VersionsAttr linux32 { get; set; }
        public VersionsAttr linux64 { get; set; }
        public VersionsAttr linuxarm { get; set; }
        public VersionsAttr linuxarm64 { get; set; }
        public VersionsAttr macosarm64 { get; set; }
        public VersionsAttr macosx64 { get; set; }
        public VersionsAttr windows32 { get; set; }
        public VersionsAttr windows64 { get; set; }
        public VersionsAttr windowsarm64 { get; set; }
    }
    internal sealed class VersionsAttr
    {
        public List<VersionInfo> versions { get; set; }
    }
    internal sealed class VersionInfo
    {
        public string cef_version { get; set; }
        public string channel { get; set; }
        public string chromium_version { get; set; }
        public List<VersionFileInfo> files { get; set; }
    }
    internal sealed class VersionFileInfo
    {
        public string last_modified { get; set; }
        public string name { get; set; }
        public string sha1 { get; set; }
        public string size { get; set; }
        public string type { get; set; }
    }
}
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;

namespace Avalonia.Extensions.Media
{
    public sealed class MediaItem : LibVLCSharp.Shared.Media
    {
        public MediaItem(MediaList mediaList) : base(mediaList) { }
        public MediaItem(LibVLC libVLC, Uri uri, params string[] options) : base(libVLC, uri, options) { }
        public MediaItem(LibVLC libVLC, string mrl, FromType type = FromType.FromPath, params string[] options) : base(libVLC, mrl, type, options) { }
        public void SetHeaders(Dictionary<string, string> headers)
        {
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    var option = $"{(!header.Key.Trim().StartsWith(":") ? string.Empty : ":")}{header.Key}={header.Value}";
                    AddOption(option);
                }
            }
        }
    }
}
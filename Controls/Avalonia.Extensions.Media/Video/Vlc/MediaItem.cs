using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;

namespace Avalonia.Extensions.Media
{
    public class MediaItem : LibVLCSharp.Shared.Media
    {
        public MediaItem(MediaList mediaList) : base(mediaList) { }
        public MediaItem(LibVLC libVLC, Uri uri, params string[] options) : base(libVLC, uri, options) { }
        public MediaItem(LibVLC libVLC, string mrl, FromType type = FromType.FromPath, params string[] options) : base(libVLC, mrl, type, options) { }
        public void SetHeader(Dictionary<string, string> headers)
        {
            try
            {
                foreach (var header in headers)
                    AddOption($":{header.Key}={header.Value}");
            }
            catch { }
        }
    }
}
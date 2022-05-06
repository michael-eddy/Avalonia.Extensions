using LibVLCSharp.Shared;
using System;

namespace Avalonia.Extensions
{
    public class MediaItem : LibVLCSharp.Shared.Media
    {
        public MediaItem(MediaList mediaList) : base(mediaList) { }
        public MediaItem(LibVLC libVLC, Uri uri, params string[] options) : base(libVLC, uri, options) { }
        public MediaItem(LibVLC libVLC, int fd, params string[] options) : base(libVLC, fd, options) { }
        public MediaItem(LibVLC libVLC, MediaInput input, params string[] options) : base(libVLC, input, options) { }
        public MediaItem(LibVLC libVLC, string mrl, FromType type = FromType.FromPath, params string[] options) : base(libVLC, mrl, type, options) { }
    }
}
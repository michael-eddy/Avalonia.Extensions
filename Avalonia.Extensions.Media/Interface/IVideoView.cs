using LibVLCSharp.Shared;
using System;

namespace Avalonia.Extensions.Media
{
    public interface IVideoView
    {
        bool Play(MediaItem media);
        bool Play(MediaList medias);
        bool Play(Uri uri, params string[] options);
        bool Play(string mrl, FromType type, params string[] options);
    }
}
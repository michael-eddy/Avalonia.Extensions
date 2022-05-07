using Avalonia.Controls;
using LibVLCSharp.Shared;
using System;

namespace Avalonia.Extensions.Media
{
    public interface IPlayerView
    {
        bool Play();
        bool Play(MediaItem media);
        bool Play(MediaList medias);
        bool Play(Uri uri, params string[] options);
        bool Play(string mrl, FromType type, params string[] options);
        bool Pause();
        bool Stop();
        Panel InitLatout();
    }
}
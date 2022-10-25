using Avalonia.Controls;

namespace Avalonia.Extensions.Media
{
    public interface IVideoView
    {
        bool Play();
        bool Pause();
        bool Stop();
    }
    public interface IPlayerView : IVideoView
    {
        IControl InitLatout();
    }
}
using Avalonia.Controls;

namespace Avalonia.Extensions.Media
{
    public interface IPlayerView
    {
        bool Play();
        bool Pause();
        bool Stop();
        Panel InitLatout();
    }
}
using Avalonia.Controls;

namespace Avalonia.Extensions.Media
{
    public interface IVideoView : IPlayerView
    {
        Control InitLatout();
    }
}
using Avalonia.Controls;

namespace Avalonia.Extensions.Media
{
    public interface IRun : IControl
    {
        Runs Children { get; }
    }
}
namespace Avalonia.Extensions.Media
{
    public enum MediaState
    {
        None,
        Read,
        Play,
        Pause,
    }
    public enum PlayStatus : int
    {
        Playing = 0,
        Pause = 1,
        Stop = 2,
        Buffering = 3,
        Buffered = 4,
        Error = 5
    }
}
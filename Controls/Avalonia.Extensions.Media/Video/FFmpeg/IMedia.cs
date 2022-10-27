using System;

namespace Avalonia.Extensions.Media
{
    public interface IMedia
    {
        public delegate void MediaHandler(TimeSpan duration);
        public event MediaHandler MediaCompleted;
    }
}
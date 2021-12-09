namespace Avalonia.Extensions.Controls
{
    public sealed class NotifyOptions
    {
        public NotifyOptions(ShowPosition position)
        {
            Position = position;
            Size = new Size(160, 60);
        }
        public NotifyOptions(ShowPosition position, Size size)
        {
            Size = size;
            Position = position;
        }
        public NotifyOptions(ShowPosition position, Size size, ScollOrientation scollOrientation) : this(position, size)
        {
            ScollOrientation = scollOrientation;
        }
        public NotifyOptions(ShowPosition position, Size size, ScollOrientation scollOrientation, int movePixel) : this(position, size, scollOrientation)
        {
            MovePixel = movePixel;
        }
        public NotifyOptions(ShowPosition position, Size size, ScollOrientation scollOrientation, int movePixel, int moveDelay) : this(position, size, scollOrientation, movePixel)
        {
            MoveDelay = moveDelay;
        }
        public Size Size { get; set; }
        public int MovePixel { get; set; } = 1;
        public int MoveDelay { get; set; } = 20;
        public ShowPosition Position { get; set; }
        public ScollOrientation ScollOrientation { get; set; }
        public bool IsVaidate => !((Position == ShowPosition.TopLeft || Position == ShowPosition.TopRight)
                    && ScollOrientation == ScollOrientation.Vertical);
        public void Update(NotifyOptions options)
        {
            Size = options.Size;
            Position = options.Position;
            MovePixel = options.MovePixel;
            MoveDelay = options.MoveDelay;
            ScollOrientation = options.ScollOrientation;
        }
    }
}
namespace Avalonia.Extensions.Controls
{
    public enum ScreenType
    {
        Monitor = 0,
        WorkArea = 1
    }
    internal enum CharClass
    {
        CharClassUnknown,
        CharClassWhitespace,
        CharClassAlphaNumeric,
    }
    public enum MessageBoxButtons
    {
        Ok,
        OkNo
    }
    public enum ShowPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
    public enum ScollOrientation
    {
        Vertical,
        Horizontal
    }
    public enum ExpandStatus
    {
        Expanded,
        Collapsed
    }
    public enum PopupLength
    {
        Short = 2000,
        Default = 5000,
        Long = 8000
    }
    public enum OS
    {
        Linux,
        OSX,
        Windows,
        Android,
        Unknow
    }
}
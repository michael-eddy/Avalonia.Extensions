using System;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Model
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public readonly struct NativeRectangle
    {
        public int Left { get; }
        public int Top { get; }
        public int Right { get; }
        public int Bottom { get; }
        public NativeRectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class NativeMonitorInfo
    {
        public int Size = Marshal.SizeOf(typeof(NativeMonitorInfo));
        public NativeRectangle Monitor;
        public NativeRectangle Work;
        public int Flags;
    }
}
using System;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Danmaku
{
    public sealed class LibLoader
    {
        private const string LibLoaderName = "libwtfdanmaku.dll";
        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr WTF_CreateInstance();
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_ReleaseInstance(IntPtr instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_InitializeWithHwnd(IntPtr instance, IntPtr hwnd);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetDanmakuTypeVisibility(IntPtr instance, int parms);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_LoadBilibiliFile(IntPtr instance, byte[] filePath);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_LoadBilibiliXml(IntPtr instance, ref byte str);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_AddDanmaku(IntPtr instance, int type, long time, string comment, int fontSize, int fontColor, long timestamp, int danmakuId);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_AddLiveDanmaku(IntPtr instance, int type, long time, string comment, int fontSize, int fontColor, long timestamp, int danmakuId);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_Start(IntPtr instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_Pause(IntPtr instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_Resume(IntPtr instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_Stop(IntPtr instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SeekTo(IntPtr instance, long milliseconds);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_Resize(IntPtr instance, uint width, uint height);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetDpi(IntPtr instance, uint dpiX, uint dpiY);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern long WTF_GetCurrentPosition(IntPtr instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern int WTF_IsRunning(IntPtr instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetFontScaleFactor(IntPtr instance, float factor);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetFontName(IntPtr instance, string fontName);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetFontWeight(IntPtr instance, int dwriteFontWeight);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetDanmakuStyle(IntPtr instance, int style);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetCompositionOpacity(IntPtr instance, float opacity);
    }
}
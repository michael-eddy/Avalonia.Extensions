using System;
using System.Runtime.InteropServices;

namespace Danmaku.Wpf
{
    internal sealed class LibLoader
    {
        private const string LibLoaderName = "libwtfdanmaku.dll";
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern WTF_Instance WTF_CreateInstance();
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_ReleaseInstance(WTF_Instance instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_InitializeWithHwnd(WTF_Instance instance, IntPtr hwnd);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern int WTF_InitializeOffscreen(WTF_Instance instance, uint initialWidth, uint initialHeight);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_Terminate(WTF_Instance instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern int WTF_QuerySwapChain(WTF_Instance instance, IntPtr pGuid, IntPtr ppObject);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_LoadBilibiliFile(WTF_Instance instance, ref byte filePath);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_LoadBilibiliFileW(WTF_Instance instance, IntPtr filePath);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_LoadBilibiliXml(WTF_Instance instance, ref byte str);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_AddDanmaku(WTF_Instance instance, int type, long time, IntPtr comment, int fontSize, int fontColor, long timestamp, int danmakuId);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_AddLiveDanmaku(WTF_Instance instance, int type, long time, IntPtr comment, int fontSize, int fontColor, long timestamp, int danmakuId);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_Start(WTF_Instance instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_Pause(WTF_Instance instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_Resume(WTF_Instance instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_Stop(WTF_Instance instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SeekTo(WTF_Instance instance, long milliseconds);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_Resize(WTF_Instance instance, uint width, uint height);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetDpi(WTF_Instance instance, uint dpiX, uint dpiY);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern long WTF_GetCurrentPosition(WTF_Instance instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern int WTF_IsRunning(WTF_Instance instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern float WTF_GetFontScaleFactor(WTF_Instance instance);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetFontScaleFactor(WTF_Instance instance, float factor);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetFontName(WTF_Instance instance, IntPtr fontName);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetFontWeight(WTF_Instance instance, int dwriteFontWeight);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetFontStyle(WTF_Instance instance, int dwriteFontStyle);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetFontStretch(WTF_Instance instance, int dwriteFontStretch);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetDanmakuStyle(WTF_Instance instance, int style);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetCompositionOpacity(WTF_Instance instance, float opacity);
        [DllImport(LibLoaderName, CallingConvention = CallingConvention.StdCall)]
        public static extern void WTF_SetDanmakuTypeVisibility(WTF_Instance instance, int parms);
    }
}
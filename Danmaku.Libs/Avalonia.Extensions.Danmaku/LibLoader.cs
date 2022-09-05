using System;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Danmaku
{
    sealed class LibLoader
    {
        private static readonly DllInvoke dllInvoke;
        static LibLoader()
        {
            var arch = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                _ => throw new NotSupportedException("current arch is not support!"),
            };
            dllInvoke = new DllInvoke($"libwtfdanmaku_{arch}.dll");
        }
        private delegate IntPtr CreateInstance();
        public static IntPtr WTF_CreateInstance() =>
            ((CreateInstance)dllInvoke.Invoke("WTF_CreateInstance", typeof(CreateInstance))).Invoke();
        private delegate void ReleaseInstance(IntPtr instance);
        public static void WTF_ReleaseInstance(IntPtr instance) =>
            ((ReleaseInstance)dllInvoke.Invoke("WTF_ReleaseInstance", typeof(ReleaseInstance))).Invoke(instance);
        private delegate void InitializeWithHwnd(IntPtr instance, IntPtr hwnd);
        public static void WTF_InitializeWithHwnd(IntPtr instance, IntPtr hwnd) =>
            ((InitializeWithHwnd)dllInvoke.Invoke("WTF_InitializeWithHwnd", typeof(InitializeWithHwnd))).Invoke(instance, hwnd);
        private delegate void SetDanmakuTypeVisibility(IntPtr instance, int parms);
        public static void WTF_SetDanmakuTypeVisibility(IntPtr instance, int parms) =>
            ((SetDanmakuTypeVisibility)dllInvoke.Invoke("WTF_SetDanmakuTypeVisibility", typeof(SetDanmakuTypeVisibility))).Invoke(instance, parms);
        private delegate void LoadBilibiliFile(IntPtr instance, byte[] filePath);
        public static void WTF_LoadBilibiliFile(IntPtr instance, byte[] filePath) =>
            ((LoadBilibiliFile)dllInvoke.Invoke("WTF_LoadBilibiliFile", typeof(LoadBilibiliFile))).Invoke(instance, filePath);
        private delegate void LoadBilibiliXml(IntPtr instance, byte[] str);
        public static void WTF_LoadBilibiliXml(IntPtr instance, byte[] str) =>
            ((LoadBilibiliXml)dllInvoke.Invoke("WTF_LoadBilibiliXml", typeof(LoadBilibiliXml))).Invoke(instance, str);
        private delegate void AddDanmaku(IntPtr instance, int type, long time, string comment, int fontSize, int fontColor, long timestamp, int danmakuId);
        public static void WTF_AddDanmaku(IntPtr instance, int type, long time, string comment, int fontSize, int fontColor, long timestamp, int danmakuId) =>
            ((AddDanmaku)dllInvoke.Invoke("WTF_AddDanmaku", typeof(AddDanmaku))).Invoke(instance, type, time, comment, fontSize, fontColor, timestamp, danmakuId);
        private delegate void AddLiveDanmaku(IntPtr instance, int type, long time, string comment, int fontSize, int fontColor, long timestamp, int danmakuId);
        public static void WTF_AddLiveDanmaku(IntPtr instance, int type, long time, string comment, int fontSize, int fontColor, long timestamp, int danmakuId) =>
            ((AddLiveDanmaku)dllInvoke.Invoke("WTF_AddLiveDanmaku", typeof(AddLiveDanmaku))).Invoke(instance, type, time, comment, fontSize, fontColor, timestamp, danmakuId);
        private delegate void Start(IntPtr instance);
        public static void WTF_Start(IntPtr instance) =>
            ((Start)dllInvoke.Invoke("WTF_Start", typeof(Start))).Invoke(instance);
        private delegate void Pause(IntPtr instance);
        public static void WTF_Pause(IntPtr instance) =>
            ((Pause)dllInvoke.Invoke("WTF_Pause", typeof(Pause))).Invoke(instance);
        private delegate void Resume(IntPtr instance);
        public static void WTF_Resume(IntPtr instance) =>
            ((Resume)dllInvoke.Invoke("WTF_Resume", typeof(Resume))).Invoke(instance);
        private delegate void Stop(IntPtr instance);
        public static void WTF_Stop(IntPtr instance) =>
            ((Stop)dllInvoke.Invoke("WTF_Stop", typeof(Stop))).Invoke(instance);
        private delegate void SeekTo(IntPtr instance, long milliseconds);
        public static void WTF_SeekTo(IntPtr instance, long milliseconds) =>
            ((SeekTo)dllInvoke.Invoke("WTF_SeekTo", typeof(SeekTo))).Invoke(instance, milliseconds);
        private delegate void Resize(IntPtr instance, uint width, uint height);
        public static void WTF_Resize(IntPtr instance, uint width, uint height) =>
            ((Resize)dllInvoke.Invoke("WTF_Resize", typeof(Resize))).Invoke(instance, width, height);
        private delegate void SetDpi(IntPtr instance, uint dpiX, uint dpiY);
        public static void WTF_SetDpi(IntPtr instance, uint dpiX, uint dpiY) =>
            ((SetDpi)dllInvoke.Invoke("WTF_SetDpi", typeof(SetDpi))).Invoke(instance, dpiX, dpiY);
        private delegate long GetCurrentPosition(IntPtr instance);
        public static long WTF_GetCurrentPosition(IntPtr instance) =>
            ((GetCurrentPosition)dllInvoke.Invoke("WTF_GetCurrentPosition", typeof(GetCurrentPosition))).Invoke(instance);
        private delegate int IsRunning(IntPtr instance);
        public static int WTF_IsRunning(IntPtr instance) =>
            ((IsRunning)dllInvoke.Invoke("WTF_IsRunning", typeof(IsRunning))).Invoke(instance);
        private delegate void SetFontScaleFactor(IntPtr instance, float factor);
        public static void WTF_SetFontScaleFactor(IntPtr instance, float factor) =>
            ((SetFontScaleFactor)dllInvoke.Invoke("WTF_SetFontScaleFactor", typeof(SetFontScaleFactor))).Invoke(instance, factor);
        private delegate void SetCompositionOpacity(IntPtr instance, float factor);
        public static void WTF_SetCompositionOpacity(IntPtr instance, float factor) =>
            ((SetCompositionOpacity)dllInvoke.Invoke("WTF_SetCompositionOpacity", typeof(SetCompositionOpacity))).Invoke(instance, factor);
        private delegate void SetFontName(IntPtr instance, string fontName);
        public static void WTF_SetFontName(IntPtr instance, string fontName) =>
            ((SetFontName)dllInvoke.Invoke("WTF_SetFontName", typeof(SetFontName))).Invoke(instance, fontName);
        private delegate void SetFontWeight(IntPtr instance, int dwriteFontWeight);
        public static void WTF_SetFontWeight(IntPtr instance, int dwriteFontWeight) =>
            ((SetFontWeight)dllInvoke.Invoke("WTF_SetFontWeight", typeof(SetFontWeight))).Invoke(instance, dwriteFontWeight);
        private delegate void SetDanmakuStyle(IntPtr instance, int style);
        public static void WTF_SetDanmakuStyle(IntPtr instance, int style) =>
            ((SetDanmakuStyle)dllInvoke.Invoke("WTF_SetDanmakuStyle", typeof(SetDanmakuStyle))).Invoke(instance, style);
    }
}
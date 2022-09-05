using System;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Danmaku
{
    sealed class DllInvoke
    {
        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(string path);
        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr lib, string funcName);
        [DllImport("kernel32.dll")]
        private extern static bool FreeLibrary(IntPtr lib);
        private readonly IntPtr hLib;
        public DllInvoke(string DLLPath)
        {
            hLib = LoadLibrary(DLLPath);
        }
        ~DllInvoke()
        {
            FreeLibrary(hLib);
        }
        public Delegate Invoke(string APIName, Type t)
        {
            IntPtr api = GetProcAddress(hLib, APIName);
            return Marshal.GetDelegateForFunctionPointer(api, t);
        }
    }
}
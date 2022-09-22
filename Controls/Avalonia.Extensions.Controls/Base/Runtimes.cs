using Avalonia.Extensions.Controls;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions
{
    public sealed class Runtimes
    {
        private Runtimes() { }
        static Runtimes()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                CurrentOS = OS.Linux;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                CurrentOS = OS.OSX;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                CurrentOS = OS.Windows;
            else
                CurrentOS = OS.Unknow;
        }
        public static OS CurrentOS { get; }
        public static Architecture Architecture => RuntimeInformation.ProcessArchitecture;
    }
}
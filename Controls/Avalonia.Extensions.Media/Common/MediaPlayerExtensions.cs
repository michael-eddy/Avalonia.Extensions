using Avalonia.Logging;
using Avalonia.Platform;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using System;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Media
{
    public static class MediaPlayerExtensions
    {
        public static void DisposeHandle(this MediaPlayer player)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    player.Stop();
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        player.Hwnd = IntPtr.Zero;
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        player.XWindow = 0;
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        player.NsObject = IntPtr.Zero;
                    player.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(player, ex.Message);
                }
            });
        }
        public static void SetHandle(this MediaPlayer player, IPlatformHandle handle)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        player.Hwnd = handle.Handle;
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        player.XWindow = (uint)handle.Handle;
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        player.NsObject = handle.Handle;
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(player, ex.Message);
                }
            });
        }
    }
}
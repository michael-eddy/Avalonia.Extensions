using Avalonia.Controls;
using Avalonia.Logging;
using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Media
{
    internal static class Untils
    {
        internal static unsafe string av_strerror(int error)
        {
            var bufferSize = 1024;
            var buffer = stackalloc byte[bufferSize];
            ffmpeg.av_strerror(error, buffer, (ulong)bufferSize);
            var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
            return message;
        }
        internal static int ThrowExceptionIfError(this int error)
        {
            if (error < 0)
                throw new ApplicationException(av_strerror(error));
            return error;
        }
        internal static bool Play(this MusicPlayerWindow player, string videoUrl, Dictionary<string, string> headers)
        {
            try
            {
                var mediaItem = new MediaItem(player.libVLC, new Uri(videoUrl));
                mediaItem.SetHeader(headers);
                player.mediaPlayer.Play(mediaItem);
                return true;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(player, ex.Message);
                return false;
            }
        }
        internal static void SetGridDef(this Control control, int rowIndex, int columnIndex)
        {
            try
            {
                Grid.SetRow(control, rowIndex);
                Grid.SetColumn(control, columnIndex);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(control, ex.Message);
            }
        }
    }
}
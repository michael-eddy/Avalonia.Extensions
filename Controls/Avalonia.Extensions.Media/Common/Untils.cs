using Avalonia.Controls;
using Avalonia.Logging;
using System;
using System.Collections.Generic;

namespace Avalonia.Extensions.Media
{
    internal static class Untils
    {
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
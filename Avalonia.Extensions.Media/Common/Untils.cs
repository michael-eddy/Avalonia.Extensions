using Avalonia.Controls;
using Avalonia.Logging;
using System;

namespace Avalonia.Extensions.Media
{
    internal static class Untils
    {
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
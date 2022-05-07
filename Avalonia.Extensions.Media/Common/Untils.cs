using Avalonia.Controls;

namespace Avalonia.Extensions.Media
{
    internal static class Untils
    {
        internal static void SetGridDef(this Control control, int rowIndex, int columnIndex)
        {
            Grid.SetRow(control, rowIndex);
            Grid.SetColumn(control, columnIndex);
        }
    }
}
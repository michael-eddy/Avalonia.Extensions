using Avalonia.Controls;
using CefNet;
using System.IO;

namespace Avalonia.Extensions
{
    public static class Untils
    {
        public static TabControl FindTabControl(this Control tab)
        {
            IControl control = tab;
            while (control != null)
            {
                if (control is TabControl tabControl)
                    return tabControl;
                control = control.Parent;
            }
            return null;
        }
    }
}
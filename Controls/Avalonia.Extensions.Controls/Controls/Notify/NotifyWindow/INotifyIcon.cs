using System;

namespace Avalonia.Extensions.Controls
{
    public interface INotifyIcon
    {
        bool Add(IntPtr ptr);
        bool Update();
        bool Hide();
        bool Show();
    }
}
using System;

namespace Avalonia.Extensions.Controls
{
    public interface INotifyIcon
    {
        bool Add();
        bool Update();
        bool Hide();
        bool Show();
        bool GetHIcon(Uri uri);
    }
}
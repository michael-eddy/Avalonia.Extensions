using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Controls
{
    public interface INotifyIcon
    {
        bool Add();
        bool Update();
        bool Hide();
        bool Show();
        Task<bool> GetHIcon(Uri uri);
    }
}
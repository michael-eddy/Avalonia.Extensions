using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Controls
{
    internal class NotifyIconLinux : INotifyIcon
    {
        private static NotifyIcon Owner;
        private NotifyIconState state = NotifyIconState.None;
        public NotifyIconLinux(NotifyIcon owner)
        {
            Owner = owner;
        }
        public bool Add()
        {
            return false;
        }
        public bool GetHIcon(Uri uri)
        {
            return false;
        }
        public bool Hide()
        {
            return false;
        }
        public bool Show()
        {
            return false;
        }
        public bool Update()
        {
            return false;
        }
    }
}
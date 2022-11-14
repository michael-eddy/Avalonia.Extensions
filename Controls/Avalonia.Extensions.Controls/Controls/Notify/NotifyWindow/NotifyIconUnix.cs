using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.Extensions.Controls
{
    internal class NotifyIconUnix : INotifyIcon
    {
        private static NotifyIcon Owner;
        private NotifyIconState state = NotifyIconState.None;
        public NotifyIconUnix(NotifyIcon owner)
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
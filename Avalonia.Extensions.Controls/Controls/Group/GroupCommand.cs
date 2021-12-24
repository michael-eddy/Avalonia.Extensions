using System;
using System.Windows.Input;

namespace Avalonia.Extensions.Controls
{
    internal sealed class GroupCommand : ICommand
    {
        public event EventHandler ExecuteCallback;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            ExecuteCallback?.Invoke(this, default);
        }
    }
}
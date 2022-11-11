using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Controls
{
    public abstract class ViewModelBase : ReactiveObject, IDisposable
    {
        protected void RunOnUiThread(Action action)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                action?.Invoke();
            });
        }
        protected void RunOnUiThread(Func<Task> action)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                action?.Invoke();
            });
        }
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
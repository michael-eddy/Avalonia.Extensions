using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Avalonia.Controls.Demo
{
    public sealed class MainViewModel : ReactiveObject, IDisposable
    {
        private string message;
        public string Message
        {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }
        private ObservableCollection<object> items;
        public ObservableCollection<object> Items
        {
            get => items;
            private set => this.RaiseAndSetIfChanged(ref items, value);
        }
        public ReactiveCommand<object, Unit> OnButtonClick { get; }
        public ReactiveCommand<object, Unit> OnItemClick { get; }
        public MainViewModel()
        {
            OnItemClick = ReactiveCommand.Create<object>(ItemClick);
            OnButtonClick = ReactiveCommand.Create<object>(ButtonClick);
            items = new ObservableCollection<object>();
            for (var idx = 0; idx < 9; idx++)
            {
                for (var idx2 = 0; idx2 < 9; idx2++)
                    items.Add(new { content = $"{idx}#{idx2}" });
            }
        }
        private void ItemClick(object obj)
        {
            Message = "你点击了SplitListView , CommandParameter :" + obj;
        }
        private void ButtonClick(object obj)
        {
            Message = "你点击了ItemsRepeaterContent , CommandParameter :" + obj;
        }
        public void Dispose()
        {

        }
    }
}
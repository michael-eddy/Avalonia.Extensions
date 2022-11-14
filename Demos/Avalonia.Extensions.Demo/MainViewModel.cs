using Avalonia.Extensions;
using Avalonia.Extensions.Media;
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
        private ObservableCollection<GroupViewItem> items;
        public ObservableCollection<GroupViewItem> Items
        {
            get => items;
            private set => this.RaiseAndSetIfChanged(ref items, value);
        }
        public ReactiveCommand<object, Unit> OnItemClick { get; }
        public ReactiveCommand<object, Unit> OnAudioClick { get; }
        public ReactiveCommand<object, Unit> OnButtonClick { get; }
        public MainViewModel()
        {
            OnItemClick = ReactiveCommand.Create<object>(ItemClick);
            OnAudioClick = ReactiveCommand.Create<object>(AudioClick);
            OnButtonClick = ReactiveCommand.Create<object>(ButtonClick);
            items = new ObservableCollection<GroupViewItem>();
            for (var idx = 0; idx < 9; idx++)
            {
                for (var idx2 = 0; idx2 < 9; idx2++)
                    items.Add(new GroupViewItem($"{idx}#{idx2}"));
            }
        }
        private void AudioClick(object obj)
        {
            new MusicPlayerWindow().Show();
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
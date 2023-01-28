using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using Avalonia.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Avalonia.Extensions.Controls
{
    public class PaginationView : TemplatedControl
    {
        private Border _border;
        private Button _btnPrePage;
        private Button _btnLastPage;
        private Button _btnFirstPage;
        private Button _btnNextPage;
        private ListBox _lstShowingPage;
        private ComboBox _cbbPageDataCount;
        private bool _isIgnoreListBoxSelectionChanged;
        private static readonly object _lock = new object();
        static PaginationView()
        {
            BoundsProperty.Changed.AddClassHandler<PaginationView>(OnBoundsChanged);
            PageDataCountProperty.Changed.AddClassHandler<PaginationView>(OnPageDataCountPropertyChanged);
            CurrentPageNumberProperty.Changed.AddClassHandler<PaginationView>(OnCurrentPageNumberChanged);
        }
        private static void OnBoundsChanged(PaginationView pagination, AvaloniaPropertyChangedEventArgs e)
        {
            if (pagination != null && !e.IsSameValue() && pagination.GetVisualRoot() is Window window && e.NewValue is Rect rect)
            {
                var visable = (window.Width * 0.8) > pagination._border.Bounds.Width;
                pagination.SetValue(IsShowPageInfoProperty, visable);
                pagination.SetValue(IsShowPageButtonSelectorProperty, visable);
                pagination.SetValue(IsShowPageDataCountSelectorProperty, visable);
            }
        }
        private static void OnPageDataCountPropertyChanged(PaginationView pagination, AvaloniaPropertyChangedEventArgs e)
            => pagination?.InitData();
        private static void OnCurrentPageNumberChanged(PaginationView pagination, AvaloniaPropertyChangedEventArgs e)
        {
            if (pagination == null) return;
            if (pagination._lstShowingPage != null)
                pagination._lstShowingPage.SelectedItem = e.NewValue;
            pagination.SetBtnEnable();
        }
        /// <summary>
        /// Defines the <see cref="FisrtPageContent"/> property.
        /// </summary>
        public static readonly StyledProperty<string> FisrtPageContentProperty =
          AvaloniaProperty.Register<PaginationView, string>(nameof(FisrtPageContent), Core.Instance.IsEnglish ? "First" : "首页");
        public string FisrtPageContent
        {
            get => GetValue(FisrtPageContentProperty);
            set => SetValue(FisrtPageContentProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="LastPageContent"/> property.
        /// </summary>
        public static readonly StyledProperty<string> LastPageContentProperty =
          AvaloniaProperty.Register<PaginationView, string>(nameof(LastPageContent), Core.Instance.IsEnglish ? "Last" : "尾页");
        public string LastPageContent
        {
            get => GetValue(LastPageContentProperty);
            set => SetValue(LastPageContentProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="IsShowPageButtonSelector"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsShowPageButtonSelectorProperty =
          AvaloniaProperty.Register<PaginationView, bool>(nameof(IsShowPageButtonSelector), true);
        public bool IsShowPageButtonSelector
        {
            get => GetValue(IsShowPageButtonSelectorProperty);
            set => SetValue(IsShowPageButtonSelectorProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="IsShowPageDataCountSelector"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsShowPageDataCountSelectorProperty =
          AvaloniaProperty.Register<PaginationView, bool>(nameof(IsShowPageDataCountSelector), true);
        public bool IsShowPageDataCountSelector
        {
            get => GetValue(IsShowPageDataCountSelectorProperty);
            set => SetValue(IsShowPageDataCountSelectorProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="IsShowPageInfo"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsShowPageInfoProperty =
          AvaloniaProperty.Register<PaginationView, bool>(nameof(IsShowPageInfo), true);
        public bool IsShowPageInfo
        {
            get => GetValue(IsShowPageInfoProperty);
            set => SetValue(IsShowPageInfoProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="ShowingPageNumberCollection"/> property.
        /// </summary>
        public static readonly StyledProperty<ObservableCollection<int>> ShowingPageNumberCollectionProperty =
          AvaloniaProperty.Register<PaginationView, ObservableCollection<int>>(nameof(ShowingPageNumberCollection), new ObservableCollection<int>());
        public ObservableCollection<int> ShowingPageNumberCollection
        {
            get => GetValue(ShowingPageNumberCollectionProperty);
            set => SetValue(ShowingPageNumberCollectionProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="PageDataCountCollection"/> property.
        /// </summary>
        public static readonly StyledProperty<ObservableCollection<int>> PageDataCountCollectionProperty =
          AvaloniaProperty.Register<PaginationView, ObservableCollection<int>>(nameof(PageDataCountCollection), new ObservableCollection<int> { 20, 30, 50 });
        public ObservableCollection<int> PageDataCountCollection
        {
            get => GetValue(PageDataCountCollectionProperty);
            set => SetValue(PageDataCountCollectionProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="PageDataCount"/> property.
        /// </summary>
        public static readonly StyledProperty<int> PageDataCountProperty =
          AvaloniaProperty.Register<PaginationView, int>(nameof(PageDataCount), 20);
        public int PageDataCount
        {
            get => GetValue(PageDataCountProperty);
            set => SetValue(PageDataCountProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="CurrentPageNumber"/> property.
        /// </summary>
        public static readonly StyledProperty<int> CurrentPageNumberProperty =
          AvaloniaProperty.Register<PaginationView, int>(nameof(CurrentPageNumber), 1);
        public int CurrentPageNumber
        {
            get => GetValue(CurrentPageNumberProperty);
            set => SetValue(CurrentPageNumberProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="TotalDataCount"/> property.
        /// </summary>
        public static readonly StyledProperty<int> TotalDataCountProperty =
          AvaloniaProperty.Register<PaginationView, int>(nameof(TotalDataCount), 0);
        public int TotalDataCount
        {
            get => GetValue(TotalDataCountProperty);
            set => SetValue(TotalDataCountProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="CurrentPageDataCount"/> property.
        /// </summary>
        public static readonly StyledProperty<int> CurrentPageDataCountProperty =
          AvaloniaProperty.Register<PaginationView, int>(nameof(CurrentPageDataCount), 0);
        public int CurrentPageDataCount
        {
            get => GetValue(CurrentPageDataCountProperty);
            set => SetValue(CurrentPageDataCountProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="TotalPageCount"/> property.
        /// </summary>
        public static readonly StyledProperty<int> TotalPageCountProperty =
          AvaloniaProperty.Register<PaginationView, int>(nameof(TotalPageCount), 1);
        public int TotalPageCount
        {
            get => GetValue(TotalPageCountProperty);
            set => SetValue(TotalPageCountProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="ShowingPageDataStartNumber"/> property.
        /// </summary>
        public static readonly StyledProperty<int> ShowingPageDataStartNumberProperty =
          AvaloniaProperty.Register<PaginationView, int>(nameof(ShowingPageDataStartNumber), 0);
        public int ShowingPageDataStartNumber
        {
            get => GetValue(ShowingPageDataStartNumberProperty);
            set => SetValue(ShowingPageDataStartNumberProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="ShowingPageDataEndNumber"/> property.
        /// </summary>
        public static readonly StyledProperty<int> ShowingPageDataEndNumberProperty =
          AvaloniaProperty.Register<PaginationView, int>(nameof(ShowingPageDataEndNumber), 0);
        public int ShowingPageDataEndNumber
        {
            get => GetValue(ShowingPageDataEndNumberProperty);
            set => SetValue(ShowingPageDataEndNumberProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="MaxShownPageCount"/> property.
        /// </summary>
        public static readonly StyledProperty<int> MaxShownPageCountProperty =
          AvaloniaProperty.Register<PaginationView, int>(nameof(MaxShownPageCount), 7);
        public int MaxShownPageCount
        {
            get => GetValue(ShowingPageDataEndNumberProperty);
            set => SetValue(ShowingPageDataEndNumberProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="SelectedPageBackground"/> property.
        /// </summary>
        public static readonly StyledProperty<SolidColorBrush> SelectedPageBackgroundProperty =
          AvaloniaProperty.Register<PaginationView, SolidColorBrush>(nameof(SelectedPageBackground), new SolidColorBrush(Colors.Red));
        public SolidColorBrush SelectedPageBackground
        {
            get => GetValue(SelectedPageBackgroundProperty);
            set => SetValue(SelectedPageBackgroundProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="PageSelectorBackground"/> property.
        /// </summary>
        public static readonly StyledProperty<SolidColorBrush> PageSelectorBackgroundProperty =
          AvaloniaProperty.Register<PaginationView, SolidColorBrush>(nameof(PageSelectorBackground), new SolidColorBrush(Colors.Transparent));
        public SolidColorBrush PageSelectorBackground
        {
            get => GetValue(PageSelectorBackgroundProperty);
            set => SetValue(PageSelectorBackgroundProperty, value);
        }
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            InitControls(e);
            ShowingPageNumberCollection = new ObservableCollection<int>();
            InitData();
        }
        private void InitControls(TemplateAppliedEventArgs e)
        {
            _border = e.NameScope.Get<Border>("PART_Pagination");
            _cbbPageDataCount = e.NameScope.Get<ComboBox>("PART_ComboBox");
            if (_cbbPageDataCount != null)
                _cbbPageDataCount.SelectionChanged += _cbbPageDataCount_SelectionChanged;
            _lstShowingPage = e.NameScope.Get<ListBox>("PART_ListBoxPages");
            if (_lstShowingPage != null)
                _lstShowingPage.SelectionChanged += _lstShowingPage_SelectionChanged;
            _btnFirstPage = e.NameScope.Get<Button>("PART_ButtonFirstPage");
            if (_btnFirstPage != null)
                _btnFirstPage.Click += _btnFirstPage_Click;
            _btnPrePage = e.NameScope.Get<Button>("PART_ButtonPrePage");
            if (_btnPrePage != null)
                _btnPrePage.Click += _btnPrePage_Click;
            _btnNextPage = e.NameScope.Get<Button>("PART_ButtonNextPage");
            if (_btnNextPage != null)
                _btnNextPage.Click += _btnNextPage_Click;
            _btnLastPage = e.NameScope.Get<Button>("PART_ButtonLastPage");
            if (_btnLastPage != null)
                _btnLastPage.Click += _btnLastPage_Click;
        }
        private void InitData()
        {
            try
            {
                _isIgnoreListBoxSelectionChanged = true;
                if (PageDataCount > 0)
                {
                    if (TotalDataCount % PageDataCount > 0)
                        TotalPageCount = TotalDataCount / PageDataCount + 1;
                    else
                        TotalPageCount = TotalDataCount / PageDataCount;
                    MaxShownPageCount = TotalPageCount;
                    if (ShowingPageNumberCollection != null)
                    {
                        lock (_lock)
                        {
                            ShowingPageNumberCollection.Clear();
                            int addPageCount = MaxShownPageCount;
                            if (TotalPageCount < MaxShownPageCount)
                                addPageCount = TotalPageCount;
                            for (int i = 1; i <= addPageCount; i++)
                                ShowingPageNumberCollection.Add(i);
                        }
                    }
                    if (_lstShowingPage != null)
                    {
                        _lstShowingPage.SelectedIndex = 0;
                        CurrentPageNumber = 1;
                    }
                    UpdateShowingPageInfo();
                }
                SetBtnEnable();
            }
            finally
            {
                _isIgnoreListBoxSelectionChanged = false;
            }
        }
        private void UpdateShowingPageInfo()
        {
            if (TotalPageCount == 0)
            {
                ShowingPageDataStartNumber = 0;
                ShowingPageDataEndNumber = 0;
            }
            else if (CurrentPageNumber < TotalPageCount)
            {
                ShowingPageDataStartNumber = (CurrentPageNumber - 1) * PageDataCount + 1;
                ShowingPageDataEndNumber = CurrentPageNumber * PageDataCount;
            }
            else if (CurrentPageNumber == TotalPageCount)
            {
                ShowingPageDataStartNumber = (CurrentPageNumber - 1) * PageDataCount + 1;
                ShowingPageDataEndNumber = TotalDataCount;
            }
        }
        private void SetBtnEnable()
        {
            if (_btnFirstPage == null || _btnPrePage == null || _btnNextPage == null || _btnLastPage == null)
                return;
            _btnPrePage.IsEnabled = true;
            _btnNextPage.IsEnabled = true;
            _btnFirstPage.IsEnabled = true;
            _btnLastPage.IsEnabled = true;
            if (ShowingPageNumberCollection == null || ShowingPageNumberCollection.Count == 0)
            {
                _btnPrePage.IsEnabled = false;
                _btnNextPage.IsEnabled = false;
                _btnFirstPage.IsEnabled = false;
                _btnLastPage.IsEnabled = false;
            }
            else
            {
                if (CurrentPageNumber == 1)
                {
                    _btnFirstPage.IsEnabled = false;
                    _btnPrePage.IsEnabled = false;
                }
                if (CurrentPageNumber == TotalPageCount)
                {
                    _btnNextPage.IsEnabled = false;
                    _btnLastPage.IsEnabled = false;
                }
            }
        }
        private void _btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            if (_lstShowingPage == null || ShowingPageNumberCollection == null || ShowingPageNumberCollection.Count == 0)
                return;
            if (ShowingPageNumberCollection.Last() != TotalPageCount)
            {
                try
                {
                    _isIgnoreListBoxSelectionChanged = true;
                    lock (_lock)
                    {
                        ShowingPageNumberCollection.Clear();
                        for (int i = 0; i < MaxShownPageCount; i++)
                            ShowingPageNumberCollection.Add(TotalPageCount - MaxShownPageCount + i + 1);
                    }
                }
                finally
                {
                    _isIgnoreListBoxSelectionChanged = false;
                }
            }
            if (_lstShowingPage.Items is ObservableCollection<int> array)
                _lstShowingPage.SelectedIndex = array.Count - 1;
        }
        private void _btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_lstShowingPage == null || ShowingPageNumberCollection == null || ShowingPageNumberCollection.Count == 0)
                return;
            if (_lstShowingPage.SelectedIndex < MaxShownPageCount - 1)
                _lstShowingPage.SelectedIndex++;
        }
        private void _btnPrePage_Click(object sender, RoutedEventArgs e)
        {
            if (_lstShowingPage == null || ShowingPageNumberCollection == null || ShowingPageNumberCollection.Count == 0)
                return;
            if (_lstShowingPage.SelectedIndex > 0)
                _lstShowingPage.SelectedIndex--;
        }
        private void _btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            if (_lstShowingPage == null || ShowingPageNumberCollection == null || ShowingPageNumberCollection.Count == 0)
                return;
            if (ShowingPageNumberCollection[0] != 1)
            {
                try
                {
                    _isIgnoreListBoxSelectionChanged = true;
                    lock (_lock)
                    {
                        ShowingPageNumberCollection.Clear();
                        for (int i = 1; i <= MaxShownPageCount; i++)
                            ShowingPageNumberCollection.Add(i);
                    }
                }
                finally
                {
                    _isIgnoreListBoxSelectionChanged = false;
                }
            }
            _lstShowingPage.SelectedIndex = 0;
        }
        private void _cbbPageDataCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox cbb || cbb.SelectedItem == null)
                return;
            string selectedCountString = cbb.SelectedItem.ToString();
            if (!int.TryParse(selectedCountString, out int selectedDataCount))
                return;
            PageDataCount = selectedDataCount;
            InitData();
        }
        private void _lstShowingPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isIgnoreListBoxSelectionChanged)
                return;
            try
            {
                _isIgnoreListBoxSelectionChanged = true;
                if (sender is not ListBox lst || lst.SelectedItem == null)
                    return;
                string selectedPageString = lst.SelectedItem.ToString();
                if (!int.TryParse(selectedPageString, out int selectedPageNumber))
                    return;
                if (TotalPageCount <= MaxShownPageCount)
                {
                    CurrentPageNumber = selectedPageNumber;
                    UpdateShowingPageInfo();
                    return;
                }
                int moveCount = MaxShownPageCount / 2 - _lstShowingPage.SelectedIndex;
                int startPageNumber = ShowingPageNumberCollection.First();
                if (moveCount > 0)
                {
                    int realMoveCount = moveCount;
                    if (ShowingPageNumberCollection.First() - 1 < moveCount)
                        realMoveCount = ShowingPageNumberCollection.First() - 1;
                    startPageNumber = ShowingPageNumberCollection.First() - realMoveCount;
                }
                else if (moveCount < 0)
                {
                    int realMoveCount = -moveCount;
                    if (TotalPageCount - ShowingPageNumberCollection.Last() < realMoveCount)
                        realMoveCount = TotalPageCount - ShowingPageNumberCollection.Last();
                    startPageNumber = ShowingPageNumberCollection.First() + realMoveCount;
                }
                lock (_lock)
                {
                    ShowingPageNumberCollection.Clear();
                    for (int i = 0; i < MaxShownPageCount; i++)
                        ShowingPageNumberCollection.Add(startPageNumber + i);
                }
                int selectedItemIndex = ShowingPageNumberCollection.IndexOf(selectedPageNumber);
                _lstShowingPage.SelectedIndex = selectedItemIndex;
                CurrentPageNumber = selectedPageNumber;
                UpdateShowingPageInfo();
            }
            finally
            {
                _isIgnoreListBoxSelectionChanged = false;
            }
        }
    }
}
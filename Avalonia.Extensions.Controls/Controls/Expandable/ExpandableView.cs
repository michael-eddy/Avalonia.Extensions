using Avalonia.Controls;
using Avalonia.Extensions.Styles;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;

namespace Avalonia.Extensions.Controls
{
    public partial class ExpandableView : StackPanel, ITemplatedControl, IStyling
    {
        public ExpandableView() : base()
        {
            Status = ExpandStatus.Collapsed;
            Orientation = Orientation.Vertical;
            SecondViewProperty.Changed.AddClassHandler<ExpandableView>(OnSecondViewChange);
            PrimaryViewProperty.Changed.AddClassHandler<ExpandableView>(OnPrimaryViewChange);
        }
        private void OnSecondViewChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue())
            {
                bool isVisible = false;
                if (e.OldValue is Panel oldControl)
                {
                    isVisible = oldControl.IsVisible;
                    Children.Remove(oldControl);
                }
                if (e.NewValue is Panel newControl)
                {
                    newControl.IsVisible = isVisible;
                    Children.Add(newControl);
                }
            }
        }
        private void OnPrimaryViewChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue())
            {
                if (e.OldValue is ClickableView oldControl)
                {
                    oldControl.Click -= ClickView_Click;
                    Children.Remove(oldControl);
                }
                if (e.NewValue is ClickableView newControl)
                {
                    newControl.Click += ClickView_Click;
                    Children.Add(newControl);
                }
            }
        }
        /// <summary>
        /// Defines the <see cref="Status"/> property.
        /// </summary>
        public static readonly StyledProperty<ExpandStatus> StatusProperty =
          AvaloniaProperty.Register<ExpandableView, ExpandStatus>(nameof(Status), ExpandStatus.Collapsed);
        /// <summary>
        /// get or set second view <see cref="ExpandStatus.Collapsed"/> or <see cref="ExpandStatus.Expanded"/>
        /// </summary>
        public ExpandStatus Status
        {
            get => GetValue(StatusProperty);
            private set => SetValue(StatusProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="PrimaryView"/> property.
        /// </summary>
        public static readonly StyledProperty<ClickableView> PrimaryViewProperty =
          AvaloniaProperty.Register<ExpandableView, ClickableView>(nameof(PrimaryView));
        public ClickableView PrimaryView
        {
            get => GetValue(PrimaryViewProperty);
            set => SetValue(PrimaryViewProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="SecondView"/> property.
        /// </summary>
        public static readonly StyledProperty<Panel> SecondViewProperty =
          AvaloniaProperty.Register<ExpandableView, Panel>(nameof(SecondView));
        public Panel SecondView
        {
            get => GetValue(SecondViewProperty);
            set => SetValue(SecondViewProperty, value);
        }
        private void ClickView_Click(object sender, RoutedEventArgs e)
        {
            if (Status == ExpandStatus.Collapsed)
            {
                Expand();
                Status = ExpandStatus.Expanded;
            }
            else
            {
                Collapse();
                Status = ExpandStatus.Collapsed;
            }
        }
        public void Collapse()
        {
            if (Status == ExpandStatus.Expanded)
            {
                SecondView.IsVisible = false;
                Status = ExpandStatus.Collapsed;
            }
        }
        public void Expand()
        {
            if (Status == ExpandStatus.Collapsed)
            {
                SecondView.IsVisible = true;
                Status = ExpandStatus.Expanded;
            }
        }
    }
}
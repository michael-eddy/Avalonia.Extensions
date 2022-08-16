using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System;

namespace Avalonia.Extensions.Controls
{
    public class ProgressRing : TemplatedControl
    {
        private const string LargeState = ":large";
        private const string SmallState = ":small";
        private const string ActiveState = ":active";
        private const string InactiveState = ":inactive";
        private double _maxSideLength = 10;
        private double _ellipseDiameter = 10;
        private Thickness _ellipseOffset = new Thickness(2);
        public ProgressRing() : base()
        {
            SetValue(WidthProperty, 80);
            SetValue(HeightProperty, 80);
        }
        public bool IsActive
        {
            get => GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }
        public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<ProgressRing, bool>(nameof(IsActive), true, notifying: OnIsActiveChanged);
        private static void OnIsActiveChanged(IAvaloniaObject obj, bool arg2) => ((ProgressRing)obj)?.UpdateVisualStates();
        public static readonly DirectProperty<ProgressRing, double> MaxSideLengthProperty =
            AvaloniaProperty.RegisterDirect<ProgressRing, double>(nameof(MaxSideLength), o => o.MaxSideLength);
        public double MaxSideLength
        {
            get => _maxSideLength;
            private set => SetAndRaise(MaxSideLengthProperty, ref _maxSideLength, value);
        }
        public static readonly DirectProperty<ProgressRing, double> EllipseDiameterProperty =
            AvaloniaProperty.RegisterDirect<ProgressRing, double>(nameof(EllipseDiameter), o => o.EllipseDiameter);
        public double EllipseDiameter
        {
            get => _ellipseDiameter;
            private set => SetAndRaise(EllipseDiameterProperty, ref _ellipseDiameter, value);
        }
        public static readonly DirectProperty<ProgressRing, Thickness> EllipseOffsetProperty =
            AvaloniaProperty.RegisterDirect<ProgressRing, Thickness>(nameof(EllipseOffset), o => o.EllipseOffset);
        public Thickness EllipseOffset
        {
            get => _ellipseOffset;
            private set => SetAndRaise(EllipseOffsetProperty, ref _ellipseOffset, value);
        }
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            double maxSideLength = Math.Min(Width, Height);
            double ellipseDiameter = maxSideLength * .1;
            if (maxSideLength <= 40)
                ellipseDiameter += 1;
            EllipseDiameter = ellipseDiameter;
            MaxSideLength = maxSideLength;
            EllipseOffset = new Thickness(0, maxSideLength / 2 - ellipseDiameter, 0, 0);
            UpdateVisualStates();
        }
        public override void Render(DrawingContext context)
        {
            base.Render(context);
            UpdateVisualStates();
        }
        private void UpdateVisualStates()
        {
            PseudoClasses.Remove(ActiveState);
            PseudoClasses.Remove(InactiveState);
            PseudoClasses.Remove(SmallState);
            PseudoClasses.Remove(LargeState);
            PseudoClasses.Add(IsActive ? ActiveState : InactiveState);
            PseudoClasses.Add(_maxSideLength < 60 ? SmallState : LargeState);
        }
    }
}
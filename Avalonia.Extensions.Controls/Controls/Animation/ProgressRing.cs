using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Controls
{
    public class ProgressRing : Canvas
    {
        private double centerRound, innerRound;
        private readonly SolidColorBrush fillBrush;
        public ProgressRing() : base()
        {
            WidthProperty.Changed.AddClassHandler<ProgressRing>(OnWidthChange);
            fillBrush = new SolidColorBrush(Colors.White);
            if (double.IsNaN(Width) && double.IsNaN(Height))
            {
                Width = 128;
                Height = 128;
            }
            ZIndex = int.MaxValue;
        }
        private void OnWidthChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is double d)
            {
                centerRound = d / 2;
                innerRound = d * 0.8;
            }
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            IsVisible = false;
            DrawBase();
            DrawAnimation();
        }
        private Ellipse _moving;
        private double _movingLeft, _movingTop, _movingRadian = -90;
        private void DrawAnimation()
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                while (IsVisible)
                {
                    double round = (Width - innerRound) / 2;
                    var movingRange = centerRound - round / 2;
                    if (_moving == null)
                    {
                        _moving = new Ellipse
                        {
                            ZIndex = 2,
                            Width = round,
                            Height = round,
                            Fill = fillBrush
                        };
                        _moving.Measure(new Size(round, round));
                        _moving.Arrange(new Rect(0, 0, round, round));
                        Children.Add(_moving);
                    }
                    _movingRadian += 1;
                    if (_movingRadian == 360)
                        _movingRadian = 0;
                    var radian = _movingRadian.ToRadians();
                    var pointX = movingRange + movingRange * Math.Cos(radian);
                    var pointY = movingRange + movingRange * Math.Sin(radian);
                    _movingTop = pointY;
                    _movingLeft = pointX;
                    SetTop(_moving, _movingTop);
                    SetLeft(_moving, _movingLeft);
                    await Task.Delay(30);
                }
            });
        }
        private void DrawBase()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var target = new Ellipse
                {
                    ZIndex = 1,
                    Width = innerRound,
                    Height = innerRound,
                    Fill = fillBrush
                };
                target.Measure(new Size(innerRound, innerRound));
                target.Arrange(new Rect(0, 0, innerRound, innerRound));
                Children.Add(target);
                var top = centerRound - innerRound / 2;
                SetLeft(target, top);
                SetTop(target, top);

                target = new Ellipse
                {
                    Width = Width,
                    Height = Width,
                    Fill = Core.Instance.PrimaryBrush
                };
                target.Measure(new Size(Width, Width));
                target.Arrange(new Rect(0, 0, Width, Width));
                Children.Add(target);
                var circleBounds = centerRound - Width / 2;
                SetLeft(target, circleBounds);
                SetTop(target, circleBounds);
            });
        }
        public void Show()
        {
            IsVisible = true;
            DrawAnimation();
        }
        public void Hidden()
        {
            IsVisible = false;
        }
    }
}
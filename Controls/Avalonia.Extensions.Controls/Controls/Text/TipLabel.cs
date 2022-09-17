using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using SkiaSharp;
using System;

namespace Avalonia.Extensions.Controls
{
    public sealed class TipLabel : Panel
    {
        private string _content;
        private Border DockPanel { get; }
        private TextBlock TextBlock { get; }
        static TipLabel()
        {
            ContentProperty.Changed.AddClassHandler<TipLabel>(OnContentChange);
            PaddingProperty.Changed.AddClassHandler<TipLabel>(OnPaddingChange);
            ForegroundProperty.Changed.AddClassHandler<TipLabel>(OnForegroundChange);
            BackgroundProperty.Changed.AddClassHandler<TipLabel>(OnBackgroundChange);
            BorderBrushProperty.Changed.AddClassHandler<TipLabel>(OnBorderBrushChange);
            CornerRadiusProperty.Changed.AddClassHandler<TipLabel>(OnCornerRadiusChanged);
            BorderThicknessProperty.Changed.AddClassHandler<TipLabel>(OnBorderThicknessChange);
        }
        public TipLabel() : base()
        {
            TextBlock = new TextBlock
            {
                ZIndex = 1,
                Text = Content,
                Foreground = Foreground,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            DockPanel = new Border
            {
                Padding = Padding,
                Background = Background,
                BorderBrush = BorderBrush,
                CornerRadius = CornerRadius,
                BorderThickness = BorderThickness
            };
            DockPanel.Child = TextBlock;
            Children.Add(DockPanel);
            SetValue(BackgroundProperty, new SolidColorBrush(Colors.Gray));
        }
        [Content]
        public string Content
        {
            get => _content;
            set => SetAndRaise(ContentProperty, ref _content, value);
        }
        /// <summary>
        /// Gets or sets a brush with which to paint the border.
        /// </summary>
        public IBrush BorderBrush
        {
            get => GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }
        /// <summary>
        /// Gets or sets the padding to place around the <see cref="Child"/> control.
        /// </summary>
        public Thickness Padding
        {
            get => GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }
        /// <summary>
        /// Gets or sets the thickness of the border.
        /// </summary>
        public Thickness BorderThickness
        {
            get => GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }
        public IBrush Foreground
        {
            get => GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        /// <summary>
        /// Gets or sets the radius of the border rounded corners.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="CornerRadius"/> property.
        /// </summary>
        public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
            AvaloniaProperty.Register<TipLabel, CornerRadius>(nameof(CornerRadius), new CornerRadius(6));
        public static readonly StyledProperty<IBrush> ForegroundProperty =
            AvaloniaProperty.Register<TipLabel, IBrush>(nameof(Foreground), new SolidColorBrush(Colors.White));
        /// <summary>
        /// Defines the <see cref="BorderBrush"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush> BorderBrushProperty =
            AvaloniaProperty.Register<TipLabel, IBrush>(nameof(BorderBrush));
        /// <summary>
        /// Defines the <see cref="BorderThickness"/> property.
        /// </summary>
        public static readonly StyledProperty<Thickness> BorderThicknessProperty =
            AvaloniaProperty.Register<TipLabel, Thickness>(nameof(BorderThickness));
        public static readonly StyledProperty<Thickness> PaddingProperty =
         AvaloniaProperty.Register<TipLabel, Thickness>(nameof(Padding));
        /// <summary>
        /// Defines the <see cref="ContentProperty"/> property.
        /// </summary>
        public static readonly DirectProperty<TipLabel, string> ContentProperty =
               AvaloniaProperty.RegisterDirect<TipLabel, string>(nameof(Content), o => o.Content, (o, v) => o.Content = v);
        private static void OnContentChange(TipLabel label, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is string chars)
            {
                label.TextBlock.Text = chars;
                SKTypeface typeface = SKTypeface.FromFamilyName(label.TextBlock.FontFamily.Name);
                var size = chars.MeasureString(label.TextBlock.FontSize, typeface);
                var width = Convert.ToDouble(size.Width).Upper() + (label.Padding.Left + label.Padding.Right).Upper();
                label.Width = label.DockPanel.Width = width;
            }
        }
        private static void OnBackgroundChange(TipLabel label, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is IBrush brush && brush != Core.Instance.Transparent)
            {
                label.DockPanel.Background = brush;
                label.Background = Core.Instance.Transparent;
            }
        }
        private static void OnPaddingChange(TipLabel label, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is Thickness thickness)
            {
                label.DockPanel.Padding = thickness;
                label.Height += thickness.Bottom + thickness.Top;
            }
        }
        private static void OnBorderThicknessChange(TipLabel label, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is Thickness thickness)
                label.DockPanel.BorderThickness = thickness;
        }
        private static void OnBorderBrushChange(TipLabel label, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is IBrush brush)
                label.DockPanel.BorderBrush = brush;
        }
        private static void OnForegroundChange(TipLabel label, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is IBrush brush)
                label.TextBlock.Foreground = brush;
        }
        private static void OnCornerRadiusChanged(TipLabel label, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is CornerRadius radius)
                label.DockPanel.CornerRadius = radius;
        }
    }
}
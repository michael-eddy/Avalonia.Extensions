﻿using Avalonia.Controls.Shapes;
using Avalonia.Extensions.Media;
using Avalonia.Extensions.Threading;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using Avalonia.Visuals.Media.Imaging;
using System;

namespace Avalonia.Extensions.Controls
{
    public sealed class CircleImage : Ellipse, IBitmapSource
    {
        private BitmapThread Task { get; }
        /// <summary>
        /// Defines the <see cref="Source"/> property.
        /// </summary>
        public static readonly StyledProperty<Uri> SourceProperty =
            AvaloniaProperty.Register<CircleImage, Uri>(nameof(Source));
        /// <summary>
        /// Defines the <see cref="ImageSource"/> property.
        /// </summary>
        public static readonly StyledProperty<Bitmap?> ImageSourceProperty =
            AvaloniaProperty.Register<CircleImage, Bitmap?>(nameof(ImageSource));
        /// <summary>
        /// Defines the <see cref="InterpolationMode"/> property.
        /// </summary>
        public static readonly StyledProperty<BitmapInterpolationMode> InterpolationModeProperty =
            AvaloniaProperty.Register<CircleImage, BitmapInterpolationMode>(nameof(InterpolationMode), BitmapInterpolationMode.HighQuality);
        /// <summary>
        /// Defines the <see cref="Failed"/> property.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> FailedEvent =
            RoutedEvent.Register<CircleImage, RoutedEventArgs>(nameof(Failed), RoutingStrategies.Bubble);
        static CircleImage()
        {
            AffectsRender<CircleImage>(SourceProperty);
            AffectsMeasure<CircleImage>(SourceProperty);
        }
        public CircleImage() : base()
        {
            Task = new BitmapThread(this);
            SourceProperty.Changed.AddClassHandler<CircleImage>(OnSourceChange);
            ImageSourceProperty.Changed.AddClassHandler<CircleImage>(OnImageSourceProperty);
        }
        private void OnImageSourceProperty(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is Bitmap bitmap)
            {
                Fill = new ImageBrush { Source = bitmap };
                DrawAgain();
                SetSize(bitmap.Size);
            }
        }
        public Bitmap Bitmap { get; set; }
        private void OnSourceChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is Uri uri)
                Task.Run(uri);
        }
        internal void SetSize(Size size)
        {
            if (double.IsNaN(Width) && double.IsNaN(Height))
                Height = Width = Math.Min(size.Width, size.Height);
            else if (double.IsNaN(Width) && Height > 0)
                Width = Height;
            else if (double.IsNaN(Height) && Width > 0)
                Height = Width;
        }
        public void DrawAgain()
        {
            InvalidateVisual();
            InvalidateMeasure();
        }
        /// <summary>
        /// Gets or sets the image that will be displayed.
        /// </summary>
        [Content]
        public Uri Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        /// <summary>
        /// Gets or sets the source of the image.
        /// </summary>
        [Content]
        public Bitmap? ImageSource
        {
            get => GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
        /// <summary>
        /// get or set image quality
        /// </summary>
        public BitmapInterpolationMode InterpolationMode
        {
            get => GetValue(InterpolationModeProperty);
            set => SetValue(InterpolationModeProperty, value);
        }
        /// <summary>
        /// Raised when the image load failed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Failed
        {
            add { AddHandler(FailedEvent, value); }
            remove { RemoveHandler(FailedEvent, value); }
        }
        /// <summary>
        /// error message if loading failed
        /// </summary>
        public string FailedMessage { get; internal set; }
    }
}
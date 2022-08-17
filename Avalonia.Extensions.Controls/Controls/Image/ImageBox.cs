using Avalonia.Controls;
using Avalonia.Extensions.Media;
using Avalonia.Extensions.Threading;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using Avalonia.Visuals.Media.Imaging;
using System;

namespace Avalonia.Extensions.Controls
{
    /// <summary>
    /// Inherited from <see cref="Image"/>.
    /// Used to display HTTP/HTTPS/Local pictures
    /// </summary>
    public sealed class ImageBox : Image, IBitmapSource
    {
        /// <summary>
        /// original image width
        /// </summary>
        public double ImageWidth { get; internal set; }
        /// <summary>
        /// original image height
        /// </summary>
        public double ImageHeight { get; internal set; }
        private BitmapThread Task { get; }
        private Uri _source;
        public ImageBox() : base()
        {
            Task = new BitmapThread(this);
        }
        /// <summary>
        /// error message if loading failed
        /// </summary>
        public string FailedMessage { get; internal set; }
        /// <summary>
        /// Defines the <see cref="InterpolationMode"/> property.
        /// </summary>
        public static readonly StyledProperty<BitmapInterpolationMode> InterpolationModeProperty =
            AvaloniaProperty.Register<HyperlinkButton, BitmapInterpolationMode>(nameof(InterpolationMode), BitmapInterpolationMode.HighQuality);
        /// <summary>
        /// Defines the <see cref="Source"/> property.
        /// </summary>
        public static new readonly DirectProperty<ImageBox, Uri> SourceProperty =
          AvaloniaProperty.RegisterDirect<ImageBox, Uri>(nameof(Source), o => o.Source, (o, v) => o.Source = v);
        /// <summary>
        /// get or set image url address
        /// </summary>
        [Content]
        public new Uri Source
        {
            get => _source;
            set
            {
                SetAndRaise(SourceProperty, ref _source, value);
                if (value != null)
                    Task.Run(value);
            }
        }
        public Bitmap Bitmap { get; set; }
        /// <summary>
        /// get or set image quality
        /// </summary>
        public BitmapInterpolationMode InterpolationMode
        {
            get => GetValue(InterpolationModeProperty);
            set => SetValue(InterpolationModeProperty, value);
        }
        public void SetSource() => base.Source = Bitmap;
        /// <summary>
        /// Defines the <see cref="Failed"/> property.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> FailedEvent =
            RoutedEvent.Register<ImageBox, RoutedEventArgs>(nameof(Failed), RoutingStrategies.Bubble);
        /// <summary>
        /// Raised when the image load failed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Failed
        {
            add { AddHandler(FailedEvent, value); }
            remove { RemoveHandler(FailedEvent, value); }
        }
        public void ZoomIn(double percentage)
        {
            Width = ImageWidth * percentage;
            Height = ImageHeight * percentage;
        }
    }
}
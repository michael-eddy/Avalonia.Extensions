using Avalonia.Controls.Shapes;
using Avalonia.Extensions.Media;
using Avalonia.Extensions.Threading;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using System;

namespace Avalonia.Extensions.Controls
{
    public sealed class CircleImage : Ellipse, IBitmapSource
    {
        private Uri _source;
        protected override Type StyleKeyOverride => typeof(CircleImage);
        private BitmapThread Task { get; }
        /// <summary>
        /// Defines the <see cref="Source"/> property.
        /// </summary>
        public static readonly DirectProperty<CircleImage, Uri?> SourceProperty =
          AvaloniaProperty.RegisterDirect<CircleImage, Uri?>(nameof(Source), o => o.Source, (o, v) => o.Source = v);
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
        }
        public void SetImageSource(Bitmap bitmap)
        {
            try
            {
                Bitmap?.Dispose();
                if (bitmap != null)
                {
                    Bitmap = bitmap;
                    Fill = new ImageBrush { Source = Bitmap };
                    DrawAgain();
                    SetSize(bitmap.Size);
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
        public Bitmap Bitmap { get; set; }
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
        public Uri? Source
        {
            get => _source;
            set
            {
                SetAndRaise(SourceProperty, ref _source, value);
                Task.Run(value);
            }
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
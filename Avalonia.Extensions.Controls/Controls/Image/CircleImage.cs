using Avalonia.Controls.Shapes;
using Avalonia.Extensions.Threading;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using Avalonia.Threading;
using System;

namespace Avalonia.Extensions.Controls
{
    public sealed class CircleImage : Ellipse
    {
        private DownloadThread Task { get; }
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
            Task = new DownloadThread();
            SourceProperty.Changed.AddClassHandler<CircleImage>(OnSourceChange);
            ImageSourceProperty.Changed.AddClassHandler<CircleImage>(OnImageSourceProperty);
        }
        private void OnImageSourceProperty(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is Bitmap bitmap)
            {
                Fill = new ImageBrush { Source = bitmap };
                DrawAgain();
                SetSize(bitmap.Size);
            }
        }
        private void OnSourceChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is Uri uri)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    switch (uri.Scheme)
                    {
                        case "http":
                        case "https":
                            Task.Create(uri, (result) =>
                            {
                                if (result.Stream != null)
                                {
                                    var bitmap = new Bitmap(result.Stream);
                                    Fill = new ImageBrush { Source = bitmap };
                                    DrawAgain();
                                    SetSize(bitmap.Size);
                                }
                            });
                            break;
                        case "avares":
                            {
                                var assets = Core.Instance.AssetLoader;
                                using var bitmap = new Bitmap(assets.Open(uri));
                                Fill = new ImageBrush { Source = bitmap };
                                DrawAgain();
                                SetSize(bitmap.Size);
                                break;
                            }
                        default:
                            {
                                FailedMessage = "unsupport URI scheme.only support HTTP/HTTPS or avares://";
                                var @event = new RoutedEventArgs(FailedEvent);
                                RaiseEvent(@event);
                                if (!@event.Handled)
                                    @event.Handled = true;
                                break;
                            }
                    }
                });
            }
        }
        private void SetSize(Size size)
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
        public string FailedMessage { get; private set; }
    }
}
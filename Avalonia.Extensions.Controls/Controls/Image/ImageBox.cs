using Avalonia.Controls;
using Avalonia.Extensions.Media;
using Avalonia.Extensions.Threading;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using Avalonia.Threading;
using System;
using System.IO;

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
        public double ImageWidth { get; private set; }
        /// <summary>
        /// original image height
        /// </summary>
        public double ImageHeight { get; private set; }
        private DownloadThread Task { get; }
        private Uri _source;
        public ImageBox() : base()
        {
            Task = new DownloadThread();
        }
        /// <summary>
        /// error message if loading failed
        /// </summary>
        public string FailedMessage { get; private set; }
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
                    LoadBitmap(value);
            }
        }
        public Bitmap BitmapSource
        {
            set => base.Source = value;
        }
        private void LoadBitmap(Uri uri)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                switch (uri.Scheme)
                {
                    case "http":
                    case "https":
                        {
                            Task.Create(uri, OnDrawBitmap);
                            break;
                        }
                    case "avares":
                        {
                            using var stream = Core.Instance.AssetLoader.Open(uri);
                            SetSource(stream);
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
        private void SetSource(Stream stream)
        {
            if (stream != null)
            {
                using var bitmap = new Bitmap(stream);
                var width = Width.ToInt32();
                if (double.IsNaN(Width) || width == 0)
                {
                    Width = ImageWidth = bitmap.PixelSize.Width;
                    Height = ImageHeight = bitmap.PixelSize.Height;
                }
                else
                {
                    ImageWidth = bitmap.PixelSize.Width;
                    ImageHeight = bitmap.PixelSize.Height;
                }
                base.Source = bitmap;
            }
        }
        private void OnDrawBitmap(DownloadThread.Result result)
        {
            if (result.Success)
            {
                Bitmap bitmap;
                var width = Width.ToInt32();
                if (double.IsNaN(Width) || width == 0)
                {
                    bitmap = new Bitmap(result.Stream);
                    Width = ImageWidth = bitmap.PixelSize.Width;
                    Height = ImageHeight = bitmap.PixelSize.Height;
                }
                else
                {
                    bitmap = Bitmap.DecodeToWidth(result.Stream, width);
                    ImageWidth = bitmap.PixelSize.Width;
                    ImageHeight = bitmap.PixelSize.Height;
                }
                base.Source = bitmap;
            }
            else
            {
                FailedMessage = result.Message;
                var @event = new RoutedEventArgs(FailedEvent);
                RaiseEvent(@event);
                if (!@event.Handled)
                    @event.Handled = true;
            }
        }
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
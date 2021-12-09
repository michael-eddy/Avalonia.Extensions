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
        private BitmapThread Task { get; }
        private Uri _source;
        public ImageBox() : base()
        {
            Task = new BitmapThread(this);
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
        public Bitmap Bitmap { get; set; }
        public void SetBitmapSource(Stream stream)
        {
            try
            {
                if (Bitmap != null)
                    Bitmap.Dispose();
                if (stream != null)
                {
                    Bitmap = new Bitmap(stream);
                    var width = Width.ToInt32();
                    if (double.IsNaN(Width) || width == 0)
                    {
                        Width = ImageWidth = Bitmap.PixelSize.Width;
                        Height = ImageHeight = Bitmap.PixelSize.Height;
                    }
                    else
                    {
                        ImageWidth = Bitmap.PixelSize.Width;
                        ImageHeight = Bitmap.PixelSize.Height;
                    }
                    base.Source = Bitmap;
                }
            }
            catch { }
        }
        private void LoadBitmap(Uri uri)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    if (!Task.Create(uri, out string message))
                    {
                        FailedMessage = message;
                        var @event = new RoutedEventArgs(FailedEvent);
                        RaiseEvent(@event);
                        if (!@event.Handled)
                            @event.Handled = true;
                    }
                }
                catch { }
            });
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
using Avalonia.Extensions.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using PCLUntils.Objects;
using System;
using System.IO;

namespace Avalonia.Extensions.Media
{
    public interface IBitmapSource : IStyleable
    {
        Uri Source { get; set; }
        Bitmap Bitmap { get; set; }
        void SetBitmapSource(Stream stream)
        {
            try
            {
                Bitmap?.Dispose();
                if (stream != null)
                {
                    if (this is CircleImage circle)
                    {
                        var width = circle.Width.ToInt32();
                        if (double.IsNaN(circle.Width) || width == 0)
                        {
                            Bitmap = new Bitmap(stream);
                            circle.Width = Bitmap.PixelSize.Width;
                            circle.Height = Bitmap.PixelSize.Height;
                        }
                        else
                            Bitmap = Bitmap.DecodeToWidth(stream, width, circle.InterpolationMode);
                        circle.Fill = new ImageBrush { Source = Bitmap };
                        circle.DrawAgain();
                        circle.SetSize(Bitmap.Size);
                    }
                    else if (this is ImageBox image)
                    {
                        var width = image.Width.ToInt32();
                        if (double.IsNaN(image.Width) || width == 0)
                        {
                            Bitmap = new Bitmap(stream);
                            image.Width = image.ImageWidth = Bitmap.PixelSize.Width;
                            image.Height = image.ImageHeight = Bitmap.PixelSize.Height;
                        }
                        else
                        {
                            Bitmap = Bitmap.DecodeToWidth(stream, width, image.InterpolationMode);
                            image.ImageWidth = Bitmap.PixelSize.Width;
                            image.ImageHeight = Bitmap.PixelSize.Height;
                        }
                        image.SetSource();
                    }
                }
            }
            catch (Exception ex)
            {
                ResultSet(false, ex.Message);
                Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(this, ex.Message);
            }
            finally
            {
                stream?.Dispose();
            }
        }
        void ResultSet(bool success, string message)
        {
            try
            {
                if (!success)
                {
                    if (this is CircleImage circle)
                    {
                        circle.FailedMessage = message;
                        var @event = new RoutedEventArgs(CircleImage.FailedEvent);
                        circle.RaiseEvent(@event);
                        if (!@event.Handled)
                            @event.Handled = true;
                    }
                    else if (this is ImageBox image)
                    {
                        image.FailedMessage = message;
                        var @event = new RoutedEventArgs(ImageBox.FailedEvent);
                        image.RaiseEvent(@event);
                        if (!@event.Handled)
                            @event.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(this, ex.Message);
            }
        }
    }
}
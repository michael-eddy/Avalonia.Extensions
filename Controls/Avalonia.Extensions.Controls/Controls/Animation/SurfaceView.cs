using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using System.Collections.Concurrent;

namespace Avalonia.Extensions.Controls
{
    public class SurfaceView : Control
    {
        private IImage temp;
        public static readonly StyledProperty<Stretch> StretchProperty =
            AvaloniaProperty.Register<SurfaceView, Stretch>(nameof(Stretch), Stretch.Uniform);
        /// <summary>
        /// Gets or sets a value controlling how the image will be stretched.
        /// </summary>
        public Stretch Stretch
        {
            get => GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="Sources"/> property.
        /// </summary>
        public static readonly StyledProperty<ConcurrentStack<IImage>> SourcesProperty =
            AvaloniaProperty.Register<SurfaceView, ConcurrentStack<IImage>>(nameof(Sources), new ConcurrentStack<IImage>());
        /// <summary>
        /// Gets or sets the image that will be displayed.
        /// </summary>
        [Content]
        public ConcurrentStack<IImage> Sources
        {
            get => GetValue(SourcesProperty);
            set => SetValue(SourcesProperty, value);
        }
        static SurfaceView()
        {
            AffectsRender<SurfaceView>(SourcesProperty, StretchProperty);
        }
        public void Enqueue(IImage image) => Sources.Push(image);
        public override void Render(DrawingContext context)
        {
            if (temp is Bitmap b)
                b.Dispose();
            Sources.TryPop(out temp);
            if (temp != null && Bounds.Width > 0 && Bounds.Height > 0)
            {
                var viewPort = new Rect(Bounds.Size);
                var sourceSize = temp.Size;
                var scale = Stretch.CalculateScaling(Bounds.Size, sourceSize, StretchDirection.Both);
                var scaledSize = sourceSize * scale;
                var destRect = viewPort.CenterRect(new Rect(scaledSize)).Intersect(viewPort);
                var sourceRect = new Rect(sourceSize).CenterRect(new Rect(destRect.Size / scale));
                context.DrawImage(temp, sourceRect, destRect);
            }
        }
    }
}
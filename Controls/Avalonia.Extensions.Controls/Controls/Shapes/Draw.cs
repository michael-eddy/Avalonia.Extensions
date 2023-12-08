using Avalonia.Extensions.Media;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using System;

namespace Avalonia.Extensions.Controls
{
    public class Draw : Shape
    {
        private Uri _source;
        protected override Type StyleKeyOverride => typeof(Shape);
        /// <summary>
        /// Defines the <see cref="Source"/> property.
        /// </summary>
        public static readonly DirectProperty<Draw, Uri> SourceProperty =
          AvaloniaProperty.RegisterDirect<Draw, Uri>(nameof(Source), o => o.Source, (o, v) => o.Source = v);
        /// <summary>
        /// get or set image url address
        /// </summary>
        [Content]
        public Uri Source
        {
            get => _source;
            set => SetAndRaise(SourceProperty, ref _source, value);
        }
        /// <summary>
        /// Defines the <see cref="StretchDirection"/> property.
        /// </summary>
        public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
            AvaloniaProperty.Register<Draw, StretchDirection>(nameof(StretchDirection), StretchDirection.Both);
        /// <summary>
        /// Gets or sets a value controlling in what direction the image will be stretched.
        /// </summary>
        public StretchDirection StretchDirection
        {
            get => GetValue(StretchDirectionProperty);
            set => SetValue(StretchDirectionProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="InterpolationMode"/> property.
        /// </summary>
        public static readonly StyledProperty<BitmapInterpolationMode> InterpolationModeProperty =
            AvaloniaProperty.Register<Draw, BitmapInterpolationMode>(nameof(InterpolationMode), BitmapInterpolationMode.MediumQuality);
        /// <summary>
        /// get or set image quality
        /// </summary>
        public BitmapInterpolationMode InterpolationMode
        {
            get => GetValue(InterpolationModeProperty);
            set => SetValue(InterpolationModeProperty, value);
        }
        private Bitmap bitmap;
        private Size? measureSize = null;
        static Draw()
        {
            SourceProperty.Changed.AddClassHandler<Draw>(OnSourceChange);
            AffectsRender<Draw>(SourceProperty, StretchProperty, StretchDirectionProperty);
            AffectsMeasure<Draw>(SourceProperty, StretchProperty, StretchDirectionProperty);
        }
        private static void OnSourceChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && sender is Draw d)
                d.measureSize = null;
        }
        protected override Geometry CreateDefiningGeometry()
        {
            if (measureSize == null)
            {
                bitmap?.Dispose();
                bitmap = this.ToBitmap(Source);
                if (bitmap != null)
                    measureSize = bitmap.Size;
            }
            return new RectangleGeometry(new Rect(measureSize.Value).Deflate(StrokeThickness / 2));
        }
        public override void Render(DrawingContext context)
        {
            var source = bitmap;
            if (source != null && Bounds.Width > 0 && Bounds.Height > 0)
            {
                Rect viewPort = new Rect(Bounds.Size);
                Size sourceSize = source.Size;
                Vector scale = Stretch.CalculateScaling(Bounds.Size, sourceSize, StretchDirection);
                Size scaledSize = sourceSize * scale;
                Rect destRect = viewPort.CenterRect(new Rect(scaledSize)).Intersect(viewPort);
                Rect sourceRect = new Rect(sourceSize).CenterRect(new Rect(destRect.Size / scale));
                context.DrawImage(source, sourceRect, destRect);
            }
        }
    }
}
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Extensions.Styles;
using Avalonia.Metadata;
using Avalonia.Styling;
using System;

namespace Avalonia.Extensions.Controls
{
    public class ImageContentButton : Button, IStyling
    {
        private TextBlock text;
        private ImageBox image;
        Type IStyleable.StyleKey => typeof(Button);
        public ImageContentButton() : base()
        {
            Init();
            ContentProperty.Changed.AddClassHandler<ImageContentButton>(OnContentChange);
            IconSizeProperty.Changed.AddClassHandler<ImageContentButton>(OnIconSizeChange);
            ImageSourceProperty.Changed.AddClassHandler<ImageContentButton>(OnImageSourceChange);
        }
        private void Init()
        {
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            image = new ImageBox
            {
                Source = ImageSource,
                Width = IconSize.Width,
                Height = IconSize.Height,
                VerticalAlignment = VerticalAlignment.Center
            };
            stackPanel.Children.Add(image);
            text = new TextBlock
            {
                Text = Content,
                VerticalAlignment = VerticalAlignment.Center
            };
            stackPanel.Children.Add(text);
            base.Content = stackPanel;
        }
        private void OnContentChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is string chars && text != null)
                text.Text = chars;
        }
        private void OnImageSourceChange(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is Uri source && image != null)
                image.Source = source;
        }
        /// <summary>
        /// Defines the <see cref="Source"/> property.
        /// </summary>
        public static readonly DirectProperty<ImageContentButton, Uri> ImageSourceProperty =
          AvaloniaProperty.RegisterDirect<ImageContentButton, Uri>(nameof(ImageSource), o => o.ImageSource, (o, v) => o.ImageSource = v);
        private Uri _source;
        /// <summary>
        /// get or set image url address
        /// </summary>
        [Content]
        public Uri ImageSource
        {
            get => _source;
            set => SetAndRaise(ImageSourceProperty, ref _source, value);
        }
        public new string Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        public static new readonly StyledProperty<string> ContentProperty =
            AvaloniaProperty.Register<ImageContentButton, string>(nameof(Content));
        public Size IconSize
        {
            get => GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
        public static readonly StyledProperty<Size> IconSizeProperty =
            AvaloniaProperty.Register<ImageContentButton, Size>(nameof(IconSize), new Size(32, 32));
        private void OnIconSizeChange(ImageContentButton obj, AvaloniaPropertyChangedEventArgs e)
        {
            if (!e.IsSameValue() && e.NewValue is Size size && image != null)
            {
                image.Width = size.Width;
                image.Height = size.Height;
            }
        }
    }
}
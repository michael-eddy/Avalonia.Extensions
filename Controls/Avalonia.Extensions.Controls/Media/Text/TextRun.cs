using Avalonia.Metadata;

namespace Avalonia.Extensions.Media
{
    public sealed class TextRun : AvaloniaObject
    {
        /// <summary>
        /// Defines the <see cref="Content"/> property.
        /// </summary>
        public static readonly StyledProperty<string> ContentProperty =
            AvaloniaProperty.Register<TextRun, string>(nameof(Content));
        /// <summary>
        /// Gets or sets the content to display.
        /// </summary>
        [Content]
        public string Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
    }
}
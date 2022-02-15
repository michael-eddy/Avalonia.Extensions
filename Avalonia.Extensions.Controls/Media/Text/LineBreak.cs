using Avalonia.Metadata;

namespace Avalonia.Extensions.Media
{
    public sealed class LineBreak : AvaloniaObject
    {
        public LineBreak()
        {
            SetValue(ContentProperty, "\r\n");
        }
        /// <summary>
        /// Defines the <see cref="Content"/> property.
        /// </summary>
        internal static readonly StyledProperty<string> ContentProperty =
            AvaloniaProperty.Register<LineBreak, string>(nameof(Content));
        /// <summary>
        /// Gets or sets the content to display.
        /// </summary>
        [Content]
        internal string Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
    }
}
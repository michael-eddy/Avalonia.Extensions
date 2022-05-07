using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Extensions.Styles;
using Avalonia.Media;

namespace Avalonia.Extensions.Controls
{
    public class ImageIcon : IconElement, IStyling
    {
        /// <inheritdoc cref="Image.SourceProperty" />
        public static readonly StyledProperty<IImage> SourceProperty = Image.SourceProperty.AddOwner<ImageIcon>();
        /// <inheritdoc cref="Image.Source" />
        public IImage Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        static ImageIcon()
        {
            AffectsRender<ImageIcon>(SourceProperty);
        }
        public ImageIcon()
        {
            this.AddResource();
        }
    }
    /// <summary>
    /// Represents an icon source that uses a image source as its content.
    /// </summary>
    public class ImageIconSource : IconSource
    {
        /// <inheritdoc cref="Image.SourceProperty" />
        public static readonly StyledProperty<IImage> SourceProperty = Image.SourceProperty.AddOwner<ImageIcon>();
        /// <inheritdoc cref="Image.Source" />
        public IImage Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        public override IDataTemplate IconElementTemplate { get; } = new FuncDataTemplate<ImageIconSource>((source, _) => new ImageIcon
        {
            [!ForegroundProperty] = source[!ForegroundProperty],
            [!SourceProperty] = source[!SourceProperty]
        });
    }
}
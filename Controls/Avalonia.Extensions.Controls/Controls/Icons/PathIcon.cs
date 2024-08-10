using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace Avalonia.Extensions.Controls.Icons
{
    public class PathIcon : IconElement
    {
        static PathIcon()
        {
            AffectsRender<PathIcon>(DataProperty);
        }
        protected static readonly StyledProperty<Geometry> DataProperty = Path.DataProperty.AddOwner<PathIcon>();
        public Geometry Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
    }
    public class PathIconSource : IconSource
    {
        protected static readonly StyledProperty<Geometry> DataProperty = Path.DataProperty.AddOwner<PathIcon>();
        public Geometry Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
        public override IDataTemplate IconElementTemplate { get; } = new FuncDataTemplate<PathIconSource>((source, _) => new PathIcon
        {
            [!ForegroundProperty] = source[!ForegroundProperty],
            [!DataProperty] = source[!DataProperty]
        });
    }
}
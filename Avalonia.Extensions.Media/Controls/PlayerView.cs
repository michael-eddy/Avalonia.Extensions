using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace Avalonia.Extensions.Media
{
    public class PlayerView : TemplatedControl
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            DrawTemplate();
        }
        private void DrawTemplate()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));



            Template = new FuncControlTemplate((_, _) => new Decorator { Child = grid });
            ApplyTemplate();
        }
    }
}
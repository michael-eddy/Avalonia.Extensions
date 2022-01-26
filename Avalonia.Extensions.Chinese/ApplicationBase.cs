using Avalonia.Platform;

namespace Avalonia.Extensions.Controls
{
    public abstract class ApplicationBase : Application
    {
        public override void RegisterServices()
        {
            AvaloniaLocator.CurrentMutable.Bind<IFontManagerImpl>().ToConstant(new FontManagerImpl());
            base.RegisterServices();
        }
    }
}
# Controls Extensions for Avalonia

> # LANGUAGE

- [x] Chinese Input Support
_Need to reference [Dove.Avalonia.Controls.Extensions.ChineseInputSupport](https://www.nuget.org/packages/Dove.Avalonia.Controls.Extensions.ChineseInputSupoort/) package_

To enable extension the `UseChineseInputSupport` method call should be present in your Program.cs file,you defined which controls(default is TextBox and TextPresenter):
```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseChineseInputSupport()
        .LogToTrace();
```
If you need to use a global font in the App, you need to inherit `ApplicationBase` in the `App.xaml.cs` class
# Controls Extensions for Avalonia

# WARNING
**<span style="color:red;">_Avalonia 11 or above,you should use v2.0.0 or above!!!_</span>**

> # LANGUAGE

- [x] Chinese Input Support
_Need to reference [Dove.Avalonia.Controls.Extensions.ChineseInputSupport](https://www.nuget.org/packages/Dove.Avalonia.Controls.Extensions.ChineseInputSupoort/) package_

To enable extension the `UseChineseInputSupport` method call should be present in your Program.cs file:
```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseChineseInputSupport()
        .LogToTrace();
```
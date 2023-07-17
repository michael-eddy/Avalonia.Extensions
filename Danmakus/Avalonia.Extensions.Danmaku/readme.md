# Controls Extensions for Avalonia

# WARNING
**<span style="color:red;">_Avalonia 11 or above,you should use v2.0.0 or above!!!_</span>**

## INPORTANT
To enable extension the `UseDoveExtensions` method call should be present in your Program.cs file:

```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseDoveExtensions()
        .LogToTrace();
```

> # DANMAKU

# WARNING
_Need to reference [Dove.Avalonia.Controls.Extensions.Danmaku](https://www.nuget.org/packages/Dove.Avalonia.Controls.Extensions.Danmaku/) package_

> # CONTROLS

- [x] DanmakuView
  > use libwtfdanmaku libs draw danmaku,only work's on Windows

- [x] DanmakuNativeView
  > use Avalonia Animation draw danmaku,for all platforms 
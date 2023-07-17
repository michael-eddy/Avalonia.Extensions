# Controls Extensions for Avalonia

# 中文描述
[点击这里](/readme-zh.md)

# WARNING
**<span style="color:red;">_Avalonia 11 or above,you should use v2.0.0 or above!!!_</span>**

> # WebView

- [x] WebView
- [x] WebViewTab
- [x] WebViewGlue
_Need to reference [Dove.Avalonia.Extensions.WebView](https://www.nuget.org/packages/Dove.Avalonia.Extensions.WebView/) package_

To enable extension the `UseWebView` method call should be present in your Program.cs file:
Auto download cef:
```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseWebView(autoDownload,eventHandler)
        .LogToTrace();
```

or

seting cef path and with setting:
```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseWebView(cefPath,cefSettings,eventHandler)
        .LogToTrace();
```
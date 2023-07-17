# Avalonia控件扩展库

# 注意
**<span style="color:red;">_Avalonia 11或以上版本需要安装v2.0.0或以上版本！！！_</span>**

> # WebView

- [x] WebView
- [x] WebViewTab
- [x] WebViewGlue
_需要引用包[Dove.Avalonia.Extensions.WebView](https://www.nuget.org/packages/Dove.Avalonia.Extensions.WebView/)_

需要在Program.cs文件调用方法`UseWebView`后进行使用:
自动下载cef库使用:
```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseWebView(autoDownload,eventHandler)
        .LogToTrace();
```

或者

指定cef路径并使用配置:
```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseWebView(cefPath,cefSettings,eventHandler)
        .LogToTrace();
```
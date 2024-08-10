# Avalonia控件扩展库

# 注意
**<span style="color:red;">_Avalonia 11或以上版本,需要安装v2.0.0或以上版本！！！_</span>**

## 信息
启用控件库需要调用`UseDoveExtensions`方法在Program.cs文件中:

```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseDoveExtensions()
        .LogToTrace();
```

> # 弹幕

# 注意
_需要引用包[Dove.Avalonia.Controls.Extensions.Danmaku](https://www.nuget.org/packages/Dove.Avalonia.Controls.Extensions.Danmaku/)_

> # 控件

- [x] DanmakuView
  > 使用libwtfdanmaku库渲染弹，只支持Windows

- [x] DanmakuNativeView
  > 使用Avalonia Animation渲染弹幕，支持所有平台
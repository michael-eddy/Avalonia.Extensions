# Avalonia控件扩展

# 注意
**<span style="color:red;">_Avalonia 11或以上版本,需要安装v2.0.0或以上版本！！！_</span>**

> # 语言

- [x] 中文输入/显示支持
_需要引用包[Dove.Avalonia.Controls.Extensions.ChineseInputSupport](https://www.nuget.org/packages/Dove.Avalonia.Controls.Extensions.ChineseInputSupoort/) _

启用扩展需要调用`UseChineseInputSupport`方法在Program.cs文件中:
```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseChineseInputSupport()
        .LogToTrace();
```
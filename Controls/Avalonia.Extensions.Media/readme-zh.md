# Avalonia媒体控件扩展库

# 注意
**<span style="color:red;">_Avalonia 11或以上版本需要安装v2.0.0或以上版本！！！_</span>**

_在[Avalonia 0.10.14](https://www.nuget.org/packages/Avalonia/)或以上版本，Windows中使用NativeControlHost需要配置app.manifest文件。否则会导致`PlayerView`无法正常工作！_
```xml
<?xml version="1.0" encoding="utf-8"?>
<assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
  <assemblyIdentity version="1.0.0.0" name="MyApplication.app"/>
  <trustInfo xmlns="urn:schemas-microsoft-com:asm.v2">
    <security>
      <requestedPrivileges xmlns="urn:schemas-microsoft-com:asm.v3">
        <requestedExecutionLevel level="asInvoker" uiAccess="false" />
      </requestedPrivileges>
    </security>
  </trustInfo>
  <compatibility xmlns="urn:schemas-microsoft-com:compatibility.v1">
    <application>
      <!-- Windows 7 -->
      <!--<supportedOS Id="{35138b9a-5d96-4fbd-8e2d-a2440225f93a}" />-->
      <!-- Windows 8 -->
      <!--<supportedOS Id="{4a2f28e3-53b9-4441-ba9c-d69d4a4a6e38}" />-->
      <!-- Windows 8.1 -->
      <!--<supportedOS Id="{1f676c76-80e1-4239-95bb-83d0f6d0da78}" />-->
      <!-- Windows 10 -->
      <!--<supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />-->
    </application>
  </compatibility>
</assembly>
```

## 信息

启用控件库需要调用`UseDoveExtensions`方法在Program.cs文件中:

```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseDoveExtensions()
        .LogToTrace();
```

---

> # 媒体

# 注意
_需要引用包[Dove.Avalonia.Extensions.Media](https://www.nuget.org/packages/Dove.Avalonia.Extensions.Media/)_

_++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++_

~~_现在1.2.0版本之后,支持ffmpeg播放视频啦！！！_~~
请使用[FFmpegView.Avalonia](https://www.nuget.org/packages/FFmpegView.Avalonia) 替换旧版本

_对了，你也需要安装ffmpeg库[Windows](https://www.nuget.org/packages/Dove.FFmpeg.Windows) or [Linux](https://www.nuget.org/packages/Dove.FFmpeg.Linux)或者[MacOS](https://www.nuget.org/packages/Dove.FFmpeg.OSX)_

_还有你也需要引用bass库[Windows](https://www.nuget.org/packages/Dove.Bass.Windows)或者[Linux](https://www.nuget.org/packages/Dove.Bass.Linux)或者[MacOS](https://www.nuget.org/packages/Dove.Bass.OSX)_

_以及[ManagedBass](https://github.com/ManagedBass/ManagedBass)，要求必须放置于你的程序运行根目录下，包[Dove.Avalonia.Extensions.Media]会放置于上述所描述的位置.如果你仍然捕获到异常提示,请你手动再执行一次！！！_

_++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++_

_如果你需要使用`VideoView` 和`PlayerView`，你需要在项目中安装ibVLC ！！！_

_如果你需要使用`FFmpegView`，你需要在项目中安装libFfmpeg并初始化！！！_

_如果你需要使用`AudioControl`，需要在项目中安装libbass并初始化！！！_

如果需要启用`UseAudioControl`和`UseVideoView`需要在Program.cs文件中调用一些方法，下面第3行对应`AudioControl`，
第4、5行则对应`VideoView`/`PlayerView`，你可以根据你的需求进行调用：

```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseAudioControl()
        .UseVideoView()
        .UseFFmpeg()
        .LogToTrace();
```

- [x] VideoView控件
  > `LibVLCSharp`中的`VideoView`，基于`UserControl`修改实现

- [x] FFmpegView控件
  > 使用ffmpeg渲染的控件

- [x] PlayerView控件
  > 自定义控件`VideoView`，类似于UWP/WPF的MediaElement

- [x] AudioControl控件
  > 播放音频的控件（没有界面）
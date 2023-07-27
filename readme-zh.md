# Avalonia控件扩展库

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

# 向导
  [关于媒体扩展](#media)

  [关于控件扩展](#controls)

  [关于弹幕扩展](#danmaku)

  [关于中文扩展](#language)

## 演示截图
![图片alt](/ss.png "截图")
---

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

> # 控件

- [x] ProgressRing
  > 由Canvas扩展而来，显示加载效果

- [x] ClickableView
  > 可以触发点击时间的控件

- [x] GridView
  > 一个多列的ListView/ListBox，类似于UWP的GridView

- [x] GridViewItem
  > GridView的子项，继承自ListViewItem，拥有左/右点击事件

- [x] CircleImage
  > 继承于Ellipse，显示园形图片。等同于UWP的PersonPicture

- [x] ExpandableView
  > 垂直滚动的两层显示/隐藏控件
  > PrimaryView
  > > 标头，选择后可显示或者隐藏SecondView
  > SecondView
  > > 选择PrimaryView之后显示或者隐藏

- [x] ImageBox
  > 继承于Image，支持加载http/https/local协议的图片

- [x] ListView
  > 继承于ListBox，类同于UWP的ListView 

- [x] ListViewItem
  > ListView的子项，继承于ListBoxItem

- [x] MessageBox
  > 消息窗口

- [x] NotifyWindow
  > 信息通知窗口，过渡动画可根据配置显示，一定时间后自动关闭

- [x] PopupMenu
  > 继承于Window，选择项之后并触发事件

- [x] HorizontalItemsRepeater
  > 继承于ItemsRepeater，水平方向布局附带点击事件

- [x] VerticalItemsRepeater
  > 继承于ItemsRepeater，垂直方向布局附带点击事件

- [x] ItemsRepeaterContent
  > ItemsRepeater的子项附带点击事件

- [x] ScrollView
  > 继承于ScrollViewer，扩展滚动到结尾/顶部事件

- [x] SeekSlider
  > 继承于`Slider`，附带值变更事件

- [x] PopupToast
  > 继承于Window，显示消息对话框并在一段时间后自动关闭
  > > 不同于`PopupDialog`， 它在工作区中内控件/面板中显示

- [x] TextLabel
  > Run标签进行绑定字符，类同WPF/UWP的`TextBlock`，`LineBreak` 可以进行手动换行

- [x] HyperlinkButton
  > 按钮风格类同于UWP的HyperlinkButton

- [x] AeroWindow
  > 窗口效果如同"Windows Aero"或者"Aero Grass"，修改`MoveDragEnable`的值为`true`可以获取到鼠标在窗口的位置

- [x] ImageContentButton
  > 渲染图片和文本的按钮

- [x] CheckBoxList
  > 一个多选项的CheckBox控件

- [x] RadioButtonList
  > 一个多选项的RadioButton控件

- [x] Text
  > 基于`Shapes`渲染的文本内容，是`Canvas`的子控件

- [x] TipLabel
  > 拥有边框的`TextBlock`

- [x] TextView  (v2.0.0以前)
  > 同等与`TextPresenter`，但由TextLayout进行渲染，所支持你项目中的自定义字体

- [x] EditTextView  (v2.0.0以前)
  > 同等与`TexBox`，但由TextLayout进行渲染，所支持你项目中的自定义字体

- [x] PathIcon
  > 等同于UWP的控件

- [x] ImageIcon
  > 加载图片的Icon控件，类同`PathIcon`和`SymbolIcon` 

- [x] SymbolIcon
  > 等同于UWP的控件(`Glyph`的值仅支持在XAML中定义)

- [x] NotifyIcon
  > 通知控件，当前仅支持Windows

- [x] PaginationView
  > `ItemControl`的分页控件(`Glyph`的值仅支持在XAML中定义)

- [x] SurfaceView
  > 如同Image的绘制控件，设置`Sources`的值将会自动刷新

> # 扩展

- [x] GetHwnd
  > 获取控件或者窗口句柄

- [x] GetScreenSize
  > 获取屏幕尺寸(工作区或者显示器)

- [x] ActualWidth
  > 获取可见控件真实宽度

- [x] ActualHeight
  > 获取可见控件真实高度

- [x] GetPrivateField
  > 获取控件私有字段

- [x] SetPrivateField

  > 对控件的私有字段设置值

- [x] GetPrivateProperty

  > 获取控件私有属性

- [x] SetPrivateProperty

  > 对控件的私有属性设置值

- [x] MeasureString

  > 获取字符串文本显示长度

- [x] AreClose

  > 值之间的差距是否相邻

- [x] InvokePrivateMethod

  > 调用私有的方法

- [x] FindControls

  > 通过控件类型遍历获取对应控件

- [x] GetWindow
  > 获取拥有该控件的窗口

- [x] Shutdown / TryShutdown
  > 关闭应用程序
 
> # 弹幕

# 注意
_需要引用包[Dove.Avalonia.Controls.Extensions.Danmaku](https://www.nuget.org/packages/Dove.Avalonia.Controls.Extensions.Danmaku/)_

> # 控件

- [x] DanmakuView
  > 使用libwtfdanmaku库渲染弹，只支持Windows

- [x] DanmakuNativeView
  > 使用Avalonia Animation渲染弹幕，支持所有平台

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
  
> # 语言

- [x] 中文输入/显示支持
_需要引用包[Dove.Avalonia.Controls.Extensions.ChineseInputSupport](https://www.nuget.org/packages/Dove.Avalonia.Controls.Extensions.ChineseInputSupoort/)_

启用扩展需要调用`UseChineseInputSupport`方法在Program.cs文件中:
```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseChineseInputSupport()
        .LogToTrace();
```
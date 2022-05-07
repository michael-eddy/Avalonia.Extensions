# Controls Extensions for Avalonia

---

## DEMO screenshot
![图片alt](/ss.png "截图")
---

## INPORTANT

To enable extension the `UseDoveExtensions` method call should be present in your Program.cs file:

```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseDoveExtensions()
        .LogToTrace();
```

---

## ABOUT FOLDER

> Base
> > Basic, public library

> Controls
> > Control extension implementation

> Utils
> > Extension Method Library

---

## TODO LIST

> # CONTROLS

- [x] ProgressRing
  > inherit from Canvas,Implement loading animation

- [x] ClickableView
  > Views that can trigger the click event

- [x] GridView
  > a Multiple-Column ListView/ListBox,Just like GridView in UWP

- [x] GridViewItem
  > Item for GridViewItem,inherit from ListViewItem,with Left/Right Click Event

- [x] CircleImage
  > inherit from Ellipse,Round picture.Just like PersonPicture in UWP

- [x] ExpandableView
  > A view that shows items in a vertically scrolling two-level list
  > PrimaryView
  > > Main Item,show/hide the SecondView after selection
  > SecondView
  > > show or hide when select PrimaryView

- [x] ImageBox
  > inherit from Image,loading image from http/https/local

- [x] ListView
  > inherit from ListBox,just like the ListView in UWP

- [x] ListViewItem
  > Item for ListView,inherit from ListBoxItem

- [x] MessageBox
  > Show message window

- [x] NotifyWindow
  > Notify message window,the transition animation can be displayed according to the preset and automatically closed after a certain period of time

- [x] PopupMenu
  > inherit from Window,close after selecting item and trigger the event

- [x] HorizontalItemsRepeater
  > inherit from ItemsRepeater,Horizontal layout with Clickable Item

- [x] VerticalItemsRepeater
  > inherit from ItemsRepeater,Vertical layout with Clickable Item

- [x] ItemsRepeaterContent
  > Item for ItemsRepeater with Clickable Event

- [x] ScrollView
  > inherit from ScrollViewer,extend sliding to the bottom, sliding to the top event

- [x] SeekSlider
  > inherit from `Slider`,extend value change event

- [x] PopupToast
  > inherit from Window,show message dialog and automatically shut down after a certain period of time
  > > diff with `PopupDialog`, it's popuping in workarea who popuping in control/panel

- [x] TextLabel
  > Run binding just LIKE WPF/UWP in `TextBlock`,with `LineBreak` can wrap the text

- [x] HyperlinkButton
  > the button style LIKE HyperlinkButton in UWP

- [x] AeroWindow
  > the window LIKE "Windows Aero" OR "Aero Grass"

- [x] ImageContentButton
  > the button show Image and Text

- [x] CheckBoxList
  > a grouping checkbox control

- [x] RadioButtonList
  > a grouping radiobutton control

- [x] Text
  > the `Shapes` control for text content,it is a child when you are using `canvas`

- [x] TipLabel
  > the `TextBlock` with border control

- [x] TextView
  > the control like `TextPresenter`,but it's draw by the textLayout,so supoort custom font in your project

- [x] PathIcon
  > the same control in uwp

- [x] ImageIcon
  > the icon control load from image,just like `PathIcon` and  `SymbolIcon` 

- [x] SymbolIcon
  > the same control in uwp(but the `Glyph` Only valid in XAML)

> # EXTEND

- [x] ActualWidth
  > get visable control actual width

- [x] ActualHeight
  > get visable control actual height

- [x] GetPrivateField
  > get control/namescope private field

- [x] SetPrivateField

  > set value for control private field

- [x] GetPrivateProperty

  > get control private property

- [x] SetPrivateProperty

  > set value for control private property

- [x] MeasureString

  > get string text display width

- [x] AreClose

  > between value is too close or not

- [x] InvokePrivateMethod

  > call control private method

- [x] FindControls

  > find child control by control type

- [x] GetWindow
  > get window who own this control

- [x] Shutdown / TryShutdown
  > shutdown application

> # MEDIA

# WARNING
Need to reference [Dove.Avalonia.Extensions.Media](https://www.nuget.org/packages/Dove.Avalonia.Extensions.Media/) package

$\color{red}{if you need `VideoView` or `PlayerView`,you should install libVLC in your project before init!!! [Windows](https://www.nuget.org/packages/VideoLAN.LibVLC.Windows/) or [MAC](https://www.nuget.org/packages/VideoLAN.LibVLC.Mac/)}$

<b>if you need `AudioControl`,should download bass Libraries(.dll/.so/.dylib/.a) are separate for x86, x64, ARM by yourself. you can find out from [here](https://github.com/ManagedBass/ManagedBass)</b>

To enable extension the `UseAudioControl` AND `UseVideoView` method call should be present in your Program.cs file,the 1st one is for `AudioControl`,
the 2nd one is for `VideoView` or `PlayerView`,you can also call it according to your NEEDS:

```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseVideoView()
        .UseAudioControl()
        .LogToTrace();
```

- [x] VideoView Support
  > the `VideoView` in `LibVLCSharp`,modified to support use in `UserControl`

- [x] PlayerView Support
  > the custom control for `VideoView`,it's just like in uwp/wpf

- [x] AudioControl Support
  > the control playing audio without visual or occupy bounds

> # LANGUAGE

- [x] Chinese Input Support
Need to reference [Dove.Avalonia.Controls.Extensions.ChineseInputSupport](https://www.nuget.org/packages/Dove.Avalonia.Controls.Extensions.ChineseInputSupoort/) package

To enable extension the `UseChineseInputSupport` method call should be present in your Program.cs file,you defined which controls(default is TextBox and TextPresenter):
```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseChineseInputSupport()
        .LogToTrace();
```
If you need to use a global font in the App, you need to inherit `ApplicationBase` in the `App.xaml.cs` class
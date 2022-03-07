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

- [x] PopupToast
  > inherit from Window,show message dialog and automatically shut down after a certain period of time
  > > diff with `PopupDialog`, it's popuping in workarea who popuping in control/panel

- [x] TextLabel
> Run binding just LIKE WPF/UWP in TextBlock,with LineBreak can wrap the text

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
> the shapes control for text content,it is a child when you are using `canvas`

- [x] TextView
> the control like `TextPresenter`,but it's draw by the textLayout,so supoort custom font in your project

> # EXTEND

- [x] Chinese Input Support

To enable extension the `UseChineseInputSupport` method call should be present in your Program.cs file,you defined which controls(default is TextBox and TextPresenter):
```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseChineseInputSupport()
        .LogToTrace();
```

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
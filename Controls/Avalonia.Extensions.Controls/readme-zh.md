# Avalonia控件库

# 注意
**<span style="color:red;">_Avalonia 11或以上版本，需要安装v2.0.0或以上版本！！！_</span>**

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
  > A view that shows items in a vertically scrolling two-level list
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
  > Notify message window，the transition animation can be displayed according to the preset and automatically closed after a certain period of time

- [x] PopupMenu
  > inherit from Window，close after selecting item and trigger the event

- [x] HorizontalItemsRepeater
  > inherit from ItemsRepeater，Horizontal layout with Clickable Item

- [x] VerticalItemsRepeater
  > inherit from ItemsRepeater，Vertical layout with Clickable Item

- [x] ItemsRepeaterContent
  > Item for ItemsRepeater with Clickable Event

- [x] ScrollView
  > inherit from ScrollViewer，extend sliding to the bottom， sliding to the top event

- [x] SeekSlider
  > 继承于`Slider`，附带值变更事件

- [x] PopupToast
  > inherit from Window，show message dialog and automatically shut down after a certain period of time
  > > diff with `PopupDialog`， it's popuping in workarea who popuping in control/panel

- [x] TextLabel
  > Run标签进行绑定字符，类同WPF/UWP的`TextBlock`，with `LineBreak` can wrap the text

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

- [x] TextView  (below v2.0.0)
  > 同等与`TextPresenter`，但由TextLayout进行渲染，所支持你项目中的自定义字体

- [x] EditTextView  (below v2.0.0)
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
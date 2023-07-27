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
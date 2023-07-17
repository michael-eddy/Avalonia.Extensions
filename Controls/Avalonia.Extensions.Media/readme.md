# Controls Extensions for Avalonia

# WARNING
**<span style="color:red;">_Avalonia 11 or above,you should use v2.0.0 or above!!!_</span>**

_IN [Avalonia 0.10.14](https://www.nuget.org/packages/Avalonia/),the NativeControlHost NEED app.manifest for Windows.IF NOT,THAT'S MAKE THE `PlayerView` CANNOT WORKS NORMALLY!_
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

> # MEDIA

# WARNING
_Need to reference [Dove.Avalonia.Extensions.Media](https://www.nuget.org/packages/Dove.Avalonia.Extensions.Media/) package_

_++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++_

~~_NOW the Version >= 1.2.0,it's able to use ffmpeg playing video!!!!_~~
Please use [FFmpegView.Avalonia](https://www.nuget.org/packages/FFmpegView.Avalonia) replace since Version >= 1.2.5

_Ah ! You can reference the ffmpeg libraries packages from [Windows](https://www.nuget.org/packages/Dove.FFmpeg.Windows) or [Linux](https://www.nuget.org/packages/Dove.FFmpeg.Linux) or [MacOS](https://www.nuget.org/packages/Dove.FFmpeg.OSX)_

_OH ! You can reference the bass libraries packages from [Windows](https://www.nuget.org/packages/Dove.Bass.Windows) or [Linux](https://www.nuget.org/packages/Dove.Bass.Linux) or [MacOS](https://www.nuget.org/packages/Dove.Bass.OSX)_

_and the [ManagedBass](https://github.com/ManagedBass/ManagedBass) require the libraries is in your program root folder,the [Dove.Avalonia.Extensions.Media] has handle copy to the require path.but if you still get an init failed messsage,plz copy by yourself!!_

_++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++_

_if you need `VideoView` or `PlayerView`,you should install libVLC in your project before init!!!_

_if you need `FFmpegView`,you should install libFfmpeg in your project before init!!!_

_if you need `AudioControl`,should install libbass in your project before init!!!_

To enable extension the `UseAudioControl` AND `UseVideoView` method call should be present in your Program.cs file,the 1st one is for `AudioControl`,
the 2nd one is for `VideoView` or `PlayerView`,you can also call it according to your NEEDS:

```csharp
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseVideoView()
        .UseAudioControl()
        .UseFFmpeg()
        .LogToTrace();
```

- [x] VideoView Support
  > the `VideoView` in `LibVLCSharp`,modified to support use in `UserControl`

- [x] FFmpegView Support
  > the video view by using ffmpeg

- [x] PlayerView Support
  > the custom control for `VideoView`,it's just like in uwp/wpf

- [x] AudioControl Support
  > the control playing audio without visual or occupy bounds
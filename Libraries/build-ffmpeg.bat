@echo off
echo "build windows package"
nuget.exe pack FFmpeg\FFmpeg.Windows\Dove.FFmpeg.Windows.nuspec -Version 1.0.0

echo "build macos package"
nuget.exe pack FFmpeg\FFmpeg.OSX\Dove.FFmpeg.OSX.nuspec -Version 1.0.0

echo "build linux package"
nuget.exe pack FFmpeg\FFmpeg.Linux\Dove.FFmpeg.Linux.nuspec -Version 1.0.0

pause
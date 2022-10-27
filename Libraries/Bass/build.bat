@echo off
echo "build windows package"
nuget.exe pack Bass.Windows\Dove.Bass.Windows.nuspec -Version 1.0.0

echo "build macos package"
nuget.exe pack Bass.OSX\Dove.Bass.OSX.nuspec -Version 1.0.0

echo "build linux package"
nuget.exe pack Bass.Linux\Dove.Bass.Linux.nuspec -Version 1.0.0

pause
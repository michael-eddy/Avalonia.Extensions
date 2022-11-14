@echo off
set root=%cd%
dotnet tool install --global dotnet-deb
dotnet tool install --global dotnet-zip
set Buildx64=%root%\linux-x64
::linux-x64
echo ÕýÔÚ±àÒëx64°æ±¾...
dotnet restore -r linux-x64
dotnet deb install
dotnet msbuild Avalonia.Extensions.Demo.csproj -property:OutDir=%Buildx64% -t:CreateDeb -p:TargetFramework=net6.0 -p:RuntimeIdentifier=linux-x64 -p:Configuration=Debug
move %Buildx64%\*.deb %root%\
rd /s/q %Buildx64%
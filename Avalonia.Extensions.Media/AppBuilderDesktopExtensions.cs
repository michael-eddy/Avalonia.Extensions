using Avalonia.Controls;
using LibVLCSharp.Shared;
using System;

namespace Avalonia.Extensions
{
    public static class AppBuilderDesktopExtensions
    {
        public static TAppBuilder UseVlcPlayer<TAppBuilder>(this TAppBuilder builder, params Type[] supportContols)
       where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup((_) => { Core.Initialize(); });
            return builder;
        }
    }
}
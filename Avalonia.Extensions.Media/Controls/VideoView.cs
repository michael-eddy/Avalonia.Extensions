using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Platform;
using CSharpFunctionalExtensions;
using LibVLCSharp.Shared;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Media
{
    public class VideoView : NativeControlHost
    {
        public static readonly DirectProperty<VideoView, Maybe<MediaPlayer>> MediaPlayerProperty =
            AvaloniaProperty.RegisterDirect<VideoView, Maybe<MediaPlayer>>(nameof(MediaPlayer), o => o.MediaPlayer, (o, v) => o.MediaPlayer = v.GetValueOrDefault(),
                defaultBindingMode: BindingMode.TwoWay);
        private readonly IDisposable attacher;
        private readonly BehaviorSubject<Maybe<MediaPlayer>> mediaPlayers = new(Maybe<MediaPlayer>.None);
        private readonly BehaviorSubject<Maybe<IPlatformHandle>> platformHandles = new(Maybe<IPlatformHandle>.None);
        public VideoView()
        {
            attacher = platformHandles.WithLatestFrom(mediaPlayers).Subscribe(x =>
            { (from h in x.First from mp in x.Second select new { n = h, m = mp }).Execute(a => a.m.SetHandle(a.n)); });
        }
        public MediaPlayer? MediaPlayer
        {
            get => mediaPlayers.Value.GetValueOrDefault();
            set => mediaPlayers.OnNext(value);
        }
        /// <inheritdoc />
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            var handle = base.CreateNativeControlCore(parent);
            platformHandles.OnNext(Maybe<IPlatformHandle>.From(handle));
            return handle;
        }
        /// <inheritdoc />
        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            attacher.Dispose();
            base.DestroyNativeControlCore(control);
            mediaPlayers.Value.Execute(MediaPlayerExtensions.DisposeHandle);
        }
    }
    public static class MediaPlayerExtensions
    {
        public static void DisposeHandle(this MediaPlayer player)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                player.Hwnd = IntPtr.Zero;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                player.XWindow = 0;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                player.NsObject = IntPtr.Zero;
        }
        public static void SetHandle(this MediaPlayer player, IPlatformHandle handle)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                player.Hwnd = handle.Handle;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                player.XWindow = (uint)handle.Handle;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                player.NsObject = handle.Handle;
        }
    }
}
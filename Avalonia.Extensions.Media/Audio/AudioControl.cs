using Avalonia.Controls;
using ManagedBass;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;

namespace Avalonia.Extensions.Media
{
    public class AudioControl : UserControl
    {
        private int _req;
        private int _chan;
        private Task task;
        private readonly Timer _timer;
        static readonly object Lock = new object();
        public string IcyMeta { get; private set; }
        public PlayStatus Status { get; private set; }
        public string TitleAndArtist { get; private set; }
        ~AudioControl()
        {
            Bass.Free();
        }
        public AudioControl()
        {
            Bass.NetPlaylist = 1;
            Bass.NetPreBuffer = 0;
            _timer = new Timer(80);
            _timer.Elapsed += Timer_Elapsed;
        }
        public void Play(string url)
        {
            TitleAndArtist = IcyMeta = null;
            Bass.NetAgent =  UserAgent;
            Bass.NetProxy = string.IsNullOrEmpty(Proxy) ? null : Proxy;
            task?.Dispose();
            task = Task.Factory.StartNew(() =>
            {
                int r;
                lock (Lock)
                    r = ++_req;
                _timer.Stop();
                Bass.StreamFree(_chan);
                Status = PlayStatus.Buffering;
                var c = Bass.CreateStream(url, 0, BassFlags.StreamDownloadBlocks | BassFlags.StreamStatus | BassFlags.AutoFree, StatusProc, new IntPtr(r));
                lock (Lock)
                {
                    if (r != _req)
                    {
                        if (c != 0)
                            Bass.StreamFree(c);
                        return;
                    }
                    _chan = c;
                }
                if (_chan == 0)
                    Status = PlayStatus.Error;
                else
                    _timer.Start();
            });
        }
        /// <summary>
        /// Defines the <see cref="Proxy"/> property.
        /// </summary>
        public static readonly StyledProperty<string> ProxyProperty =
            AvaloniaProperty.Register<AudioControl, string>(nameof(Proxy), null);
        /// <summary>
        /// 
        /// </summary>
        public string Proxy
        {
            get => GetValue(ProxyProperty);
            set => SetValue(ProxyProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="UserAgent"/> property.
        /// </summary>
        public static readonly StyledProperty<string> UserAgentProperty =
            AvaloniaProperty.Register<AudioControl, string>(nameof(UserAgent), null);
        /// <summary>
        /// 
        /// </summary>
        public string UserAgent
        {
            get => GetValue(UserAgentProperty);
            set => SetValue(UserAgentProperty, value);
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var progress = Bass.StreamGetFilePosition(_chan, FileStreamPosition.Buffer)
                * 100 / Bass.StreamGetFilePosition(_chan, FileStreamPosition.End);
            if (progress > 75 || Bass.StreamGetFilePosition(_chan, FileStreamPosition.Connected) == 0)
            {
                _timer.Stop();
                Status = PlayStatus.Playing;
                var icy = Bass.ChannelGetTags(_chan, TagType.ICY);
                if (icy == IntPtr.Zero)
                    icy = Bass.ChannelGetTags(_chan, TagType.HTTP);
                if (icy != IntPtr.Zero)
                {
                    foreach (var tag in ManagedBass.Extensions.ExtractMultiStringAnsi(icy))
                    {
                        var icymeta = string.Empty;
                        if (tag.StartsWith("icy-name:"))
                            icymeta += $"ICY Name: {tag[9..]}";
                        if (tag.StartsWith("icy-url:"))
                            icymeta += $"ICY Url: {tag[8..]}";
                        IcyMeta = icymeta;
                    }
                }
                DoMeta();
                Bass.ChannelSetSync(_chan, SyncFlags.MetadataReceived, 0, MetaSync);
                Bass.ChannelSetSync(_chan, SyncFlags.OggChange, 0, MetaSync);
                Bass.ChannelSetSync(_chan, SyncFlags.End, 0, EndSync);
                Bass.ChannelPlay(_chan);
            }
        }
        private void StatusProc(IntPtr buffer, int length, IntPtr user)
        {
            if (buffer != IntPtr.Zero && length == 0 && user.ToInt32() == _req)
            {
                var status = Marshal.PtrToStringAnsi(buffer);
                Status = status switch
                {
                    "HTTP/1.1 200 OK" => PlayStatus.Buffered,
                    _ => PlayStatus.Error
                };
            }
        }
        private void MetaSync(int Handle, int Channel, int Data, IntPtr User) => DoMeta();
        void EndSync(int Handle, int Channel, int Data, IntPtr User) => Status = PlayStatus.Stop;
        private void DoMeta()
        {
            var meta = Bass.ChannelGetTags(_chan, TagType.META);
            if (meta != IntPtr.Zero)
            {
                var data = Marshal.PtrToStringAnsi(meta);
                var i = data.IndexOf("StreamTitle='");
                if (i == -1)
                    return;
                var j = data.IndexOf("';", i);
                if (j != -1)
                    TitleAndArtist = $"Title: {data.Substring(i, j - i + 1)}";
            }
            else
            {
                meta = Bass.ChannelGetTags(_chan, TagType.OGG);
                if (meta == IntPtr.Zero)
                    return;
                foreach (var tag in ManagedBass.Extensions.ExtractMultiStringUtf8(meta))
                {
                    string artist = null, title = null;
                    if (tag.StartsWith("artist="))
                        artist = $"Artist: {tag[7..]}";
                    if (tag.StartsWith("title="))
                        title = $"Title: {tag[6..]}";
                    if (title != null)
                        TitleAndArtist = artist != null ? $"{title} - {artist}" : title;
                }
            }
        }
    }
}
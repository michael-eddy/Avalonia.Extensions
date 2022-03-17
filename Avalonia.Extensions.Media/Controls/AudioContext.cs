using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Extensions.Styles;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Styling;
using Avalonia.Threading;
using OpenTK.Audio.OpenAL;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Avalonia.Extensions.Media
{
    public class AudioContext : Button, IStyling
    {
        private int state;
        private int buffer;
        private int source;
        private readonly object stepLock = new object();
        private HttpClient Client { get; }
        Type IStyleable.StyleKey => typeof(Button);
        public bool Playing => state == (int)ALSourceState.Playing;
        public AudioContext()
        {
            Client = Core.Instance.GetClient();
        }
        /// <summary>
        /// Defines the <see cref="Failed"/> property.
        /// </summary>
        public static readonly RoutedEvent<MediaFailedEventArgs> FailedEvent =
            RoutedEvent.Register<AudioContext, MediaFailedEventArgs>(nameof(MediaFailed), RoutingStrategies.Bubble);
        /// <summary>
        /// Raised when the image load failed.
        /// </summary>
        public event EventHandler<MediaFailedEventArgs> MediaFailed
        {
            add { AddHandler(FailedEvent, value); }
            remove { RemoveHandler(FailedEvent, value); }
        }
        /// <summary>
        /// Defines the <see cref="End"/> property.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> EndEvent =
            RoutedEvent.Register<AudioContext, RoutedEventArgs>(nameof(MediaEnd), RoutingStrategies.Bubble);
        /// <summary>
        /// Raised when the image load failed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> MediaEnd
        {
            add { AddHandler(EndEvent, value); }
            remove { RemoveHandler(EndEvent, value); }
        }
        /// <summary>
        /// Defines the <see cref="End"/> property.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> PlayEvent =
            RoutedEvent.Register<AudioContext, RoutedEventArgs>(nameof(MediaPlaying), RoutingStrategies.Bubble);
        /// <summary>
        /// Raised when the image load failed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> MediaPlaying
        {
            add { AddHandler(PlayEvent, value); }
            remove { RemoveHandler(PlayEvent, value); }
        }
        /// <summary>
        /// Defines the <see cref="Source"/> property.
        /// </summary>
        public static readonly StyledProperty<Uri> SourceProperty =
            AvaloniaProperty.Register<AudioContext, Uri>(nameof(Source));
        public Uri Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        protected override void OnClick()
        {
            base.OnClick();
            lock (stepLock)
            {
                if (Playing)
                {
                    try
                    {
                        AL.SourceStop(source);
                        OnEnd();
                        AL.DeleteSource(source);
                        AL.DeleteBuffer(buffer);
                    }
                    catch (Exception ex)
                    {
                        OnError(ex.Message);
                    }
                }
                else
                    PlayVoice();
            }
        }
        protected virtual void PlayVoice()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    buffer = AL.GenBuffer();
                    source = AL.GenSource();
                    byte[] sound_data = LoadWave(out int channels, out int bits_per_sample, out int sample_rate);
                    if (sound_data != null && sound_data.Length > 0)
                    {
                        var format = channels switch
                        {
                            1 => bits_per_sample == 8 ? ALFormat.Mono8 : ALFormat.Mono16,
                            2 => bits_per_sample == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16,
                            _ => throw new NotSupportedException("The specified sound format is not supported."),
                        };
                        AL.BufferData(buffer, format, sound_data, sound_data.Length, sample_rate);
                        AL.Source(source, ALSourcei.Buffer, buffer);
                        AL.SourcePlay(source);
                        OnPlay();
                        Trace.Write("Playing");
                        do
                        {
                            Thread.Sleep(30);
                            Trace.Write(".");
                            AL.GetSource(source, ALGetSourcei.SourceState, out state);
                        }
                        while ((ALSourceState)state == ALSourceState.Playing);
                        Trace.WriteLine("");
                        AL.SourceStop(source);
                        OnEnd();
                        AL.DeleteSource(source);
                        AL.DeleteBuffer(buffer);
                    }
                }
                catch (Exception ex)
                {
                    OnError(ex.Message);
                }
            }, DispatcherPriority.ApplicationIdle);
        }
        private byte[] LoadWave(out int channels, out int bits, out int rate)
        {
            channels = bits = rate = 0;
            Stream stream = default;
            switch (Source.Scheme)
            {
                case "http":
                case "https":
                    try
                    {
                        var url = Source.AbsoluteUri;
                        if (!string.IsNullOrEmpty(url))
                        {
                            HttpResponseMessage hr = Client.GetAsync(url).GetAwaiter().GetResult();
                            hr.EnsureSuccessStatusCode();
                            stream = hr.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                        }
                        else
                            OnError("URL cannot be NULL or EMPTY.");
                    }
                    catch (Exception ex)
                    {
                        OnError(ex.Message);
                    }
                    break;
                case "avares":
                    try
                    {
                        stream = Core.Instance.AssetLoader.Open(Source);
                    }
                    catch (Exception ex)
                    {
                        OnError(ex.Message);
                    }
                    break;
                default:
                    OnError("unsupport URI scheme.only support HTTP/HTTPS or avares://");
                    break;
            }
            if (stream != null)
            {
                using BinaryReader reader = new BinaryReader(stream);
                string signature = new string(reader.ReadChars(4));
                if (signature.Equals("RIFF", StringComparison.CurrentCultureIgnoreCase))
                {
                    OnError("Specified stream is not a wave file.");
                    return default;
                }
                int riff_chunck_size = reader.ReadInt32();
                string format = new string(reader.ReadChars(4));
                if (format.Equals("WAVE", StringComparison.CurrentCultureIgnoreCase))
                {
                    OnError("Specified stream is not a wave file.");
                    return default;
                }
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature.Equals("fmt ", StringComparison.CurrentCultureIgnoreCase))
                {
                    OnError("Specified wave file is not supported.");
                    return default;
                }
                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();
                string data_signature = string.Empty;
                while (true)
                {
                    data_signature = new string(reader.ReadChars(4));
                    if (data_signature == "data") break;
                    var size = reader.ReadInt32();
                    reader.ReadBytes(size);
                }
                int data_chunk_size = reader.ReadInt32();
                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;
                return reader.ReadBytes(data_chunk_size);
            }
            return default;
        }
        private void OnPlay()
        {
            var @event = new RoutedEventArgs(PlayEvent);
            RaiseEvent(@event);
            if (!@event.Handled)
                @event.Handled = true;
        }
        private void OnEnd()
        {
            var @event = new RoutedEventArgs(EndEvent);
            RaiseEvent(@event);
            if (!@event.Handled)
                @event.Handled = true;
        }
        private void OnError(string msg)
        {
            Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, msg);
            var @event = new MediaFailedEventArgs(FailedEvent, msg);
            RaiseEvent(@event);
            if (!@event.Handled)
                @event.Handled = true;
        }
    }
}
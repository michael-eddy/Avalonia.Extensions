using Avalonia.Controls.Primitives;
using Avalonia.Logging;
using ManagedBass;
using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Media
{
    public unsafe class AudioElement : TemplatedControl, IVideoView
    {
        private readonly Task playTask;
        private readonly DecodecAudio audio;
        ~AudioElement()
        {
            Bass.Free();
        }
        public AudioElement()
        {
            audio = new DecodecAudio();
            playTask = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        if (audio.IsPlaying)
                        {
                            if (audio.TryReadNextFrame(out var frame))
                            {
                                var bytes = audio.FrameConvertBytes(&frame);
                                if (bytes == null) continue;
                                var handle = Bass.CreateStream(bytes, 0, 0, BassFlags.Mono | BassFlags.Default);
                                Bass.ChannelPlay(handle, false);
                                if (handle != 0)
                                    Bass.StreamFree(handle);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
                    }
                }
            });
        }
        public bool Play()
        {
            try
            {
                audio.Play();
                playTask.Wait();
                return true;
            }
            catch { }
            return false;
        }
        public bool Play(string uri)
        {
            try
            {
                if (audio.State == MediaState.None)
                {
                    audio.InitDecodecAudio(uri);
                    DisplayVideoInfo();
                }
                audio.Play();
                playTask.Wait();
                return true;
            }
            catch { }
            return false;
        }
        public bool Pause()
        {
            try
            {
                audio.Pause();
                playTask.Wait();
                return true;
            }
            catch { }
            return false;
        }
        public bool Stop()
        {
            try
            {
                audio.Stop();
                playTask.Wait();
                return true;
            }
            catch { }
            return false;
        }
        #region 音频信息
        private string codec;
        public string Codec => codec;
        private TimeSpan duration;
        public TimeSpan Duration => duration;
        private double sampleRate;
        public double SampleRate => sampleRate;
        private double frameHeight;
        public double FrameHeight => frameHeight;
        private double frameWidth;
        public double FrameWidth => frameWidth;
        private long audioBitrate;
        public long AudioBitrate => audioBitrate;
        private long audioBitsPerSample;
        public long AudioBitsPerSample => audioBitsPerSample;
        private int audioChannels;
        public int AudioChannels => audioChannels;
        private ulong audioChannelsLayout;
        public ulong AudioChannelsLayout => audioChannelsLayout;
        private double totalSeconds;
        public double TotalSeconds => totalSeconds;
        private TimeSpan position;
        public TimeSpan Position => position;
        void DisplayVideoInfo()
        {
            position = audio.Position;
            duration = audio.Duration;
            codec = audio.CodecName;
            audioBitrate = audio.Bitrate;
            audioChannels = audio.Channels;
            sampleRate = audio.SampleRate;
            audioBitsPerSample = audio.BitsPerSample;
            audioChannelsLayout = audio.ChannelLyaout;
            totalSeconds = audio.Duration.TotalSeconds;
        }
        #endregion
    }
}
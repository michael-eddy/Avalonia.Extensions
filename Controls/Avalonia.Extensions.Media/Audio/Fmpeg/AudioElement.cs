using Avalonia.Controls;
using Avalonia.Logging;
using ManagedBass;
using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Media
{
    public unsafe class AudioElement : Control, IPlayerView
    {
        private Errors error;
        private int decodeStream;
        private readonly Task playTask;
        private readonly AudioStreamDecoder audio;
        public Errors LastError => error;
        public AudioElement()
        {
            audio = new AudioStreamDecoder();
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
                                if (bytes == null) return;
                                if (Bass.StreamPutData(decodeStream, bytes, bytes.Length) == -1)
                                    error = Bass.LastError;
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
                    DisplayAudioInfo();
                    if (decodeStream != 0)
                        Bass.StreamFree(decodeStream);
                    decodeStream = Bass.CreateStream(audio.SampleRate, audio.Channels, BassFlags.Mono, StreamProcedureType.Push);
                    if (!Bass.ChannelPlay(decodeStream, true))
                        error = Bass.LastError;
                }
                audio.Play();
                playTask.Start();
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
        private long audioBitrate;
        public long AudioBitrate => audioBitrate;
        private long audioBitsPerSample;
        public long AudioBitsPerSample => audioBitsPerSample;
        private double totalSeconds;
        public double TotalSeconds => totalSeconds;
        private TimeSpan position;
        public TimeSpan Position => position;
        void DisplayAudioInfo()
        {
            position = audio.Position;
            duration = audio.Duration;
            codec = audio.CodecName;
            audioBitrate = audio.Bitrate;
            sampleRate = audio.SampleRate;
            audioBitsPerSample = audio.BitsPerSample;
            totalSeconds = audio.Duration.TotalSeconds;
        }
        #endregion
    }
}
using Avalonia.Controls;
using Avalonia.Logging;
using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Media
{
    //https://github.com/haku1gh/UltraStar.Core/blob/865ecaacc054dde96a0b64cc32bc2928c3480ab8/src/UltraStar.Core/Audio/BassAudioPlayback.cs
    public unsafe class AudioElement : Control, IPlayerView
    {
        private readonly Task playTask;
        private readonly AudioStreamDecoder audio;
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
        void DisplayVideoInfo()
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
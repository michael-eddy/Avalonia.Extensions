using Avalonia.Logging;
using FFmpeg.AutoGen;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Media
{
    public sealed unsafe class AudioStreamDecoder : IMedia
    {
        AVFormatContext* format;
        AVCodecContext* codecContext;
        AVStream* audioStream;
        AVPacket* packet;
        AVFrame* frame;
        SwrContext* convert;
        int audioStreamIndex;
        bool isNextFrame = true;
        TimeSpan lastTime;
        TimeSpan OffsetClock;
        Stopwatch clock = new Stopwatch();
        readonly object syncLock = new object();
        public event IMedia.MediaHandler MediaCompleted;
        public bool IsPlaying { get; private set; }
        public MediaState State { get; private set; }
        public TimeSpan frameDuration { get; private set; }
        public TimeSpan Duration { get; private set; }
        public TimeSpan Position => OffsetClock + clock.Elapsed;
        public string CodecName { get; private set; }
        public string CodecId { get; private set; }
        public long Bitrate { get; private set; }
        public long SampleRate { get; private set; }
        public long BitsPerSample { get; private set; }
        public AVSampleFormat SampleFormat { get; private set; }
        public void InitDecodecAudio(string path)
        {
            int error = 0;
            format = ffmpeg.avformat_alloc_context();
            var tempFormat = format;
            error = ffmpeg.avformat_open_input(&tempFormat, path, null, null);
            if (error < 0)
            {
                Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, "Failed to open media file");
                return;
            }
            ffmpeg.avformat_find_stream_info(format, null);
            AVCodec* codec;
            audioStreamIndex = ffmpeg.av_find_best_stream(format, AVMediaType.AVMEDIA_TYPE_AUDIO, -1, -1, &codec, 0);
            if (audioStreamIndex < 0)
            {
                Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, "No audio stream found");
                return;
            }
            audioStream = format->streams[audioStreamIndex];
            codecContext = ffmpeg.avcodec_alloc_context3(codec);
            error = ffmpeg.avcodec_parameters_to_context(codecContext, audioStream->codecpar);
            if (error < 0)
                Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, "Setting decoder parameters failed");
            error = ffmpeg.avcodec_open2(codecContext, codec, null);
            Duration = TimeSpan.FromMilliseconds(format->duration / 1000);
            CodecId = codec->id.ToString();
            CodecName = ffmpeg.avcodec_get_name(codec->id);
            Bitrate = codecContext->bit_rate;
            var channelLayout = codecContext->ch_layout;
            SampleRate = codecContext->sample_rate;
            SampleFormat = codecContext->sample_fmt;
            BitsPerSample = ffmpeg.av_samples_get_buffer_size(null, 2, codecContext->frame_size, AVSampleFormat.AV_SAMPLE_FMT_S16, 1);
            audioBuffer = Marshal.AllocHGlobal((int)BitsPerSample);
            bufferPtr = (byte*)audioBuffer;
            InitConvert(channelLayout, AVSampleFormat.AV_SAMPLE_FMT_S16, (int)SampleRate, channelLayout, SampleFormat, (int)SampleRate);
            packet = ffmpeg.av_packet_alloc();
            frame = ffmpeg.av_frame_alloc();
            State = MediaState.Read;
        }
        IntPtr audioBuffer;
        byte* bufferPtr;
        bool InitConvert(AVChannelLayout occ, AVSampleFormat osf, int osr, AVChannelLayout icc, AVSampleFormat isf, int isr)
        {
            convert = ffmpeg.swr_alloc();
            var tempConvert = convert;
            ffmpeg.swr_alloc_set_opts2(&tempConvert, &occ, osf, osr, &icc, isf, isr, 0, null);
            if (convert == null)
                return false;
            ffmpeg.swr_init(convert);
            return true;
        }
        public bool TryReadNextFrame(out AVFrame outFrame)
        {
            if (lastTime == TimeSpan.Zero)
            {
                lastTime = Position;
                isNextFrame = true;
            }
            else
            {
                if (Position - lastTime >= frameDuration)
                {
                    lastTime = Position;
                    isNextFrame = true;
                }
                else
                {
                    outFrame = *frame;
                    return false;
                }
            }
            if (isNextFrame)
            {
                lock (syncLock)
                {
                    int result = -1;
                    ffmpeg.av_frame_unref(frame);
                    while (true)
                    {
                        ffmpeg.av_packet_unref(packet);
                        result = ffmpeg.av_read_frame(format, packet);
                        if (result == ffmpeg.AVERROR_EOF || result < 0)
                        {
                            outFrame = *frame;
                            StopPlay();
                            return false;
                        }
                        if (packet->stream_index != audioStreamIndex)
                            continue;
                        ffmpeg.avcodec_send_packet(codecContext, packet);
                        result = ffmpeg.avcodec_receive_frame(codecContext, frame);
                        if (result < 0) continue;
                        frameDuration = TimeSpan.FromTicks((long)Math.Round(TimeSpan.TicksPerMillisecond * 1000d * frame->nb_samples / frame->sample_rate, 0));
                        outFrame = *frame;
                        return true;
                    }
                }
            }
            else
            {
                outFrame = *frame;
                return false;
            }
        }
        void StopPlay()
        {
            lock (syncLock)
            {
                if (State == MediaState.None) return;
                IsPlaying = false;
                OffsetClock = TimeSpan.FromSeconds(0);
                clock.Reset();
                clock.Stop();
                var tempFormat = format;
                ffmpeg.avformat_free_context(tempFormat);
                format = null;
                var tempCodecContext = codecContext;
                ffmpeg.avcodec_free_context(&tempCodecContext);
                var tempPacket = packet;
                ffmpeg.av_packet_free(&tempPacket);
                var tempFrame = frame;
                ffmpeg.av_frame_free(&tempFrame);
                var tempConvert = convert;
                ffmpeg.swr_free(&tempConvert);
                Marshal.FreeHGlobal(audioBuffer);
                bufferPtr = null;
                audioStream = null;
                audioStreamIndex = -1;
                Duration = TimeSpan.FromMilliseconds(0);
                CodecName = string.Empty;
                CodecId = string.Empty;
                Bitrate = 0;
                SampleRate = 0;
                BitsPerSample = 0;
                State = MediaState.None;
                lastTime = TimeSpan.Zero;
                MediaCompleted?.Invoke(Duration);
            }
        }
        public void SeekProgress(int seekTime)
        {
            if (format == null || audioStreamIndex == -1) return;
            lock (syncLock)
            {
                IsPlaying = false;
                clock.Stop();
                var timestamp = seekTime / ffmpeg.av_q2d(audioStream->time_base);
                ffmpeg.av_seek_frame(format, audioStreamIndex, (long)timestamp, ffmpeg.AVSEEK_FLAG_BACKWARD | ffmpeg.AVSEEK_FLAG_FRAME);
                ffmpeg.av_frame_unref(frame);
                ffmpeg.av_packet_unref(packet);
                int error = 0;
                while (packet->pts < timestamp)
                {
                    do
                    {
                        do
                        {
                            ffmpeg.av_packet_unref(packet);
                            error = ffmpeg.av_read_frame(format, packet);
                            if (error == ffmpeg.AVERROR_EOF) return;
                        } while (packet->stream_index != audioStreamIndex);
                        ffmpeg.avcodec_send_packet(codecContext, packet);
                        error = ffmpeg.avcodec_receive_frame(codecContext, frame);
                    } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));
                }
                OffsetClock = TimeSpan.FromSeconds(seekTime);
                clock.Restart();
                IsPlaying = true;
                lastTime = TimeSpan.Zero;
            }
        }
        public byte[] FrameConvertBytes(AVFrame* sourceFrame)
        {
            var tempBufferPtr = bufferPtr;
            var outputSamplesPerChannel = ffmpeg.swr_convert(convert, &tempBufferPtr, frame->nb_samples, sourceFrame->extended_data, sourceFrame->nb_samples);
            var outPutBufferLength = ffmpeg.av_samples_get_buffer_size(null, 2, outputSamplesPerChannel, AVSampleFormat.AV_SAMPLE_FMT_S16, 1);
            if (outputSamplesPerChannel < 0)
                return null;
            byte[] bytes = new byte[outPutBufferLength];
            Marshal.Copy(audioBuffer, bytes, 0, bytes.Length);
            return bytes;
        }
        public void Play()
        {
            if (State == MediaState.Play)
                return;
            clock.Start();
            IsPlaying = true;
            State = MediaState.Play;
        }
        public void Pause()
        {
            if (State != MediaState.Play) return;
            IsPlaying = false;
            OffsetClock = clock.Elapsed;
            clock.Stop();
            clock.Reset();
            State = MediaState.Pause;
        }
        public void Stop()
        {
            if (State == MediaState.None) return;
            StopPlay();
        }
    }
}
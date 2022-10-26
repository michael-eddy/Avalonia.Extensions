using Avalonia.Logging;
using FFmpeg.AutoGen;
using System;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.Media.Audio.Fmpeg
{
    public unsafe sealed class AudioStreamDecoder
    {
        #region
        private byte* bufferPtr;
        private AVFrame* frame;
        private AVPacket* packet;
        private SwrContext* convert;
        private int audioStreamIndex;
        private AVStream* audioStream;
        private AVFormatContext* format;
        private AVCodecContext* codecContext;
        public MediaState State { get; private set; }
        public TimeSpan Duration { get; private set; }
        public string CodecName { get; private set; }
        public string CodecId { get; private set; }
        public long Bitrate { get; private set; }
        public int Channels { get; private set; }
        public int SampleRate { get; private set; }
        public int BitsPerSample { get; private set; }
        public IntPtr audioBuffer { get; private set; }
        public ulong ChannelLyaout { get; private set; }
        public AVSampleFormat SampleFormat { get; private set; }
        #endregion
        bool InitConvert(int occ, AVSampleFormat osf, int osr, int icc, AVSampleFormat isf, int isr)
        {
            convert = ffmpeg.swr_alloc();
            convert = ffmpeg.swr_alloc_set_opts(convert, occ, osf, osr, icc, isf, isr, 0, null);
            if (convert == null)
                return false;
            ffmpeg.swr_init(convert);
            return true;
        }
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
            Channels = codecContext->channels;
            ChannelLyaout = codecContext->channel_layout;
            SampleRate = codecContext->sample_rate;
            SampleFormat = codecContext->sample_fmt;
            BitsPerSample = ffmpeg.av_samples_get_buffer_size(null, 2, codecContext->frame_size, AVSampleFormat.AV_SAMPLE_FMT_S16, 1);
            audioBuffer = Marshal.AllocHGlobal(BitsPerSample);
            bufferPtr = (byte*)audioBuffer;
            InitConvert((int)ChannelLyaout, AVSampleFormat.AV_SAMPLE_FMT_S16, SampleRate, (int)ChannelLyaout, SampleFormat, SampleRate);
            packet = ffmpeg.av_packet_alloc();
            frame = ffmpeg.av_frame_alloc();
            State = MediaState.Read;
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
    }
}
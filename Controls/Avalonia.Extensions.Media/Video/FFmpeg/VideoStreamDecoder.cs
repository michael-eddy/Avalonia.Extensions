using Avalonia.Logging;
using FFmpeg.AutoGen;
using PCLUntils.Objects;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static Avalonia.Extensions.Media.IMedia;

namespace Avalonia.Extensions.Media
{
    public sealed unsafe class VideoStreamDecoder : IMedia
    {
        AVFormatContext* format;
        AVCodecContext* codecContext;
        AVPacket* packet;
        AVFrame* frame;
        SwsContext* convert;
        AVStream* videoStream;
        int videoStreamIndex;
        TimeSpan OffsetClock;
        IntPtr FrameBufferPtr;
        byte_ptrArray4 TargetData;
        int_array4 TargetLinesize;
        readonly Stopwatch clock = new Stopwatch();
        readonly object SyncLock = new object();
        TimeSpan lastTime;
        bool isNextFrame = true;
        public event MediaHandler MediaCompleted;
        public event MediaHandler MediaPlay;
        public event MediaHandler MediaPause;
        #region
        public TimeSpan Duration { get; private set; }
        public string CodecName { get; private set; }
        public string CodecId { get; private set; }
        public int Bitrate { get; private set; }
        public double FrameRate { get; private set; }
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public bool IsPlaying { get; private set; }
        public MediaState State { get; private set; }
        public TimeSpan Position => clock.Elapsed + OffsetClock;
        public TimeSpan frameDuration { get; private set; }
        #endregion
        public VideoStreamDecoder()
        {
            Init(null);
        }
        public void Init(string ffmpegBinaryPath)
        {
            if (ffmpegBinaryPath.IsEmpty())
                ffmpegBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libffmpeg", Environment.Is64BitProcess ? "x64" : "x86");
            if (Directory.Exists(ffmpegBinaryPath))
            {
                Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, $"FFmpeg binaries found in: {ffmpegBinaryPath}");
                ffmpeg.RootPath = ffmpegBinaryPath;
            }
            else
                throw new FileNotFoundException($"cannot found FFmpeg binaries from path:\"{ffmpegBinaryPath}\"");
            ffmpeg.avdevice_register_all();
            ffmpeg.avformat_network_init();
            ffmpeg.av_log_set_level(ffmpeg.AV_LOG_VERBOSE);
            av_log_set_callback_callback logCallback = (p0, level, format, vl) =>
            {
                if (level > ffmpeg.av_log_get_level()) return;
                var lineSize = 1024;
                var lineBuffer = stackalloc byte[lineSize];
                var printPrefix = 1;
                ffmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
                var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);
                Console.Write(line);
            };
            ffmpeg.av_log_set_callback(logCallback);
        }
        public void InitDecodecVideo(string path)
        {
            int error = 0;
            format = ffmpeg.avformat_alloc_context();
            if (format == null)
            {
                Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, "Failed to create media format (container)");
                return;
            }
            var tempFormat = format;
            error = ffmpeg.avformat_open_input(&tempFormat, path, null, null);
            if (error < 0)
            {
                Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, "Failed to open video");
                return;
            }
            ffmpeg.avformat_find_stream_info(format, null);
            AVCodec* codec = null;
            videoStreamIndex = ffmpeg.av_find_best_stream(format, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0);
            if (videoStreamIndex < 0)
            {
                Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, "No video stream found");
                return;
            }
            videoStream = format->streams[videoStreamIndex];
            codecContext = ffmpeg.avcodec_alloc_context3(codec);
            error = ffmpeg.avcodec_parameters_to_context(codecContext, videoStream->codecpar);
            if (error < 0)
            {
                Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, "Failed to set decoder parameters");
                return;
            }
            error = ffmpeg.avcodec_open2(codecContext, codec, null);
            if (error < 0)
            {
                Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, "Failed to open decoder");
                return;
            }
            Duration = TimeSpan.FromMilliseconds(format->duration / 1000);
            CodecId = videoStream->codecpar->codec_id.ToString();
            CodecName = ffmpeg.avcodec_get_name(videoStream->codecpar->codec_id);
            Bitrate = (int)videoStream->codecpar->bit_rate;
            FrameRate = ffmpeg.av_q2d(videoStream->r_frame_rate);
            FrameWidth = videoStream->codecpar->width;
            FrameHeight = videoStream->codecpar->height;
            frameDuration = TimeSpan.FromMilliseconds(1000 / FrameRate);
            var result = InitConvert(FrameWidth, FrameHeight, codecContext->pix_fmt, FrameWidth, FrameHeight, AVPixelFormat.AV_PIX_FMT_BGR0);
            if (result)
            {
                packet = ffmpeg.av_packet_alloc();
                frame = ffmpeg.av_frame_alloc();
            }
        }
        bool InitConvert(int sourceWidth, int sourceHeight, AVPixelFormat sourceFormat, int targetWidth, int targetHeight, AVPixelFormat targetFormat)
        {
            convert = ffmpeg.sws_getContext(sourceWidth, sourceHeight, sourceFormat, targetWidth, targetHeight, targetFormat, ffmpeg.SWS_FAST_BILINEAR, null, null, null);
            if (convert == null)
            {
                Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, "Failed to create converter");
                return false;
            }
            var bufferSize = ffmpeg.av_image_get_buffer_size(targetFormat, targetWidth, targetHeight, 1);
            FrameBufferPtr = Marshal.AllocHGlobal(bufferSize);
            TargetData = new byte_ptrArray4();
            TargetLinesize = new int_array4();
            ffmpeg.av_image_fill_arrays(ref TargetData, ref TargetLinesize, (byte*)FrameBufferPtr, targetFormat, targetWidth, targetHeight, 1);
            return true;
        }
        public AVFrame FrameConvert(AVFrame* sourceFrame)
        {
            ffmpeg.sws_scale(convert, sourceFrame->data, sourceFrame->linesize, 0, sourceFrame->height, TargetData, TargetLinesize);
            var data = new byte_ptrArray8();
            data.UpdateFrom(TargetData);
            var linesize = new int_array8();
            linesize.UpdateFrom(TargetLinesize);
            return new AVFrame
            {
                data = data,
                linesize = linesize,
                width = FrameWidth,
                height = FrameHeight
            };
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
                lock (SyncLock)
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
                        if (packet->stream_index != videoStreamIndex)
                            continue;
                        ffmpeg.avcodec_send_packet(codecContext, packet);
                        result = ffmpeg.avcodec_receive_frame(codecContext, frame);
                        if (result < 0) continue;
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
            lock (SyncLock)
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
                ffmpeg.sws_freeContext(convert);
                videoStream = null;
                videoStreamIndex = -1;
                Duration = TimeSpan.FromMilliseconds(0);
                CodecName = string.Empty;
                CodecId = string.Empty;
                Bitrate = 0;
                FrameRate = 0;
                FrameWidth = 0;
                FrameHeight = 0;
                State = MediaState.None;
                Marshal.FreeHGlobal(FrameBufferPtr);
                lastTime = TimeSpan.Zero;
                MediaCompleted?.Invoke(Duration);
            }
        }
        public void SeekProgress(int seekTime)
        {
            if (format == null || videoStream == null)
                return;
            lock (SyncLock)
            {
                IsPlaying = false;
                clock.Stop();
                var timestamp = seekTime / ffmpeg.av_q2d(videoStream->time_base);
                ffmpeg.av_seek_frame(format, videoStreamIndex, (long)timestamp, ffmpeg.AVSEEK_FLAG_BACKWARD | ffmpeg.AVSEEK_FLAG_FRAME);
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
                            if (error == ffmpeg.AVERROR_EOF)
                                return;
                        } while (packet->stream_index != videoStreamIndex);
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
        public void Play()
        {
            if (State == MediaState.Play) return;
            clock.Start();
            IsPlaying = true;
            State = MediaState.Play;
            MediaPlay?.Invoke(Duration);
        }
        public void Pause()
        {
            if (State != MediaState.Play) return;
            IsPlaying = false;
            OffsetClock = clock.Elapsed;
            clock.Stop();
            clock.Reset();
            State = MediaState.Pause;
            MediaPause?.Invoke(Duration);
        }
        public void Stop()
        {
            if (State == MediaState.None) return;
            StopPlay();
        }
    }
}
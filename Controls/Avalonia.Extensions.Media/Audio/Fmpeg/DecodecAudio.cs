using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Avalonia.Extensions.Media
{
    public unsafe class DecodecAudio : IMedia
    {
        //媒体格式容器
        AVFormatContext* format;
        //解码上下文
        AVCodecContext* codecContext;
        AVStream* audioStream;
        //媒体数据包
        AVPacket* packet;
        AVFrame* frame;
        SwrContext* convert;
        int audioStreamIndex;
        bool isNextFrame = true;
        //播放上一帧的时间
        TimeSpan lastTime;
        TimeSpan OffsetClock;
        object SyncLock = new object();
        Stopwatch clock = new Stopwatch();
        bool isNexFrame = true;
        public event IMedia.MediaHandler MediaCompleted;
        //是否是正在播放中
        public bool IsPlaying { get; protected set; }
        /// <summary>
        /// 媒体状态
        /// </summary>
        public MediaState State { get; protected set; }
        /// <summary>
        /// 帧播放时长
        /// </summary>
        public TimeSpan frameDuration { get; protected set; }
        /// <summary>
        /// 媒体时长
        /// </summary>
        public TimeSpan Duration { get; protected set; }
        /// <summary>
        /// 播放位置
        /// </summary>
        public TimeSpan Position { get => OffsetClock + clock.Elapsed; }
        /// <summary>
        /// 解码器名字
        /// </summary>
        public string CodecName { get; protected set; }
        /// <summary>
        /// 解码器Id
        /// </summary>
        public string CodecId { get; protected set; }
        /// <summary>
        /// 比特率
        /// </summary>
        public long Bitrate { get; protected set; }
        //通道数
        public int Channels { get; protected set; }
        //采样率
        public long SampleRate { get; protected set; }
        //采样次数
        public long BitsPerSample { get; protected set; }
        //通道布局
        public ulong ChannelLyaout { get; protected set; }
        /// <summary>
        /// 采样格式
        /// </summary>
        public AVSampleFormat SampleFormat { get; protected set; }
        public void InitDecodecAudio(string path)
        {
            int error = 0;
            //创建一个 媒体格式上下文
            format = ffmpeg.avformat_alloc_context();
            var tempFormat = format;
            //打开媒体文件
            error = ffmpeg.avformat_open_input(&tempFormat, path, null, null);
            if (error < 0)
            {
                Debug.WriteLine("打开媒体文件失败");
                return;
            }
            //嗅探媒体信息
            ffmpeg.avformat_find_stream_info(format, null);
            AVCodec* codec;
            //获取音频流索引
            audioStreamIndex = ffmpeg.av_find_best_stream(format, AVMediaType.AVMEDIA_TYPE_AUDIO, -1, -1, &codec, 0);
            if (audioStreamIndex < 0)
            {
                Debug.WriteLine("没有找到音频流");
                return;
            }
            //获取音频流
            audioStream = format->streams[audioStreamIndex];
            //创建解码上下文
            codecContext = ffmpeg.avcodec_alloc_context3(codec);
            //将音频流里面的解码器参数设置到 解码器上下文中
            error = ffmpeg.avcodec_parameters_to_context(codecContext, audioStream->codecpar);
            if (error < 0)
            {
                Debug.WriteLine("设置解码器参数失败");
            }
            error = ffmpeg.avcodec_open2(codecContext, codec, null);
            //媒体时长
            Duration = TimeSpan.FromMilliseconds(format->duration / 1000);
            //编解码id
            CodecId = codec->id.ToString();
            //解码器名字
            CodecName = ffmpeg.avcodec_get_name(codec->id);
            //比特率
            Bitrate = codecContext->bit_rate;
            //音频通道数
            Channels = codecContext->channels;
            //通道布局类型
            ChannelLyaout = codecContext->channel_layout;
            //音频采样率
            SampleRate = codecContext->sample_rate;
            //音频采样格式
            SampleFormat = codecContext->sample_fmt;
            //采样次数  //获取给定音频参数所需的缓冲区大小。
            BitsPerSample = ffmpeg.av_samples_get_buffer_size(null, 2, codecContext->frame_size, AVSampleFormat.AV_SAMPLE_FMT_S16, 1);
            //创建一个指针
            audioBuffer = Marshal.AllocHGlobal((int)BitsPerSample);
            bufferPtr = (byte*)audioBuffer;
            //初始化音频重采样转换器
            InitConvert((int)ChannelLyaout, AVSampleFormat.AV_SAMPLE_FMT_S16, (int)SampleRate, (int)ChannelLyaout, SampleFormat, (int)SampleRate);
            //创建一个包和帧指针
            packet = ffmpeg.av_packet_alloc();
            frame = ffmpeg.av_frame_alloc();
            State = MediaState.Read;
        }
        //缓冲区指针
        IntPtr audioBuffer;
        //缓冲区句柄
        byte* bufferPtr;
        /// <summary>
        /// 初始化重采样转换器
        /// </summary>
        /// <param name="occ">输出的通道类型</param>
        /// <param name="osf">输出的采样格式</param>
        /// <param name="osr">输出的采样率</param>
        /// <param name="icc">输入的通道类型</param>
        /// <param name="isf">输入的采样格式</param>
        /// <param name="isr">输入的采样率</param>
        /// <returns></returns>
        bool InitConvert(int occ, AVSampleFormat osf, int osr, int icc, AVSampleFormat isf, int isr)
        {
            //创建一个重采样转换器
            convert = ffmpeg.swr_alloc();
            //设置重采样转换器参数
            convert = ffmpeg.swr_alloc_set_opts(convert, occ, osf, osr, icc, isf, isr, 0, null);
            if (convert == null)
                return false;
            //初始化重采样转换器
            ffmpeg.swr_init(convert);
            return true;
        }
        /// <summary>
        /// 尝试读取下一帧
        /// </summary>
        /// <param name="outFrame"></param>
        /// <returns></returns>
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
                    //清理上一帧的数据
                    ffmpeg.av_frame_unref(frame);
                    while (true)
                    {
                        //清理上一帧的数据包
                        ffmpeg.av_packet_unref(packet);
                        //读取下一帧，返回一个int 查看读取数据包的状态
                        result = ffmpeg.av_read_frame(format, packet);
                        //读取了最后一帧了，没有数据了，退出读取帧
                        if (result == ffmpeg.AVERROR_EOF || result < 0)
                        {
                            outFrame = *frame;
                            StopPlay();
                            return false;
                        }
                        //判断读取的帧数据是否是视频数据，不是则继续读取
                        if (packet->stream_index != audioStreamIndex)
                            continue;
                        //将包数据发送给解码器解码
                        ffmpeg.avcodec_send_packet(codecContext, packet);
                        //从解码器中接收解码后的帧
                        result = ffmpeg.avcodec_receive_frame(codecContext, frame);
                        if (result < 0)
                            continue;
                        //计算当前帧播放的时长
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
                ffmpeg.swr_free(&tempConvert);

                Marshal.FreeHGlobal(audioBuffer);
                bufferPtr = null;

                audioStream = null;
                audioStreamIndex = -1;
                //视频时长
                Duration = TimeSpan.FromMilliseconds(0);
                //编解码器名字
                CodecName = String.Empty;
                CodecId = String.Empty;
                //比特率
                Bitrate = 0;
                //帧率

                Channels = 0;
                ChannelLyaout = 0;
                SampleRate = 0;
                BitsPerSample = 0;
                State = MediaState.None;

                lastTime = TimeSpan.Zero;
                MediaCompleted?.Invoke(Duration);
            }
        }
        /// <summary>
        /// 更改进度
        /// </summary>
        /// <param name="seekTime">更改到的位置（秒）</param>
        public void SeekProgress(int seekTime)
        {
            if (format == null || audioStreamIndex == null)
                return;
            lock (SyncLock)
            {
                IsPlaying = false;//将视频暂停播放
                clock.Stop();
                //将秒数转换成视频的时间戳
                var timestamp = seekTime / ffmpeg.av_q2d(audioStream->time_base);
                //将媒体容器里面的指定流（视频）的时间戳设置到指定的位置，并指定跳转的方法；
                ffmpeg.av_seek_frame(format, audioStreamIndex, (long)timestamp, ffmpeg.AVSEEK_FLAG_BACKWARD | ffmpeg.AVSEEK_FLAG_FRAME);
                ffmpeg.av_frame_unref(frame);//清除上一帧的数据
                ffmpeg.av_packet_unref(packet); //清除上一帧的数据包
                int error = 0;
                //循环获取帧数据，判断获取的帧时间戳已经大于给定的时间戳则说明已经到达了指定的位置则退出循环
                while (packet->pts < timestamp)
                {
                    do
                    {
                        do
                        {
                            ffmpeg.av_packet_unref(packet);//清除上一帧数据包
                            error = ffmpeg.av_read_frame(format, packet);//读取数据
                            if (error == ffmpeg.AVERROR_EOF)//是否是到达了视频的结束位置
                                return;
                        } while (packet->stream_index != audioStreamIndex);//判断当前获取的数据是否是视频数据
                        ffmpeg.avcodec_send_packet(codecContext, packet);//将数据包发送给解码器解码
                        error = ffmpeg.avcodec_receive_frame(codecContext, frame);//从解码器获取解码后的帧数据
                    } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));
                }
                OffsetClock = TimeSpan.FromSeconds(seekTime);//设置时间偏移
                clock.Restart();//时钟从新开始
                IsPlaying = true;//视频开始播放
                lastTime = TimeSpan.Zero;
            }
        }
        /// <summary>
        /// 将音频帧转换成字节数组
        /// </summary>
        /// <param name="sourceFrame"></param>
        /// <returns></returns>
        public byte[] FrameConvertBytes(AVFrame* sourceFrame)
        {
            var tempBufferPtr = bufferPtr;
            //重采样音频
            var outputSamplesPerChannel = ffmpeg.swr_convert(convert, &tempBufferPtr, frame->nb_samples, sourceFrame->extended_data, sourceFrame->nb_samples);
            //获取重采样后的音频数据大小
            var outPutBufferLength = ffmpeg.av_samples_get_buffer_size(null, 2, outputSamplesPerChannel, AVSampleFormat.AV_SAMPLE_FMT_S16, 1);
            if (outputSamplesPerChannel < 0)
                return null;
            byte[] bytes = new byte[outPutBufferLength];
            //从内存中读取转换后的音频数据
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
            if (State != MediaState.Play)
                return;
            IsPlaying = false;
            OffsetClock = clock.Elapsed;
            clock.Stop();
            clock.Reset();

            State = MediaState.Pause;
        }
        public void Stop()
        {
            if (State == MediaState.None)
                return;
            StopPlay();
        }
    }
}
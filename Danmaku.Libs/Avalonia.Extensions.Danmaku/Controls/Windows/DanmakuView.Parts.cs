using Avalonia.Logging;
using PCLUntils.Plantform;
using PCLUntils;
using System;
using Avalonia.Threading;
using System.IO;
using System.Text;
using LibVLCSharp.Shared;

namespace Avalonia.Extensions.Danmaku
{
	public partial class DanmakuView
	{
		public void Load(string xml)
		{
			try
			{
				switch (PlantformUntils.Platform)
				{
					case Platforms.Windows:
						Dispatcher.UIThread.InvokeAsync(() =>
						{
							if (!string.IsNullOrEmpty(xml))
							{
								while (wtf != IntPtr.Zero)
								{
									LibLoader.WTF_LoadBilibiliXml(wtf, xml);
									LibLoader.WTF_Start(wtf);
									Resize(Bounds.Width, Bounds.Height);
									break;
								}
							}
						});
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
		public void Load(FileInfo filePath)
		{
			try
			{
				switch (PlantformUntils.Platform)
				{
					case Platforms.Windows:
						Dispatcher.UIThread.InvokeAsync(() =>
						{
							while (wtf != IntPtr.Zero)
							{
								if (!filePath.Exists)
									throw new FileNotFoundException($"File [{filePath.FullName}] not found!");
								LibLoader.WTF_LoadBilibiliFile(wtf, Encoding.ASCII.GetBytes(filePath.FullName));
								LibLoader.WTF_Start(wtf);
								Resize(Bounds.Width, Bounds.Height);
								break;
							}
						});
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
		public void SetDanmakuTypeVisibility(DanmakuTypeVisable visableType)
		{
			try
			{
				LibLoader.WTF_SetDanmakuTypeVisibility(wtf, (int)visableType);
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
		public void AddDanmaku(DanmakuType type, long time, string comment, int fontSize, int fontColor, long timestamp, int danmakuId)
		{
			try
			{
				LibLoader.WTF_AddDanmaku(wtf, (int)type, time, comment, fontSize, fontColor, timestamp, danmakuId);
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
		public void AddLiveDanmaku(DanmakuType type, long time, string comment, int fontSize, int fontColor, long timestamp, int danmakuId)
		{
			try
			{
				LibLoader.WTF_AddLiveDanmaku(wtf, (int)type, time, comment, fontSize, fontColor, timestamp, danmakuId);
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
		public void Play()
		{
			try
			{
				switch (PlantformUntils.Platform)
				{
					case Platforms.Windows:
						if (wtf != IntPtr.Zero)
							LibLoader.WTF_Start(wtf);
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
		public void Pause()
		{
			try
			{
				switch (PlantformUntils.Platform)
				{
					case Platforms.Windows:
						if (wtf != IntPtr.Zero)
							LibLoader.WTF_Pause(wtf);
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
		public void SeekTo(long milliseconds)
		{
			try
			{
				switch (PlantformUntils.Platform)
				{
					case Platforms.Windows:
						if (wtf != IntPtr.Zero)
							LibLoader.WTF_SeekTo(wtf, milliseconds);
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
		public long GetCurrentPosition()
		{
			try
			{
				switch (PlantformUntils.Platform)
				{
					case Platforms.Windows:
						if (wtf != IntPtr.Zero)
							return LibLoader.WTF_GetCurrentPosition(wtf);
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
			return 0;
		}
		public void SetDpi(uint dpiX, uint dpiY)
		{
			try
			{
				switch (PlantformUntils.Platform)
				{
					case Platforms.Windows:
						if (wtf != IntPtr.Zero)
							LibLoader.WTF_SetDpi(wtf, dpiX, dpiY);
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
		private void DestoryWindows()
		{
			try
			{
				if (LibLoader.WTF_IsRunning(wtf) != 0)
					LibLoader.WTF_Stop(wtf);
				LibLoader.WTF_ReleaseInstance(wtf);
				wtf = IntPtr.Zero;
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
			finally
			{
				player.MediaPlayer.Paused -= MediaPlayer_Paused;
				player.MediaPlayer.Playing -= MediaPlayer_Playing;
				player.MediaPlayer.Opening -= MediaPlayer_Opening;
				player.MediaPlayer.Stopped -= MediaPlayer_Stopped;
			}
		}
		private void MediaPlayer_Opening(object sender, EventArgs e) => Init();
		private void MediaPlayer_Playing(object sender, EventArgs e) => Play();
		private void MediaPlayer_Stopped(object sender, EventArgs e) => DestoryWindows();
		private void MediaPlayer_Paused(object sender, EventArgs e) => Pause();
		private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
		{

		}
	}
}
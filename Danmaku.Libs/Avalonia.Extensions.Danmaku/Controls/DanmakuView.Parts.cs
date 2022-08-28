using Avalonia.Logging;
using PCLUntils.Plantform;
using PCLUntils;
using System;
using Avalonia.Threading;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Reactive;
using System.Xml.Linq;

namespace Avalonia.Extensions.Danmaku
{
	public partial class DanmakuView
	{
		public void Load(string filePath)
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
								if (!File.Exists(filePath))
									throw new FileNotFoundException($"File [{filePath}] not found!");
								LibLoader.WTF_LoadBilibiliFile(wtf, Encoding.ASCII.GetBytes(filePath));
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
		}
	}
}
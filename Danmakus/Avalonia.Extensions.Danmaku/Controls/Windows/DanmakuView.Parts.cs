using Avalonia.Logging;
using PCLUntils.Plantform;
using PCLUntils;
using System;
using System.IO;
using System.Text;
using Avalonia.Threading;
using System.Net.Http;
using Avalonia.Extensions.Controls;

namespace Avalonia.Extensions.Danmaku
{
	public partial class DanmakuView
	{
		private byte[] xml;
		private void Load()
		{
			try
			{
				if (xml != null && wtf != IntPtr.Zero)
				{
					LibLoader.WTF_LoadBilibiliXml(wtf, xml);
					LibLoader.WTF_Start(wtf);
					Resize(Bounds.Width, Bounds.Height);
				}
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
		public void Load(string xml, Encoding encoding)
		{
			try
			{
				switch (PlantformUntils.Platform)
				{
					case Platforms.Windows:
						{
							if (!string.IsNullOrEmpty(xml))
							{
								this.xml = encoding.GetBytes(xml);
								if (wtf != IntPtr.Zero)
								{
									LibLoader.WTF_LoadBilibiliXml(wtf, this.xml);
									LibLoader.WTF_Start(wtf);
									Resize(Bounds.Width, Bounds.Height);
								}
							}
							break;
						}
				}
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
		public void Load(Uri uri)
		{
			if (uri == null) 
				return;
			Dispatcher.UIThread.InvokeAsync(() =>
			{
				try
				{
					switch (uri.Scheme)
					{
						case "file":
							FileInfo fileInfo = new FileInfo(uri.ToString().Replace("file:///", ""));
							if (fileInfo.Exists)
							{
								using var fs = fileInfo.OpenRead();
								ReadStream(fs);
							}
							break;
						case "avares":
							ReadStream(Core.Instance.AssetLoader.Open(uri));
							break;
						case "http":
						case "https":
							HttpResponseMessage hr = httpClient.GetAsync(uri).GetAwaiter().GetResult();
							hr.EnsureSuccessStatusCode();
							var stream = hr.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
							ReadStream(stream);
							break;
					}
					void ReadStream(Stream stream)
					{
						xml = new byte[stream.Length];
						stream.Read(xml);
						if (wtf != IntPtr.Zero)
						{
							LibLoader.WTF_LoadBilibiliXml(wtf, xml);
							LibLoader.WTF_Start(wtf);
							Resize(Bounds.Width, Bounds.Height);
						}
					}
				}
				catch (Exception ex)
				{
					Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
				}
			});
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
		public void Start()
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
		public void Stop()
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
		public void Dispose()
		{
			try
			{
				xml = null;
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
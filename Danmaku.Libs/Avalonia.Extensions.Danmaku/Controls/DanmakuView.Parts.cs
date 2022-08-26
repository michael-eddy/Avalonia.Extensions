using Avalonia.Logging;
using PCLUntils.Plantform;
using PCLUntils;
using System;
using Avalonia.Threading;
using System.IO;
using System.Text;

namespace Avalonia.Extensions.Danmaku
{
	public partial class DanmakuView
	{
		protected void Load(string filePath)
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
		protected void Play()
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
		protected void Pause()
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
		private void DestoryWindows()
		{
			try
			{
				if (LibLoader.WTF_IsRunning(wtf) != 0)
				{
					LibLoader.WTF_Stop(wtf);
					LibLoader.WTF_ReleaseInstance(wtf);
					wtf = IntPtr.Zero;
				}
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
			}
		}
	}
}
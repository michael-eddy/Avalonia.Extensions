using Avalonia.Extensions.Controls;
using Avalonia.Extensions.WebView.Models;
using Avalonia.Logging;
using Downloader;
using PCLUntils.Json;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace Avalonia.Extensions.WebView
{
    internal sealed class DownloadTask
    {
        private readonly string cefVersion;
        private readonly string projectPath;
        private static DownloadTask instance;
        private readonly HttpClient HttpClient;
        private DownloadService downloadService;
        public static DownloadTask Instance => instance ??= new DownloadTask();
        private DownloadTask()
        {
            HttpClient = Core.Instance.GetClient();
            projectPath = Path.GetDirectoryName(typeof(Application).Assembly.Location);
            string cefNetPath = Path.Combine(projectPath, "CefNet.Avalonia.dll");
            if (File.Exists(cefNetPath))
            {
                var fileVersions = FileVersionInfo.GetVersionInfo(cefNetPath).FileVersion.Split(".");
                cefVersion = $"{fileVersions[0]}.{fileVersions[1]}".Trim();
            }
        }
        public void Start()
        {
            if (!string.IsNullOrEmpty(cefVersion) && GetVersionData(out List<VersionInfo> infos))
            {
                var newVersion = infos.Where(x => x.cef_version.StartsWith(cefVersion) && x.channel.Contains("stable", StringComparison.CurrentCultureIgnoreCase))
                    .OrderByDescending(x => x.cef_version).FirstOrDefault()?.files.FirstOrDefault(x => x.type.Contains("minimal", StringComparison.CurrentCultureIgnoreCase));
                if (newVersion != null)
                {
                    var directoryInfo = new DirectoryInfo(Path.Combine(projectPath, "cef"));
                    var fileName = Path.Combine(directoryInfo.FullName, newVersion.name);
                    DownloadPackage(newVersion, directoryInfo);
                    if (File.Exists(fileName))
                    {
                        try
                        {
                            using var stream = File.OpenRead(fileName);
                            using var reader = ReaderFactory.Open(stream);
                            var options = new ExtractionOptions { Overwrite = true, ExtractFullPath = true };
                            reader.WriteAllToDirectory(directoryInfo.FullName, options);
                            string zipFolderName = Path.Combine(directoryInfo.FullName, Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileName)));
                            Untils.MoveFolder(zipFolderName, directoryInfo.FullName);
                        }
                        catch (Exception ex)
                        {
                            Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, $"ERROR! INIT CEF FAILED!{ex}");
                        }
                        finally
                        {
                            File.Delete(fileName);
                        }
                    }
                    else
                        Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(this, $"ERROR! Download File CANNOT FOUND!");
                }
            }
        }
        private void DownloadPackage(VersionFileInfo newVersion, DirectoryInfo directoryInfo)
        {
            try
            {
                var downloadConfiguration = new DownloadConfiguration();
                downloadService = new DownloadService(downloadConfiguration);
                downloadService.DownloadFileCompleted += OnDownloadFileCompleted;
                downloadService.DownloadProgressChanged += OnDownloadProgressChanged;
                downloadService.DownloadFileTaskAsync($"https://cef-builds.spotifycdn.com/{newVersion.name}", directoryInfo).Wait();
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, $"cef init error.{ex.Message}");
            }
            finally
            {
                downloadService.Clear();
                downloadService.Dispose();
            }
        }
        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) =>
            Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(sender, $"DownloadProgressChanged:{e.ProgressPercentage}");
        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e) =>
            Logger.TryGet(LogEventLevel.Information, LogArea.Control)?.Log(sender, $"DownloadFileCompleted Cancelled:{e.Cancelled};Error:{e.Error}");
        private bool GetVersionData(out List<VersionInfo> infos)
        {
            infos = default;
            try
            {
                var result = HttpClient.GetStringAsync("https://cef-builds.spotifycdn.com/index.json").Result;
                var data = result.ParseObject<VersionData>();
                if (data != null)
                {
                    switch (Runtimes.CurrentOS)
                    {
                        case OS.Linux:
                            switch (Runtimes.Architecture)
                            {
                                case Architecture.X86:
                                    infos = data.linux32.versions;
                                    break;
                                case Architecture.X64:
                                    infos = data.linux64.versions;
                                    break;
                                case Architecture.Arm:
                                    infos = data.linuxarm.versions;
                                    break;
                                case Architecture.Arm64:
                                    infos = data.linuxarm64.versions;
                                    break;
                            }
                            break;
                        case OS.OSX:
                            switch (Runtimes.Architecture)
                            {
                                case Architecture.X64:
                                    infos = data.macosx64.versions;
                                    break;
                                case Architecture.Arm64:
                                    infos = data.macosarm64.versions;
                                    break;
                            }
                            break;
                        case OS.Windows:
                            switch (Runtimes.Architecture)
                            {
                                case Architecture.X86:
                                    infos = data.windows32.versions;
                                    break;
                                case Architecture.X64:
                                    infos = data.windows64.versions;
                                    break;
                                case Architecture.Arm64:
                                    infos = data.windowsarm64.versions;
                                    break;
                            }
                            break;
                    }
                }
                return infos != null && infos.Count > 0;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Property)?.Log(this, ex.Message);
            }
            return false;
        }
    }
}
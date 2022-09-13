using System;

namespace Avalonia.Extensions.Controls
{
    internal sealed class DownloadTask
    {
        private static DownloadTask instance;
        public static DownloadTask Instance => instance ??= new DownloadTask();
        private DownloadTask()
        {

        }
        public void Start()
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.Extensions
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
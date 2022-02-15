using System;
using System.Threading;

namespace Avalonia.Extensions.Threading
{
    public sealed class ThreadJob
    {
        public Uri Uri { get; }
        public CancellationToken Token { get; }
        public ThreadJob(CancellationToken token, Uri uri)
        {
            Uri = uri;
            Token = token;
        }
    }
}
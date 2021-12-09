using System;
using System.IO;

namespace Avalonia.Extensions.Model
{
    internal sealed class Result
    {
        public Result(Stream stream)
        {
            Success = true;
            Stream = stream;
        }
        public Result(Exception exception)
        {
            Success = false;
            Message = exception.Message;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        public Stream Stream { get; set; }
    }
}
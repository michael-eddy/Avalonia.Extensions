using Avalonia.Controls;
using Avalonia.Extensions.Controls;
using Avalonia.Logging;
using System;
using System.Threading.Tasks;

namespace Avalonia.Extensions.Threading
{
    internal sealed class AnimationThread : IDisposable
    {
        private Task Thread { get; }
        private Window Window { get; }
        private NotifyOptions Options { get; set; }
        private PixelPoint StopPosition { get; set; }
        internal bool IsDisposed { get; private set; }
        private PixelPoint StartPosition { get; set; }
        private PixelPoint LeftHorizontalNext { get; set; }
        private PixelPoint BottomVerticalNext { get; set; }
        private PixelPoint RightHorizontalNext { get; set; }
        public event EventHandler DisposeEvent;
        public AnimationThread(Window window)
        {
            Window = window;
            Thread = new Task(RunJob);
        }
        public void Start(NotifyOptions options)
        {
            if (options.IsVaidate)
            {
                IsDisposed = false;
                Options = options;
                LeftHorizontalNext = new PixelPoint(Options.MovePixel, 0);
                BottomVerticalNext = new PixelPoint(0, -Options.MovePixel);
                RightHorizontalNext = new PixelPoint(-Options.MovePixel, 0);
                Thread.Start();
            }
            else
            {
                string errorMessage = "when Position is Top***,the Scroll Way(ScollOrientation) cannot be Vertical!";
                Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(this, errorMessage);
                throw new NotSupportedException(errorMessage);
            }
        }
        public void SetPath(PixelPoint startPosition, PixelPoint endPosition)
        {
            StopPosition = endPosition;
            StartPosition = startPosition;
        }
        private async void RunJob()
        {
            while (!IsDisposed)
            {
                try
                {
                    switch (Options.Position)
                    {
                        case ShowPosition.BottomLeft:
                            {
                                if (Options.ScollOrientation == ScollOrientation.Vertical)
                                    await BottomVerticalVoid();
                                else
                                    await LeftHorizontalVoid();
                                break;
                            }
                        case ShowPosition.BottomRight:
                            {
                                if (Options.ScollOrientation == ScollOrientation.Vertical)
                                    await BottomVerticalVoid();
                                else
                                    await RightHorizontalVoid();
                                break;
                            }
                        case ShowPosition.TopLeft:
                            {
                                await LeftHorizontalVoid();
                                break;
                            }
                        case ShowPosition.TopRight:
                            {
                                await RightHorizontalVoid();
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(this, "AnimationThread RunJob ERROR : " + ex.Message);
                }
            }
        }
        private async Task LeftHorizontalVoid()
        {
            StartPosition -= LeftHorizontalNext;
            Window.Position = StartPosition;
            if (StartPosition.SmallerThan(StopPosition, true))
            {
                await Task.Delay(Options.MoveDelay);
                Dispose();
            }
            else
                await Task.Delay(Options.MoveDelay);
        }
        private async Task RightHorizontalVoid()
        {
            StartPosition -= RightHorizontalNext;
            Window.Position = StartPosition;
            if (StartPosition.BiggerThan(StopPosition, true))
            {
                await Task.Delay(Options.MoveDelay);
                Dispose();
            }
            else
                await Task.Delay(Options.MoveDelay);
        }
        private async Task BottomVerticalVoid()
        {
            StartPosition -= BottomVerticalNext;
            Window.Position = StartPosition;
            if (StartPosition.SmallerThan(StopPosition, true))
            {
                await Task.Delay(Options.MoveDelay);
                Dispose();
            }
            else
                await Task.Delay(Options.MoveDelay);
        }
        public void Dispose()
        {
            try
            {
                if (!IsDisposed)
                {
                    IsDisposed = true;
                    Thread.Dispose();
                    DisposeEvent?.Invoke(this, null);
                    GC.Collect();
                }
            }
            catch { }
        }
    }
}
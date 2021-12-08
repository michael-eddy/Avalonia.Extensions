using Avalonia.Controls;
using Avalonia.Extensions.Threading;
using Avalonia.Threading;
using System;

namespace Avalonia.Extensions.Controls
{
    public sealed class NotifyWindow : Window
    {
        private NotifyOptions Options { get; }
        private AnimationThread Thread { get; }
        public NotifyWindow() : base()
        {
            Topmost = true;
            CanResize = false;
            ShowInTaskbar = false;
            Thread = new AnimationThread(this);
            Thread.DisposeEvent += Thread_DisposeEvent;
            Options = new NotifyOptions(ShowPosition.BottomRight);
            SystemDecorations = SystemDecorations.None;
        }
        private void Thread_DisposeEvent(object sender, EventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Close();
            });
        }
        public void Show(NotifyOptions options)
        {
            if (!options.IsVaidate)
                throw new NotSupportedException("when Position is Top***,the Scroll Way(ScollOrientation) cannot be Vertical!");
            try
            {
                Width = options.Size.Width;
                Height = options.Size.Height;
                Show();
                Options.Update(options);
                HandleAnimation(options.Position);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void HandleAnimation(ShowPosition showOn)
        {
            try
            {
                PixelPoint endPoint = default;
                int h = this.ActualHeight().ToInt32(), w = this.ActualWidth().ToInt32();
                int sw = Screens.Primary.WorkingArea.Width, sh = Screens.Primary.WorkingArea.Height;
                switch (showOn)
                {
                    case ShowPosition.BottomLeft:
                        {
                            var top = sh - h;
                            Position = new PixelPoint(0, top);
                            endPoint = Options.ScollOrientation == ScollOrientation.Vertical ? new PixelPoint(0, 0) : new PixelPoint(-w, 0);
                            break;
                        }
                    case ShowPosition.BottomRight:
                        {
                            int left = sw - w, top = sh - h;
                            Position = new PixelPoint(left, top);
                            endPoint = Options.ScollOrientation == ScollOrientation.Vertical ? new PixelPoint(left, 0) : new PixelPoint(sw, top);
                            break;
                        }
                    case ShowPosition.TopLeft:
                        {
                            Position = new PixelPoint(0, 0);
                            endPoint = new PixelPoint(-w, 0);
                            break;
                        }
                    case ShowPosition.TopRight:
                        {
                            var left = sw - w;
                            Position = new PixelPoint(left, 0);
                            endPoint = new PixelPoint(sw, 0);
                            break;
                        }
                }
                Thread.SetPath(Position, endPoint);
                Thread.Start(Options);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
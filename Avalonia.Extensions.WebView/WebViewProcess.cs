using CefNet;

namespace Avalonia.Extensions.WebView
{
    public sealed class WebViewProcess
    {
        private static WebViewProcess instance;
        public static WebViewProcess Instance
        {
            get
            {
                if (instance == null)
                    instance = new WebViewProcess();
                return instance;
            }
        }
        private WebViewProcess()
        {

        }
        public static CefNetApplication CefApp => AppBuilderDesktopExtensions.CefApp;
    }
}
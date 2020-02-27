using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Threading;
using TelegramCoinMiner.CefHandlers;

namespace TelegramCoinMiner
{
    static class WebBrowserFactory
    {
        static WebBrowserFactory()
        {
            var setting = new CefSettings();

            setting.LogSeverity = LogSeverity.Disable;
            Cef.EnableHighDPISupport();
            // Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(setting, performDependencyCheck: false, browserProcessHandler: null);
        }

        public static ChromiumWebBrowser CreateWebBrowser()
        {
            var browser = new ChromiumWebBrowser();

            // wait till browser initialised
            AutoResetEvent waitHandle = new AutoResetEvent(false);

            EventHandler onBrowserInitialized = null;

            onBrowserInitialized = (sender, e) =>
            {
                browser.BrowserInitialized -= onBrowserInitialized;
                browser.LifeSpanHandler = new LifeSpanHandler();
                browser.JsDialogHandler = new JSDialogHandler();
                waitHandle.Set();
            };

            browser.BrowserInitialized += onBrowserInitialized;

            waitHandle.WaitOne();
            return browser;
        }

        public static void ShutdownWebBrowsers()
        {
            Cef.Shutdown();
        }
    }
}

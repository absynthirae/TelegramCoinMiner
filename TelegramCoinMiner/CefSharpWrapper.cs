using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Threading;
using System.Threading.Tasks;
using TelegramCoinMiner.CefHandlers;
namespace TelegramCoinMiner
{
    public class JavascriptException : Exception
    {
        public JavascriptException(string message) : base(message) { }
    }

    public sealed class CefSharpWrapper
    {
        public ChromiumWebBrowser _browser; //соврменнные прорблемы ...

        public void InitializeBrowser()
        {

            var setting = new CefSettings();
            
            setting.LogSeverity = LogSeverity.Disable;
            Cef.EnableHighDPISupport();
            // Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(setting, performDependencyCheck: false, browserProcessHandler: null);

            _browser = new ChromiumWebBrowser();

            // wait till browser initialised
            AutoResetEvent waitHandle = new AutoResetEvent(false);

            EventHandler onBrowserInitialized = null;

            onBrowserInitialized = (sender, e) =>
            {
                _browser.BrowserInitialized -= onBrowserInitialized;
                _browser.LifeSpanHandler = new LifeSpanHandler();
                _browser.JsDialogHandler = new JSDialogHandler();
                waitHandle.Set();
            };

            _browser.BrowserInitialized += onBrowserInitialized;

            waitHandle.WaitOne();
        }

        public void ShutdownBrowser()
        {
            // Clean up Chromium objects.  You need to call this in your application otherwise
            // you will get a crash when closing.
            Cef.Shutdown();
        }

        public Task<T> GetResultAfterPageLoad<T>(string pageUrl, Func<Task<T>> onLoadCallback)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

            EventHandler<LoadingStateChangedEventArgs> onPageLoaded = null;

            T t = default(T);

            // An event that is fired when the first page is finished loading.
            // This returns to us from another thread.
            onPageLoaded = async (sender, e) =>
            {
                // Check to see if loading is complete - this event is called twice, one when loading starts
                // second time when it's finished
                // (rather than an iframe within the main frame).
                if (!e.IsLoading)
                {
                    // Remove the load event handler, because we only want one snapshot of the initial page.
                    _browser.LoadingStateChanged -= onPageLoaded;

                    t = await onLoadCallback();

                    tcs.SetResult(t);
                }
            };

            _browser.LoadingStateChanged += onPageLoaded;

            _browser.Load(pageUrl);

            return tcs.Task;
        }

        public async Task EvaluateJavascript(string script)
        {
            JavascriptResponse javascriptResponse = await _browser.GetMainFrame().EvaluateScriptAsync(script);

            if (!javascriptResponse.Success)
            {
                throw new JavascriptException(javascriptResponse.Message);
            }
        }

        public async Task<T> EvaluateJavascript<T>(string script)
        {
            JavascriptResponse javascriptResponse = await _browser.GetMainFrame().EvaluateScriptAsync(script);

            if (javascriptResponse.Success)
            {
                object scriptResult = javascriptResponse.Result;
                return ConvertHelper.ToTypedVariable<T>(scriptResult);
            }

            throw new JavascriptException(javascriptResponse.Message);
        }
    }
}

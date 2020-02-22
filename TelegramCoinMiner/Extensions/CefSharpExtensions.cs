using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;

namespace TelegramCoinMiner
{
    public static class CefSharpExtensions
    {
        public static Task LoadPageAsync(this ChromiumWebBrowser browser, string address = null)
        {
            
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                //Wait for while page to finish loading not just the first frame
                if (!args.IsLoading)
                {
                    browser.LoadingStateChanged -= handler;
                    //Important that the continuation runs async using TaskCreationOptions.RunContinuationsAsynchronously
                    tcs.TrySetResult(true);
                }
            };

            browser.LoadingStateChanged += handler;

            if (!string.IsNullOrEmpty(address))
            {
                //browser.EvaluateScriptAsync("window.alert = function() {};");
                //browser.EvaluateScriptAsync("window.prompt = function() {};");
               // browser.EvaluateScriptAsync("window.confirm = function() {};");

                browser.Load(address);
            }
            return tcs.Task;
        }

        public async static Task<string> GetHtmlAfterPageLoad(this ChromiumWebBrowser browser, string url)
        {
            await browser.LoadPageAsync(url);
            var html = await browser.GetSourceAsync();
            return html;
        }

        public async static Task<bool> HasDogeclickCapcha(this ChromiumWebBrowser browser)
        {
            var html = await browser.GetSourceAsync();
            if (browser.Address.StartsWith("http://dogeclick.com/") &&
                (html.Contains("Please solve the reCAPTCHA to continue") || 
                html.Contains("Please solve the puzzle to continue"))
                )
            {
                return true;
            }
            return false;
        }

        public static void CheckSpecificTaskAndSetHasFocusFunc(this ChromiumWebBrowser browser)
        {
            
            if (browser.Address.StartsWith("http://dogeclick.com/"))
            {
                Console.WriteLine("Обнаружена специфичная заадча");
               // browser.EvaluateScriptAsync("document.__proto__.hasFocus = function() {return true}");
            }
        }
    }
}

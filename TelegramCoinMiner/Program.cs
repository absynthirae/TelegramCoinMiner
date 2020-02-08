using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCoinMiner
{
    class Program
    {
        private static void Main()
        {
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            CefSharpWrapper wrapper = new CefSharpWrapper();

            wrapper.InitializeBrowser();

            string[] urls = await wrapper.GetResultAfterPageLoad("https://yandex.ru", async () =>
                await wrapper.EvaluateJavascript<string[]>("$('a[href]').map((index, element) => $(element).prop('href')).toArray()"));

            wrapper.ShutdownBrowser();
        }
    }
}

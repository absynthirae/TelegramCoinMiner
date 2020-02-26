using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;

namespace TelegramCoinMiner
{
    class Program
    {
        static CefSharpWrapper wrapper = new CefSharpWrapper();
        
        private static void Main()
        {
            wrapper.InitializeBrowser();
            MainAsync().Wait();
            Console.ReadKey();
            wrapper.ShutdownBrowser();
        }

        private static async Task MainAsync()
        {
            int apiId = 1038521;

            string apiHash = "e365dd8b6c6336da17a4537f5fae2870"; //API-key Tema

            Console.WriteLine("Enter phone number as +71234567890");

            string phone = Console.ReadLine();

            //TelegramClientWrapper telegramClient = new TelegramClientWrapper(apiId, apiHash, phone, wrapper._browser);

            //await telegramClient.Start();

            var client = await TelegramClientFactory.CreateTelegramClientAsync(phone);

            var workerPool = new ClickBotWorkerPool(new List<LaunchClickBotParams>() {
                new LaunchClickBotParams
                {
                    TelegramClient = client,
                    Browser = wrapper._browser,
                    TokenSource = new CancellationTokenSource()
                }
            });

            workerPool.Start();

            if (Console.ReadKey().Key == ConsoleKey.Escape) 
            {
                //telegramClient.Stop();
                workerPool.Stop();
                Console.WriteLine("Вы прервали процесс");
            }
        }
    }
}

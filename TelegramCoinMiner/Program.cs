using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;

namespace TelegramCoinMiner
{
    class Program
    {        
        private static void Main()
        {
            MainAsync().Wait();
            Console.ReadKey();
            WebBrowserFactory.ShutdownWebBrowsers();
        }

        private static async Task MainAsync()
        {
            int apiId = 1038521;
            string apiHash = "e365dd8b6c6336da17a4537f5fae2870"; //API-key Tema

            Console.WriteLine("Enter phone number as 79123456789");

            string phone = Console.ReadLine();

            var client = await TelegramClientFactory.CreateTelegramClientAsync(phone);

            var workerPool = new ClickBotWorkerPool(new List<LaunchClickBotParams>() {
                new LaunchClickBotParams
                {
                    TelegramClient = client,
                    Browser = WebBrowserFactory.CreateWebBrowser(),
                    TokenSource = new CancellationTokenSource()
                }
            });

            workerPool.Start();

            //if (Console.ReadKey().Key == ConsoleKey.Escape) 
            //{
            //    workerPool.Stop();
            //    Console.WriteLine("Вы прервали процесс");
            //}
        }
    }
}

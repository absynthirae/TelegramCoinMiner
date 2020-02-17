using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

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

            TelegramClientWrapper telegramClient = new TelegramClientWrapper(apiId, apiHash, phone, wrapper._browser);

            await telegramClient.Start(CoinClickBotInfo.CreateBitcoinClickBotInfo());

            if (Console.ReadKey().Key == ConsoleKey.Escape) {           
                telegramClient.Stop();
                Console.WriteLine("Вы прервали процесс");
            }


        }
    }
}

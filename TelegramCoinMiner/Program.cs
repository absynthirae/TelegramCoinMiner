using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        private static async Task MainAsync()
        {
            int apiId = 1038521;
            string apiHash = "e365dd8b6c6336da17a4537f5fae2870"; //API-key Tema

            //Console.WriteLine("Enter phone number as 79123456789");

            //string phone = Console.ReadLine();

            //var client = await TelegramClientFactory.CreateTelegramClientAsync(phone);

            //var workerPool = new ClickBotWorkerPool(new List<LaunchClickBotParams>() {
            //    new LaunchClickBotParams
            //    {
            //        TelegramClient = client,
            //        Browser = WebBrowserFactory.CreateWebBrowser(),
            //        TokenSource = new CancellationTokenSource()
            //    }
            //});

            //workerPool.Start();

            var workerPool = new ClickBotWorkerPool();
            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("Вы вошли в режим командной строки. Введите /exit чтобы выйти");
                    while (true)
                    {
                        var command = Console.ReadLine();

                        if (command.StartsWith("/exit"))
                        {
                            break;
                        }

                        if (command.StartsWith("/add"))
                        {
                            var comParams = command.Split(' ');
                            var phoneNumber = comParams.Last();

                            Console.WriteLine("Добавить клиент с номером телефона " + phoneNumber + "? y/n");
                            if (Console.ReadKey().Key == ConsoleKey.Y)
                            {
                                workerPool.Add(new LaunchClickBotParams
                                {
                                    TelegramClient = await TelegramClientFactory.CreateTelegramClientAsync(phoneNumber),
                                    Browser = WebBrowserFactory.CreateWebBrowser(),
                                    TokenSource = new CancellationTokenSource()
                                });
                            }
                        }

                        if (command.StartsWith("/start"))
                        {
                            workerPool.Start();
                            Console.WriteLine("Вы запустили Worker Pool");
                        }

                        if (command.StartsWith("/stop"))
                        {
                            workerPool.Stop();
                            Console.WriteLine("Вы остановили Worker Pool");
                            WebBrowserFactory.ShutdownWebBrowsers();
                            Console.WriteLine("Браузер выключен");
                            return;
                        }
                    }
                }
            }
        }
    }
}

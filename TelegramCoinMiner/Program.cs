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

            #region TargetData

            string botName = "BTC Click Bot";

            #endregion

            Console.WriteLine("Enter phone number as +71234567890");
            string phone = Console.ReadLine();

            bool sessionExist = File.Exists(phone + ".dat");

            var client = new TelegramClient(apiId, apiHash, sessionUserId: phone); //cleint
            await client.ConnectAsync();

            if (!sessionExist)
            {
                var hash = await client.SendCodeRequestAsync(phone);
                Console.WriteLine("Enter telegram code");
                string code = Console.ReadLine();
                var user = await client.MakeAuthAsync(phone, hash, code);
            }

            var dialogs = (TLDialogs)await client.GetUserDialogsAsync();

            var channel = dialogs.Users
                .OfType<TLUser>()
                .FirstOrDefault(c => c.FirstName == botName);

            var messages = (await client.GetMessages((long)channel.AccessHash, channel.Id, 5))
                .Messages
                .OfType<TLMessage>();
        }
    }
}

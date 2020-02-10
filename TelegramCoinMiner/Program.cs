using System;
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

            TLMessagesSlice res = await client.GetMessages((long)channel.AccessHash, channel.Id);


            foreach (var mes in res.Messages)
            { //смотрим сообщение
                TLMessage sms = mes as TLMessage;
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine(sms.Message); //вывод сообщений

                TLReplyInlineMarkup Mark = sms.ReplyMarkup as TLReplyInlineMarkup;
                if (Mark == null) { continue; } //Если сообщение не содержит кнопок со ссылками
                var rows = Mark.Rows;

                foreach (TLKeyboardButtonRow keyRow in rows)  //бегаем по строкам 
                {
                    TLVector<TLAbsKeyboardButton> buttons = keyRow.Buttons;  //берем все кнопки

                    Console.WriteLine($"Buttons {buttons.Count}");
                    if (buttons.Count <= 0) { continue; };

                    foreach (TLAbsKeyboardButton keyBtn in buttons) //бегаем по взятым кнопкам 
                    {
                        if (!(keyBtn is TLKeyboardButtonUrl)) { continue; }
                        var key = (TLKeyboardButtonUrl)keyBtn;
                        if (key.Text.Contains("Go to website"))
                        { //если кнопка содержит текст то:
                            /*Переход по ссылке*/
                            /*
                            Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\Chrome.exe" + " " + key.Url);
                            await Task.Delay(15000);
                            foreach (var proc in Process.GetProcessesByName("Chrome.exe")) {
                            proc.Kill();
                            }
                            */
                            Console.WriteLine("Был переход по ссылке:" + key.Url);
                            await wrapper.GetResultAfterPageLoad(key.Url, async () => { return Task.Delay(15000); });
                        }
                    }
                }
                Console.WriteLine(sms.Message);
            }
        }
    }
}

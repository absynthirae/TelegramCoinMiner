using System;
using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;
using TelegramCoinMiner.Exceptions;
using TelegramCoinMiner.Extensions;

namespace TelegramCoinMiner.Commands
{
    public class WatchAdCommand : IAsyncCommand
    {
        public WatchAdParams Params { get; set; }

        public WatchAdCommand(WatchAdParams commandParams)
        {
            Params = commandParams;
        }
        
        public async Task Execute()
        {
            Console.WriteLine("-----------------------------------" + DateTime.Now.ToString("hh:mm:ss"));
            var adMessage = Params.AdMessage;

            if (adMessage == null)
            {
                throw new AdMessageNotFoundException();
            }

            var goToWebsiteButton = adMessage.GetButtonWithUrl("go to website");
            string url = goToWebsiteButton.Url;
            Console.WriteLine("URL:" + url);

            if (!Params.Browser.LoadPageAsync(url).Wait(60000))
            {
                Console.WriteLine("Подозрение на DDos => пропуск");
                throw new BrowserTimeoutException();
            }

            Console.WriteLine("Страница загружена");

            if (await Params.Browser.HasDogeclickCapcha())
            {
                Console.WriteLine("Капча");
                throw new CapchaException();
            }
            else
            {
                Console.WriteLine("Нет капчи");
            }


            if (!Task.Run(() => Params.Browser.CheckSpecificTaskAndSetHasFocusFunc()).Wait(60000))
            {
                Console.WriteLine("Подозрение на DDos => пропуск");
                throw new BrowserTimeoutException();
            }
           
        }
    }
}

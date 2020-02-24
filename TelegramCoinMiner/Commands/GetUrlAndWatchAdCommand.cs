using System;
using System.Linq;
using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;
using TelegramCoinMiner.Extensions;
using TeleSharp.TL;
using TeleSharp.TL.Messages;

namespace TelegramCoinMiner.Commands
{
    public class GetUrlAndWatchAdCommand : IAsyncCommand
    {
        public GetUrlAndWatchAdParams Params { get; set; }
        public GetUrlAndWatchAdCommand(GetUrlAndWatchAdParams commandParams)
        {
            Params = commandParams;
        }
        //Press the "Visit website" button to earn
        public async Task Execute()
        {
            Console.WriteLine("-----------------------------------" + DateTime.Now.ToString("hh:mm:ss"));
            var messages = await Params.TelegramClient.GetMessages(Params.BotChannel, Params.BotInfo.ReadMessagesCount);

            var adMessage = messages.OfType<TLMessage>().FirstOrDefault(x => x.Message.Contains("Press the \"Visit website\" button to earn"));
            var goToWebsiteButton = adMessage.GetButtonWithUrl("go to website");
            string url = goToWebsiteButton.Url;
            Console.WriteLine("URL:" + url);

            if (!Params.Browser.LoadPageAsync(url).Wait(60000))
            {
                Console.WriteLine("Подозрение на DDos => пропуск");
                await SkipTask(adMessage);
                throw new Exception("Browser Timeout");
            }

            Console.WriteLine("Страница загружена");

            if (await Params.Browser.HasDogeclickCapcha())
            {
                Console.WriteLine("Капча");
                await SkipTask(adMessage);
                throw new Exception("Capcha");
            }
            else
            {
                Console.WriteLine("Нет капчи");
            }


            if (!Task.Run(() => Params.Browser.CheckSpecificTaskAndSetHasFocusFunc()).Wait(60000))
            {
                Console.WriteLine("Подозрение на DDos => пропуск");
                await SkipTask(adMessage);
                throw new Exception("Browser Timeout");
            };
        }

        private async Task SkipTask(TLMessage adMessage)
        {
            var skipCallbackButton = adMessage.GetButtonWithCallBack("Skip");
            var data = skipCallbackButton.Data;
            var messageId = adMessage.Id;
            await Params.TelegramClient.SendRequestAsync<object>(
                new TLRequestGetBotCallbackAnswer()
                {
                    Peer = new TLInputPeerUser() { UserId = Params.BotChannel.Id, AccessHash = Params.BotChannel.AccessHash.Value },
                    Data = data,
                    MsgId = messageId
                });
        }
    }
}

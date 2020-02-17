using CefSharp.OffScreen;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TLSharp.Core;
using CefSharp;
using TeleSharp.TL.Messages;

namespace TelegramCoinMiner
{
    public class TelegramClientWrapper
    {
        private bool _sessionExist = false;
        private TelegramClient _client;
        private Task _workerThread;
        private ChromiumWebBrowser _browser;
        private bool _clientIsConnected = false;
     //   private const int _readMessagesCount = 2; //чтобы не всегда пять сообщений брать
        public bool IsStarted { get; private set; } = false;

        public TelegramClientWrapper(int apiId, string apiHash, string phone, ChromiumWebBrowser browser)
        {
            _sessionExist = File.Exists(phone + ".dat");
            _client = new TelegramClient(apiId, apiHash, sessionUserId: phone);
            _browser = browser;
            
        }

        private async Task ConnectAsync()
        {
            await _client.ConnectAsync();

            if (!_sessionExist)
            {
                throw new Exception("Session not exist");
            }
        }

        public async Task Start(CoinClickBotInfo bot)
        {         
                try
                {
                    await ConnectAsync();
                _clientIsConnected = true;
                }
                catch (Exception) {_clientIsConnected = false; }

            if (_clientIsConnected)
            {

                IsStarted = true;
                var botChannel = await _client.GetChannelByName(bot.BotName);

                await _client.SendMessageAsync(new TLInputPeerUser() { UserId = botChannel.Id, AccessHash = botChannel.AccessHash.Value }, "/visit"); //стартуем бот-канал с той стороны
                await Task.Delay(2000);
                _workerThread = new Task(() => InvokeAlgoritm(botChannel, bot.ReadMessagesCount)); //возможно надо счётчик сообщений в параметры пихнуть
                _workerThread.Start();
            }
            else { throw new Exception("Коннект провален"); }
        }

        public void Stop()
        {
            IsStarted = false;
            if (_workerThread != null)
            {
                _workerThread.Wait();
                _workerThread = null;
            }
        }

        private async Task InvokeAlgoritm(TLUser botChannel, int Count)
        {
            while (IsStarted)
            {
                Console.WriteLine("Начало метода");
                var messages = await _client.GetMessages(botChannel.AccessHash.Value, botChannel.Id, Count); //сообщения
                
                var url = messages
                    .OfType<TLMessage>()
                    .GetButtonWithUrl("go to website")
                    .Url;

                Console.WriteLine("URL:"+url);

                _browser.Load(url);
                await Task.Delay(2000);

                Console.WriteLine("Перешли по ссылке");

                var curentHtml = await _browser.GetSourceAsync();

               // Console.WriteLine(curentHtml);

                Console.WriteLine("-------------------------------");
                if (curentHtml.HasCaptcha())
                {
                    //Надо скипнуть задачу в телеге
                    Console.WriteLine("Капча");
                    Console.WriteLine("-------------------------------");
                    //скипнуть надо

                    var btn = messages.OfType<TLMessage>().GetButtonWithCallBack("Skip");

                    var data = ((TeleSharp.TL.TLKeyboardButtonCallback)btn).Data;

                    var res = await _client.SendRequestAsync<object>(new TLRequestGetBotCallbackAnswer() {
                        Peer = new TLInputPeerUser() { UserId = botChannel.Id, AccessHash = botChannel.AccessHash.Value },
                        Data = data,
                        MsgId = messages.Select(x=>x).OfType<TLMessage>().FirstOrDefault(x=>x.Message.ToLower().Contains("visit website")).Id});
                    //фыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыы
                    continue;

                }//втупую капча1
               
                int time = 15;
                await Task.Delay(2000);
                (await _client.GetMessages(botChannel.AccessHash.Value, botChannel.Id, Count))
                    .OfType<TLMessage>().Where(x=>x.Message.Contains("seconds")).FirstOrDefault()
                    .Message.Split(new char[] { ' ' }).FirstOrDefault(x=>int.TryParse(x, out time ));
                

                await Task.Delay(time*1000); //надо подумать сколько ждать потмоу что часто сообщение приходит после перехода и после нажатия на кнопку
                var GoodMess= (await _client.GetMessages(botChannel.AccessHash.Value, botChannel.Id, Count)).OfType<TLMessage>().FirstOrDefault();
              await Task.Delay(5000);
                Console.WriteLine(GoodMess.Message.ToString());
            }
        }
    }
}

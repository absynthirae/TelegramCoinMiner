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
        public bool IsStarted { get; private set; } = false;
        private CoinClickBotInfo _botInfo = CoinClickBotInfo.CreateBitcoinClickBotInfo();

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

        public async Task Start()
        {
            try
            {
                await ConnectAsync();
                _clientIsConnected = true;
            }
            catch (Exception) { _clientIsConnected = false; }

            if (_clientIsConnected)
            {

                IsStarted = true;
                var botChannel = await _client.GetChannelByName(_botInfo.BotName);

                await _client.SendMessageAsync(new TLInputPeerUser() { UserId = botChannel.Id, AccessHash = botChannel.AccessHash.Value }, "/visit"); //стартуем бот-канал с той стороны
                await Task.Delay(2000);
                _workerThread = new Task(() => InvokeAlgoritm(botChannel)); //возможно надо счётчик сообщений в параметры пихнуть
                _workerThread.Start();
            }
            else 
            { 
                throw new Exception("Коннект провален"); 
            }
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

        private async Task InvokeAlgoritm(TLUser botChannel)
        {
            
            while (IsStarted)
            {
                Console.WriteLine("Начало метода");
                var messages = await _client.GetMessages(botChannel.AccessHash.Value, botChannel.Id, _botInfo.ReadMessagesCount);
                
                var url = messages
                    .OfType<TLMessage>()
                    .GetButtonWithUrl("go to website")
                    .Url;

                Console.WriteLine("URL:"+url);

                //need test
                //_browser.Load(url);
                //await Task.Delay(2000);
                await _browser.LoadPageAsync(url);

                Console.WriteLine("Перешли по ссылке");

                var curentHtml = await _browser.GetSourceAsync();

                //Console.WriteLine(curentHtml);

                Console.WriteLine("-----------------------------------");
                if (curentHtml.HasCaptcha())
                {
                    Console.WriteLine("Капча");
                    Console.WriteLine("-------------------------------");

                    var skipCallbackButton = messages.OfType<TLMessage>().GetButtonWithCallBack("Skip");

                    var data = skipCallbackButton.Data;

                    var messageId = messages.Select(x => x).OfType<TLMessage>().FirstOrDefault(x => x.Message.ToLower().Contains("visit website")).Id;

                    var res = await _client.SendRequestAsync<object>(
                        new TLRequestGetBotCallbackAnswer()
                        {
                            Peer = new TLInputPeerUser() { UserId = botChannel.Id, AccessHash = botChannel.AccessHash.Value },
                            Data = data,
                            MsgId = messageId
                        });

                    continue;
                }
               
                int time = 15;
                await Task.Delay(2000);
                (await _client.GetMessages(botChannel.AccessHash.Value, botChannel.Id, _botInfo.ReadMessagesCount))
                    .OfType<TLMessage>()
                    .Where(x => x.Message.Contains("seconds"))
                    .FirstOrDefault()
                    .Message
                    .Split(new char[] { ' ' })
                    .FirstOrDefault(x => int.TryParse(x, out time));

                await Task.Delay(time * 1000);

                await Task.Delay(3000);
            }
        }
    }
}

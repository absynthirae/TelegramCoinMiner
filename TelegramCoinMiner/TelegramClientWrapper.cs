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
        private bool _sessionExist = false; // Check of session-file exist
        private TelegramClient _client; 
        private Task _workerThread; //  worker

        private ChromiumWebBrowser _browser; 
        private bool _clientIsConnected = false;
        /// <summary>
        /// farm is started
        /// </summary>
        public bool IsStarted { get; private set; } = false; 

        private CoinClickBotInfo _botInfo = CoinClickBotInfo.CreateBitcoinClickBotInfo();

        public TelegramClientWrapper(int apiId, string apiHash, string phone, ChromiumWebBrowser browser)
        {
            _sessionExist = File.Exists(phone + ".dat");
            _client = new TelegramClient(apiId, apiHash, sessionUserId: phone);
            _browser = browser;
            
            _browser.LifeSpanHandler = new LifeSpanHandler();
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
                //await SendVisitCommand(botChannel);
                //await Task.Delay(2000);
                _workerThread = new Task(() => InvokeAlgoritm(botChannel)); //возможно надо счётчик сообщений в параметры пихнуть
                try
                {
                    _workerThread.Start();
                }
                catch
                {
                    Console.WriteLine("После неизвестной ошибки проект перезапускается");
                    await Start();
                }


            }
            else
            {
                throw new Exception("Коннект провален");
            }
        }

        private async Task SendVisitCommand(TLUser channel)
        {
            await _client.SendMessageAsync(new TLInputPeerUser() { UserId = channel.Id, AccessHash = channel.AccessHash.Value }, "/visit"); //стартуем бот-канал с той стороны
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



            await SendVisitCommand(botChannel);
            await Task.Delay(2000);
            while (IsStarted)
            {
                try
                {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("Начало метода");
                    var messages = await _client.GetMessages(botChannel.AccessHash.Value, botChannel.Id, _botInfo.ReadMessagesCount);

                    var url = messages
                        .OfType<TLMessage>()
                        .GetButtonWithUrl("go to website")
                        .Url;

                    Console.WriteLine("URL:" + url);


                    await _browser.LoadPageAsync(url);

                    Console.WriteLine("Перешли по ссылке");

                    var curentHtml = await _browser.GetSourceAsync();




                    if (curentHtml.HasCaptcha())
                    {
                        Console.WriteLine("Капча");


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

                    Console.WriteLine("Всё прошло нормально " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Нет ссылок - запрос на ссылки");
                    await SendVisitCommand(botChannel); //означает что выдало два сообщения не тех подряд и среди них не было ссылки
                    await Task.Delay(1000);            //добавили делэй

                }
                catch (Exception)                       //не известное исключение => перезапуск
                {
                    Console.WriteLine("Неизвестная ошибка");
                    await SendVisitCommand(botChannel);
                    await Task.Delay(1000);
                    throw new Exception("FatalError");
                }

            }


        }
    }
}

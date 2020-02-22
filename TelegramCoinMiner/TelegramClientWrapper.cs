using CefSharp.OffScreen;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TLSharp.Core;
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
            try
            {
                await _client.ConnectAsync();
                _clientIsConnected = true;
            }
            catch
            {
                _clientIsConnected = false;
                Console.WriteLine("Не удалось произвести подключние");
            }

            if (!_sessionExist)
            {
                throw new Exception("Session not exist");
            }
        }

        public async Task Start()
        {
            await ConnectAsync();

            if (_clientIsConnected)
            {
                IsStarted = true;

                var botChannel = await _client.GetChannelByName(_botInfo.BotName);
            
                _workerThread = new Task(() => InvokeAlgoritm(botChannel)); //возможно надо счётчик сообщений в параметры пихнуть
                _workerThread.Start();
            }
            else 
            {
                throw new Exception("Нет подключения");
            }
        }
 
        public void Stop()
        {
           
            if (_workerThread != null)
            {
                _workerThread.Wait();
                _workerThread = null;
            }
            IsStarted = false;
        }

        private async Task InvokeAlgoritm(TLUser botChannel)
        {
            await SendVisitCommand(botChannel);

            while (IsStarted)
            {
                try
                {
                    //Wait task message
                    await Task.Delay(2000);
                    Console.WriteLine("-----------------------------------");
                    var messages = await _client.GetMessages(botChannel, _botInfo.ReadMessagesCount);

                    string url = GetUrlFromTaskMessage(messages);
                    Console.WriteLine("URL:" + url);

                    await _browser.LoadPageAsync(url);
                    Console.WriteLine("Страница загружена");

                    //Wait message about task wait time
                    await Task.Delay(1500);

                    if (await _browser.HasDogeclickCapcha())
                    {
                        Console.WriteLine("Капча");
                        await SkipTask(botChannel, messages);
                        continue;
                    }

                    _browser.CheckSpecificTaskAndSetHasFocusFunc();

                    await WaitTaskСompletion(botChannel);

                    Console.WriteLine("Задание выполнено " + DateTime.Now.ToString("hh:mm:ss"));
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Нет ссылок => запрос на ссылки");
                    await SendVisitCommand(botChannel); //означает что выдало два сообщения не тех подряд и среди них не было ссылки
                    await Task.Delay(1000);                
                }
            }
        }

        private async Task SendVisitCommand(TLUser channel)
        {
            await _client.SendMessageAsync(new TLInputPeerUser() { UserId = channel.Id, AccessHash = channel.AccessHash.Value }, "/visit"); //стартуем бот-канал с той стороны
        }

        private async Task WaitTaskСompletion(TLUser botChannel)
        {
            int time = await GetTaskWaitTimeInSeconds(botChannel);
            await Task.Delay(time * 1000);
        }

        /// <summary>
        /// Get messages from channel and parse task wait time 
        /// </summary>
        /// <param name="botChannel"></param>
        /// <returns></returns>
        private async Task<int> GetTaskWaitTimeInSeconds(TLUser botChannel)
        {
            //Default time
            int time = 15;
            (await _client.GetMessages(botChannel.AccessHash.Value, botChannel.Id, _botInfo.ReadMessagesCount))
                .OfType<TLMessage>()
                .Where(x => x.Message.Contains("seconds"))
                .FirstOrDefault()
                .Message
                .Split(new char[] { ' ' })
                .FirstOrDefault(x => int.TryParse(x, out time));
            return time;
        }

        private async Task SkipTask(TLUser botChannel, TLVector<TLAbsMessage> messages)
        {
            var skipCallbackButton = messages.OfType<TLMessage>().GetButtonWithCallBack("Skip");
            var data = skipCallbackButton.Data;
            var messageId = messages.Select(x => x).OfType<TLMessage>().FirstOrDefault(x => x.Message.ToLower().Contains("visit website")).Id;
            await _client.SendRequestAsync<object>(
                new TLRequestGetBotCallbackAnswer()
                {
                    Peer = new TLInputPeerUser() { UserId = botChannel.Id, AccessHash = botChannel.AccessHash.Value },
                    Data = data,
                    MsgId = messageId
                });
        }

        private static string GetUrlFromTaskMessage(TLVector<TLAbsMessage> messages)
        {
            return messages
                .OfType<TLMessage>()
                .GetButtonWithUrl("go to website")
                .Url;
        }
    }
}

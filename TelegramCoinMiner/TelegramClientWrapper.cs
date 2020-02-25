using CefSharp.OffScreen;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TLSharp.Core;
using TeleSharp.TL.Messages;
using TelegramCoinMiner.Extensions;
using TelegramCoinMiner.CefHandlers;
using TeleSharp.TL.Contacts;

namespace TelegramCoinMiner
{
    public class TelegramClientWrapper
    {
        /// <summary>
        /// farm is started
        /// </summary>
        public bool IsStarted { get; private set; } = false;

        /// <summary>
        /// Check of session-file exist
        /// </summary>
        private bool _sessionExist = false;
        private TelegramClient _client; 
        /// <summary>
        /// Worker
        /// </summary>
        private Task _workerThread;
        private ChromiumWebBrowser _browser; 
        private bool _clientIsConnected = false;
        private ClickBotInfo _botInfo = ClickBotInfo.CreateBitcoinClickBotInfo();

        public TelegramClientWrapper(int apiId, string apiHash, string phone, ChromiumWebBrowser browser)
        {
            _sessionExist = File.Exists(phone + ".dat");
            _client = new TelegramClient(apiId, apiHash, sessionUserId: phone);
            _browser = browser;
            _browser.LifeSpanHandler = new LifeSpanHandler();
            _browser.JsDialogHandler = new JSDialogHandler();
          //  _browser.RequestHandler = new DefaultRequestHandler();
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
                TLFound found = await _client.SearchUserAsync(_botInfo.BotName);
                var botChannel = found.Chats.OfType<TLChannel>().FirstOrDefault(x => x.Username == _botInfo.BotName || x.Title == _botInfo.BotName);
            
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

        private async Task InvokeAlgoritm(TLChannel botChannel)
        {
           
            await SendVisitCommand(botChannel);

            while (IsStarted)
            {
                try
                {
                    #region GetUrlAndWatchAdCommand
                    Console.WriteLine("-----------------------------------" + DateTime.Now.ToString("hh:mm:ss"));
                    var messages = await _client.GetMessages(botChannel, _botInfo.ReadMessagesCount);

                    string url = GetUrlFromTaskMessage(messages);
                    Console.WriteLine("URL:" + url);

                    if (!_browser.LoadPageAsync(url).Wait(60000))
                    {
                        Console.WriteLine("Подозрение на DDos => пропуск");
                        await SkipTask(botChannel, messages);          
                        continue;
                    }


                    Console.WriteLine("Страница загружена");

                    if (await _browser.HasDogeclickCapcha())
                    {
                        Console.WriteLine("Капча");
                        await SkipTask(botChannel, messages);
                        continue;
                    }
                    else 
                    {
                        Console.WriteLine("Нет капчи"); 
                    }


                    if (!Task.Run(()=>_browser.CheckSpecificTaskAndSetHasFocusFunc()).Wait(60000)) 
                    {
                        Console.WriteLine("Подозрение на DDos => пропуск");
                        await SkipTask(botChannel, messages);
                        continue;
                    };
                    #endregion

                    await WaitTaskСompletion(botChannel);

                    Console.WriteLine("Задание выполнено " + DateTime.Now.ToString("hh:mm:ss"));
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Нет ссылок => запрос на ссылки");
                    await SendVisitCommand(botChannel); //означает что выдало два сообщения не тех подряд и среди них не было ссылки
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Send message "/visit" in telegram chat
        /// </summary>
        /// <param name="channel">Bot channel</param>
        /// <returns></returns>
        private async Task SendVisitCommand(TLChannel channel)
        {
            await _client.SendMessageAsync(new TLInputPeerUser() { UserId = channel.Id, AccessHash = channel.AccessHash.Value }, "/visit");
            //Wait task message
            await Task.Delay(2000);
        }

        private async Task WaitTaskСompletion(TLChannel botChannel)
        {
            int time = await GetTaskWaitTimeInSeconds(botChannel);
            Console.WriteLine("Время ожидания: "+time);
            await Task.Delay(time * 1000 + 1000);
        }

        /// <summary>
        /// Get messages from channel and parse task wait time 
        /// </summary>
        /// <param name="botChannel"></param>
        /// <returns></returns>
        private async Task<int> GetTaskWaitTimeInSeconds(TLChannel botChannel)
        {
            //Wait message about task wait time
            await Task.Delay(1500);
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

        private async Task SkipTask(TLChannel botChannel, TLVector<TLAbsMessage> messages)
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
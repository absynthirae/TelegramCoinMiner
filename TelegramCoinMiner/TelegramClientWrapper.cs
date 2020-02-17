using CefSharp.OffScreen;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TLSharp.Core;
using CefSharp;

namespace TelegramCoinMiner
{
    public class TelegramClientWrapper
    {
        private bool _sessionExist = false;
        private TelegramClient _client;
        private Task _workerThread;
        private ChromiumWebBrowser _browser;
        private const int _readMessagesCount = 2; //чтобы не всегда пять сообщений брать
        public bool IsStarted { get; private set; } = false;

        public TelegramClientWrapper(int apiId, string apiHash, string phone, ChromiumWebBrowser browser)
        {
            _sessionExist = File.Exists(phone + ".dat");
            _client = new TelegramClient(apiId, apiHash, sessionUserId: phone);
            _browser = browser;
        }

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync();

            if (!_sessionExist)
            {
                throw new Exception("Session not exist");
            }
        }

        public async Task Start(string botName)
        {
            IsStarted = true;
            var botChannel = await _client.GetChannelByName(botName);      
            _workerThread = new Task(async () => await InvokeAlgoritm(botChannel)); //возможно надо счётчик сообщений в параметры пихнуть
            _workerThread.Start();
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
                var messages = await _client.GetMessages(botChannel.AccessHash.Value, botChannel.Id, _readMessagesCount); //сообщения
                var url = messages
                    .OfType<TLMessage>()
                    .GetButtonWithUrl("go to website")
                    .Url;

                _browser.Load(url);
                var curentHtml = await _browser.GetSourceAsync();

                if (curentHtml.HasCaptcha())
                {
                    //Надо скипнуть задачу в телеге
                    continue;
                }//втупую капча1

                int time = 15; 
                (await _client.GetMessages(botChannel.AccessHash.Value, botChannel.Id, _readMessagesCount))
                    .OfType<TLMessage>().Where(x=>x.Message.Contains("seconds")).FirstOrDefault()
                    .Message.Split(new char[] { ' ' }).FirstOrDefault(x=>int.TryParse(x, out time ));
                

                await Task.Delay(time); //надо подумать сколько ждать потмоу что часто сообщение приходит после перехода и после нажатия на кнопку
            }
        }
    }
}

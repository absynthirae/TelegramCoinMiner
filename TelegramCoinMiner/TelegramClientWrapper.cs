using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
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
        private int mesCount { get; set; } = 2; //чтобы не всегда пять сообщений брать
        public bool IsStarted { get; private set; } = false;

        public TelegramClientWrapper(int apiId, string apiHash, string phone, ChromiumWebBrowser browser)
        {
            _sessionExist = File.Exists(phone + ".dat");
            _client = new TelegramClient(apiId, apiHash, sessionUserId: phone);
            _browser = browser;
        }

        async Task ConnectAsync()
        {
            await _client.ConnectAsync();

            if (!_sessionExist)
            {
                throw new Exception("Session not exist");
            }
        }

        public void Start(string botName, string keyTxt)
        {
            IsStarted = true;
            _workerThread = new Task(()=>InvokeAlgoritm(botName, mesCount, keyTxt)); //возможно надо счётчик сообщений в параметры пихнуть
            _workerThread.Start(); ;
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

        private void InvokeAlgoritm(string botName, int mesCount, string keyTxt)
        {
            
                //Do smth
                var dialogs = (TLDialogs)_client.GetUserDialogsAsync().Result; //получаем все диалоги

                var channel = dialogs.Users
                    .OfType<TLUser>()
                    .FirstOrDefault(c => c.FirstName.ToLower() == botName.ToLower()); //берем нужный нам канал(юзер потому что бот)
            long Access=0; //поставить хэш телеграма
            int Id = 0;//аналогично

            if (channel.AccessHash.HasValue) {
                Access = channel.AccessHash.Value;
                Id = channel.Id;
            }
            

                while (IsStarted)
                {     
                
                var messanges = _client.GetMessages(Access, channel.Id, mesCount).Result; //сообщения
                var url = messanges.Messages.OfType<TLMessage>().GetButtonWithUrl(keyTxt).Url; //ссылка сама
                _browser.Load(url);
                var curentHtml = _browser.GetSourceAsync().Result;

                if (curentHtml.hasCaptcha()) {

                 //Надо скипнуть задачу в телеге
                    
                    continue;
                
                }//втупую капча
                
                Task.Delay(15000); //надо подумать сколько ждать потмоу что часто сообщение приходит после перехода и после нажатия на кнопку
                              
                }
            
        }
    }
}

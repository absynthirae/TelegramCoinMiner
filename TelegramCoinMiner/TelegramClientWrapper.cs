using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TLSharp.Core;

namespace TelegramCoinMiner
{
    public class TelegramClientWrapper
    {
        private bool _sessionExist = false;
        private TelegramClient _client;
        private Thread _workerThread;
        private ChromiumWebBrowser _browser;
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

        public void Start()
        {
            IsStarted = true;
            _workerThread = new Thread(new ThreadStart(InvokeAlgoritm));
            _workerThread.Start();
        }

        public void Stop()
        {
            IsStarted = false;
            if (_workerThread != null)
            {
                _workerThread.Join();
                _workerThread.Abort();
                _workerThread = null;
            }
        }

        private void InvokeAlgoritm()
        {
            while (IsStarted)
            {
                //Do smth
            }
        }
    }
}

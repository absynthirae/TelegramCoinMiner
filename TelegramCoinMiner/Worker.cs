using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramCoinMiner
{
    public class Worker
    {
        public bool IsWorks { get => _tokenSource.Token.IsCancellationRequested; }

        private Task _workerThread;
        private CancellationTokenSource _tokenSource;

        public void Start(Action workerJob)
        {
            if (_tokenSource != null)
            {
                Stop();
            }

            _tokenSource = new CancellationTokenSource();
            InitializeWorkerThread(workerJob);
            _workerThread.Start();
        }

        private void InitializeWorkerThread(Action workerJob)
        {
            _workerThread = new Task(() =>
            {
                while (!_tokenSource.Token.IsCancellationRequested)
                {
                    workerJob();
                }
            });
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            _workerThread.Wait();
            _tokenSource.Dispose();
            _workerThread.Dispose();
            _tokenSource = null;
            _workerThread = null;
        }
    }
}

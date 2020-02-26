using System.Collections.Generic;
using TLSharp.Core;

namespace TelegramCoinMiner
{
    public class ClickBotWorkerPool
    {
        public List<Worker> ClickBotWorkers { get ; set; }

        public ClickBotWorkerPool(List<TelegramClient> telegramClients)
        {

        }
    }
}

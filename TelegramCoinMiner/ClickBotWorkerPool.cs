using System.Collections.Generic;
using TelegramCoinMiner.Commands;
using TelegramCoinMiner.Commands.Params;

namespace TelegramCoinMiner
{
    public class ClickBotWorkerPool
    {
        public List<Worker> ClickBotWorkers { get ; set; }

        public ClickBotWorkerPool(List<LaunchClickBotParams> clickBotsParams)
        {
            foreach (var item in clickBotsParams)
            {
                ClickBotWorkers.Add(new Worker(async () => await new LaunchClickBotCommand(item).Execute(), item.TokenSource));
            }
        }

        public void Start()
        {
            foreach (var worker in ClickBotWorkers)
            {
                worker.Start();
            }
        }

        public void Stop()
        {
            foreach (var worker in ClickBotWorkers)
            {
                worker.Stop();
            }
        }
    }
}

using System.Collections.Generic;
using TelegramCoinMiner.Commands;
using TelegramCoinMiner.Commands.Params;

namespace TelegramCoinMiner
{
    public class ClickBotWorkerPool
    {
        public List<Worker> ClickBotWorkers { get ; set; }

        public ClickBotWorkerPool()
        {
            ClickBotWorkers = new List<Worker>();
        }

        public ClickBotWorkerPool(List<LaunchClickBotParams> clickBotsParams)
        {
            ClickBotWorkers = new List<Worker>();
            foreach (var item in clickBotsParams)
            {
                var launchCommand = new LaunchClickBotCommand(item);
                ClickBotWorkers.Add(new Worker(() => launchCommand.Execute().Wait(), item.TokenSource));
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

        public void Add(LaunchClickBotParams clickBotParams)
        {
            var launchCommand = new LaunchClickBotCommand(clickBotParams);
            ClickBotWorkers.Add(new Worker(() => launchCommand.Execute().Wait(), clickBotParams.TokenSource));
        }
    }
}

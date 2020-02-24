using System;
using System.Linq;
using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;
using TelegramCoinMiner.Extensions;
using TeleSharp.TL;

namespace TelegramCoinMiner.Commands
{
    public class WaitForTheEndOfAdCommand : IAsyncCommand
    {
        WaitForTheEndOfAdParams Params;
        public WaitForTheEndOfAdCommand(WaitForTheEndOfAdParams commandParams)
        {
            Params = commandParams;
        }
        public async Task Execute()
        {
            int time = await GetTaskWaitTimeInSeconds();
            Console.WriteLine("Время ожидания: " + time);
            await Task.Delay(time * 1000 + 1000);
        }

        private async Task<int> GetTaskWaitTimeInSeconds()
        {
            //Wait message about task wait time
            await Task.Delay(1500);
            //Default time
            int time = 15;
            (await Params.TelegramClient.GetMessages(Params.BotChannel, Params.BotInfo.ReadMessagesCount))
                .OfType<TLMessage>()
                .Where(x => x.Message.Contains("seconds"))
                .FirstOrDefault()
                .Message
                .Split(new char[] { ' ' })
                .FirstOrDefault(x => int.TryParse(x, out time));
            return time;
        }
    }
}

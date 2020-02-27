using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;
using TelegramCoinMiner.Extensions;

namespace TelegramCoinMiner.Commands
{
    public class SendStartCommand : IAsyncCommand
    {
        public SendStartParams Params { get; set; }

        public SendStartCommand(SendStartParams commandParams)
        {
            Params = commandParams;
        }

        public async Task Execute()
        {
            await Params.TelegramClient.SendMessageAsync(Params.Channel, "/start");
            
        }
    }
}

using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;
using TelegramCoinMiner.Extensions;

namespace TelegramCoinMiner.Commands
{
    public class SendSkipCommand : IAsyncCommand
    {
        public SendSkipParams Params { get; set; }

        public SendSkipCommand(SendSkipParams commandParams)
        {
            Params = commandParams;
        }

        public async Task Execute()
        {
            await Params.TelegramClient.SendMessageAsync(Params.Channel, "/start");
        }
    }
}

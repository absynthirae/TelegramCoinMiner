using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;

namespace TelegramCoinMiner.Commands
{
    public class SendVisitCommand : IAsyncCommand
    {
        public SendVisitParams Params { get; set; }

        public SendVisitCommand(SendVisitParams sendVisitParams)
        {
            Params = sendVisitParams;
        }

        public async Task Execute()
        {
          await Params.TelegramClient.SendMessageAsync(Params.Channel, "/visit");
        }
    }
}

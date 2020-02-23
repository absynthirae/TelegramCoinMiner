using TelegramCoinMiner.Commands.Params;

namespace TelegramCoinMiner.Commands
{
    public class SendVisitCommand : ICommand
    {
        public SendVisitParams Params { get; set; }

        public SendVisitCommand(SendVisitParams sendVisitParams)
        {
            Params = sendVisitParams;
        }

        public void Execute()
        {
            Params.TelegramClient.SendMessageAsync(Params.Channel, "/visit").Wait();
        }
    }
}

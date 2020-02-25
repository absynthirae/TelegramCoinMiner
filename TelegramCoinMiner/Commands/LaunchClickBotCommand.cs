using System.Linq;
using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;
using TeleSharp.TL;
using TeleSharp.TL.Contacts;

namespace TelegramCoinMiner.Commands
{
    class LaunchClickBotCommand : IAsyncCommand
    {
        public LaunchClickBotParams Params { get; set; }
        private ClickBotSwitcher _botSwitcher;

        public LaunchClickBotCommand(LaunchClickBotParams commandParams)
        {
            Params = commandParams;
            _botSwitcher = new ClickBotSwitcher();
        }

        public async Task Execute()
        {
            var currentBotInfo = _botSwitcher.CurrentBotInfo;
            TLFound foundChannels = await Params.TelegramClient.SearchUserAsync(currentBotInfo.BotName);
            var currentChannel = foundChannels.Users.OfType<TLUser>().FirstOrDefault(x => x.Username == currentBotInfo.BotName && x.FirstName == currentBotInfo.Title);

            IAsyncCommand sendVisitCommand = new SendVisitCommand(new SendVisitParams
                {
                    Channel = currentChannel,
                    TelegramClient = Params.TelegramClient
                });
            await sendVisitCommand.Execute();

            while (Params.IsWorks)
            {
                IAsyncCommand getUrlAndWatchAdCommand = new WatchAdCommand(
                    new WatchAdParams 
                    {
                        BotChannel = currentChannel,
                        TelegramClient = Params.TelegramClient,
                        Browser = Params.Browser
                    });
                await getUrlAndWatchAdCommand.Execute();

                IAsyncCommand waitForTheEndOfAdCommand = new WaitForTheEndOfAdCommand(
                    new WaitForTheEndOfAdParams
                    {
                        Channel = currentChannel,
                        TelegramClient = Params.TelegramClient
                    });
                await waitForTheEndOfAdCommand.Execute();
            }
        }
    }
}

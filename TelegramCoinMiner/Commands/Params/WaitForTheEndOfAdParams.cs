using TeleSharp.TL;
using TLSharp.Core;

namespace TelegramCoinMiner.Commands.Params
{
    public class WaitForTheEndOfAdParams
    {
        public TelegramClient TelegramClient { get; set; }
        public TLUser BotChannel { get; set; }
        public CoinClickBotInfo BotInfo { get; set; }
    }
}

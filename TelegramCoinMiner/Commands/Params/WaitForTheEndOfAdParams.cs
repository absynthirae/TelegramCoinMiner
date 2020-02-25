using TeleSharp.TL;
using TLSharp.Core;

namespace TelegramCoinMiner.Commands.Params
{
    public class WaitForTheEndOfAdParams
    {
        public TelegramClient TelegramClient { get; set; }
        public TLUser Channel { get; set; }
        public ClickBotInfo BotInfo { get; set; }
    }
}

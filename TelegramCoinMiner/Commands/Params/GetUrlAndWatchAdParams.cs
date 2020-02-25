using CefSharp.OffScreen;
using TeleSharp.TL;
using TLSharp.Core;

namespace TelegramCoinMiner.Commands.Params
{
    public class GetUrlAndWatchAdParams
    {
        public TelegramClient TelegramClient { get; set; }
        public TLUser BotChannel { get; set; }
        public CoinClickBotInfo BotInfo { get; set; }
        public ChromiumWebBrowser Browser { get; set; }
    }
}

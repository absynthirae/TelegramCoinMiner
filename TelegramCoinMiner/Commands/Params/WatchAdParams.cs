using CefSharp.OffScreen;
using TeleSharp.TL;
using TLSharp.Core;

namespace TelegramCoinMiner.Commands.Params
{
    public class WatchAdParams
    {
        public TelegramClient TelegramClient { get; set; }
        public TLUser Channel { get; set; }
        public TLMessage AdMessage { get; set; }
        public ChromiumWebBrowser Browser { get; set; }
    }
}

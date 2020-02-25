using CefSharp.OffScreen;
using TLSharp.Core;

namespace TelegramCoinMiner.Commands.Params
{
    public class LaunchClickBotParams
    {
        public TelegramClient TelegramClient { get; set; }
        public ChromiumWebBrowser Browser { get; set; }
        public bool IsWorks { get; set; }
    }
}

using CefSharp.OffScreen;
using System.Threading;
using TLSharp.Core;

namespace TelegramCoinMiner.Commands.Params
{
    public class LaunchClickBotParams
    {
        public TelegramClient TelegramClient { get; set; }
        public ChromiumWebBrowser Browser { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
    }
}

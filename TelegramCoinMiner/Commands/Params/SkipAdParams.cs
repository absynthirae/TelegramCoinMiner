using TeleSharp.TL;
using TLSharp.Core;

namespace TelegramCoinMiner.Commands.Params
{
    public class SkipAdParams
    {
        public TelegramClient TelegramClient { get; set; }
        public TLMessage AdMessage { get; set; }
        public TLUser Channel { get; set; }
    }
}

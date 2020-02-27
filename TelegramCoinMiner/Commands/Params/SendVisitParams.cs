using TeleSharp.TL;
using TLSharp.Core;

namespace TelegramCoinMiner.Commands.Params
{
    public class SendVisitParams
    {
        public TelegramClient TelegramClient { get; set; }
        public TLUser Channel { get; set; }
    }
}

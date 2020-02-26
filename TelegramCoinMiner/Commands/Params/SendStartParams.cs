using TeleSharp.TL;
using TLSharp.Core;

namespace TelegramCoinMiner.Commands.Params
{
    public class SendStartParams
    {
        public TelegramClient TelegramClient { get; set; }
        public TLUser Channel { get; set; }
    }
}

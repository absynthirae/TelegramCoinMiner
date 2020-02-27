using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;
using TelegramCoinMiner.Extensions;
using TeleSharp.TL;
using TeleSharp.TL.Messages;

namespace TelegramCoinMiner.Commands
{
    public class SkipAdCommand : IAsyncCommand
    {
        public SkipAdParams Params { get; set; }

        public SkipAdCommand(SkipAdParams commandParams)
        {
            Params = commandParams;
        }

        public async Task Execute()
        {
            var skipCallbackButton = Params.AdMessage.GetButtonWithCallBack("Skip");
            var data = skipCallbackButton.Data;
            var messageId = Params.AdMessage.Id;
            await Params.TelegramClient.SendRequestAsync<object>(
                new TLRequestGetBotCallbackAnswer()
                {
                    Peer = new TLInputPeerUser() { UserId = Params.Channel.Id, AccessHash = Params.Channel.AccessHash.Value },
                    Data = data,
                    MsgId = messageId
                });
        }
    }
}

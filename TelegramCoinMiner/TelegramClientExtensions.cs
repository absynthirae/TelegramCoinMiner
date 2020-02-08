using System.Threading.Tasks;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace TelegramCoinMiner
{
    static class TelegramClientExtensions
    {
        public async static Task<TLMessagesSlice> GetMessages(this TelegramClient client, long access_hash, int user_id)
        {
            return await client.SendRequestAsync<TLMessagesSlice>(new TLRequestGetHistory()
            {
                Peer = new TeleSharp.TL.TLInputPeerUser { AccessHash = access_hash, UserId = user_id },
                Limit = 5,
                AddOffset = 0,
                OffsetId = 0
            });
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLSharp.Core;

namespace TelegramCoinMiner
{
    static class TelegramClientFactory
    {
        public static List<TelegramClient> TelegramClients { get; set; } = new List<TelegramClient>();

        public static async Task<TelegramClient> CreateTelegramClientAsync(string phoneNumber)
        {
            var client = TelegramClients.FirstOrDefault(x => x.Session.TLUser.Phone == phoneNumber);

            if (client != null)
            {
                return client;
            }

            int apiId = 907056;
            string apiHash = "697525d840e31523f43a972dff47e16a";

            client = new TelegramClient(apiId, apiHash, sessionUserId: phoneNumber);
            await client.ConnectAsync();
            if (!client.IsUserAuthorized())
            {
                throw new System.Exception("Session not found");
            }
            TelegramClients.Add(client);
            return client;
        }
    }
}

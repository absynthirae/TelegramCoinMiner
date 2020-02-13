using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace TelegramCoinMiner
{
    static class TLSharpExtensions
    {
        public async static Task<TLMessagesSlice> GetMessages(this TelegramClient client, long accessHash, int userId, int count)
        {
            return await client.SendRequestAsync<TLMessagesSlice>(new TLRequestGetHistory()
            {
                Peer = new TLInputPeerUser { AccessHash = accessHash, UserId = userId },
                Limit = count,
                AddOffset = 0,
                OffsetId = 0
            });
        }

        public static TLKeyboardButtonUrl GetButtonWithUrl(this IEnumerable<TLMessage> messages, string keyText)
        {
            var keyboardLinesButtons = messages
                            .Select(x => x.ReplyMarkup) //берем разметку сообщений
                            .OfType<TLReplyInlineMarkup>() //берем тип, содержащий кнопки
                            .Select(x => x.Rows.Select(row => row.Buttons)); //берем все кнопки из строк

            //преобразовываем список списков в одномерный список
            var absButtons = new List<TLAbsKeyboardButton>();
            foreach (var buttonsLine in keyboardLinesButtons)
            {
                foreach (var buttons in buttonsLine)
                {
                    absButtons.AddRange(buttons);
                }
            }

            //ищем кнопку для перехода по ссылке
            var goToWebsiteButton = absButtons
                .OfType<TLKeyboardButtonUrl>()
                .FirstOrDefault(x => x.Text.ToLower().Contains(keyText.ToLower()));
            
            return goToWebsiteButton;
        }
    }
}

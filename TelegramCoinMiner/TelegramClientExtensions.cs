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
        public async static Task<TLVector<TLAbsMessage>> GetMessages(this TelegramClient client, long accessHash, int userId, int count)
        {
            var messagesSlice = await client.SendRequestAsync<TLMessagesSlice>(new TLRequestGetHistory()
            {
                Peer = new TLInputPeerUser { AccessHash = accessHash, UserId = userId },
                Limit = count,
                AddOffset = 0,
                OffsetId = 0
            });

            return messagesSlice.Messages;
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

        public static async Task<TLUser> GetChannelByName(this TelegramClient client, string channelName)
        {
            var dialogs = (TLDialogs)await client.GetUserDialogsAsync(); //получаем все диалоги

            var channel = dialogs.Users
                .OfType<TLUser>()
                .FirstOrDefault(c => c.FirstName.ToLower() == channelName.ToLower()); //берем нужный нам канал(юзер потому что бот)
            
            return channel;
        }

        public static bool HasCaptcha(this string html) {
            // чисто примитив
            if (html.ToLower().Contains("captcha"))
            {
                return true; 
            }

            return false;
        }
    }
}

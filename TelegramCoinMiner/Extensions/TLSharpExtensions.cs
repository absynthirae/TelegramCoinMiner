﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace TelegramCoinMiner.Extensions
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

        public async static Task<TLVector<TLAbsMessage>> GetMessages(this TelegramClient client, TLUser channel, int count)
        {
            return await client.GetMessages(channel.AccessHash.Value, channel.Id, count);
        }

        public async static Task<TLAbsUpdates> SendMessageAsync(this TelegramClient client, TLUser channel, string message)
        {
            return await client.SendMessageAsync(new TLInputPeerUser { UserId = channel.Id, AccessHash = channel.AccessHash.Value }, message);
        }

        public static TLKeyboardButtonUrl GetButtonWithUrl(this IEnumerable<TLMessage> messages, string searchText)
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
            var button = absButtons
                .OfType<TLKeyboardButtonUrl>()
                .FirstOrDefault(x => x.Text.ToLower().Contains(searchText.ToLower()));
            
            return button;
        }

        public static TLKeyboardButtonUrl GetButtonWithUrl(this TLMessage message, string searchText)
        {
            var markup = message.ReplyMarkup as TLReplyInlineMarkup;
            
            if (markup == null)
                return null;

            var keyboardLinesButtons = markup.Rows.Select(row => row.Buttons); //берем все кнопки из строк

            //преобразовываем список списков в одномерный список
            var absButtons = new List<TLAbsKeyboardButton>();
            foreach (var buttons in keyboardLinesButtons)
            {
                absButtons.AddRange(buttons);
            }
            
            //ищем кнопку для перехода по ссылке
            var button = absButtons
                .OfType<TLKeyboardButtonUrl>()
                .FirstOrDefault(x => x.Text.ToLower().Contains(searchText.ToLower()));

            return button;
        }

        public static TLKeyboardButtonCallback GetButtonWithCallBack(this IEnumerable<TLMessage> messages, string searchText)
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
            var button = absButtons
                .OfType<TLKeyboardButtonCallback>()
                .FirstOrDefault(x => x.Text.ToLower().Contains(searchText.ToLower()));

            return button;
        }

        public static TLKeyboardButtonCallback GetButtonWithCallBack(this TLMessage message, string searchText)
        {
            var markup = message.ReplyMarkup;
            var inlineMarkup = markup as TLReplyInlineMarkup;

            var keyboardLinesButtons = inlineMarkup.Rows.Select(row => row.Buttons); //берем все кнопки из строк

            //преобразовываем список списков в одномерный список
            var absButtons = new List<TLAbsKeyboardButton>();
            foreach (var buttons in keyboardLinesButtons)
            {
                    absButtons.AddRange(buttons);
            }

            //ищем кнопку для перехода по ссылке
            var button = absButtons
                .OfType<TLKeyboardButtonCallback>()
                .FirstOrDefault(x => x.Text.ToLower().Contains(searchText.ToLower()));

            return button;
        }


        public static async Task<TLUser> GetChannelByName(this TelegramClient client, string channelName)
        {
            var dialogs = (TLDialogs)await client.GetUserDialogsAsync(); //получаем все диалоги

            var channel = dialogs.Users
                .OfType<TLUser>()
                .FirstOrDefault(c => c.FirstName.ToLower() == channelName.ToLower()); //берем нужный нам канал(юзер потому что бот)
            
            return channel;
        }
    }
}

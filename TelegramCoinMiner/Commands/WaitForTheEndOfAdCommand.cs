using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;
using TelegramCoinMiner.Extensions;
using TeleSharp.TL;

namespace TelegramCoinMiner.Commands
{
    public class WaitForTheEndOfAdCommand : IAsyncCommand
    {
        WaitForTheEndOfAdParams Params;

        public WaitForTheEndOfAdCommand(WaitForTheEndOfAdParams commandParams)
        {
            Params = commandParams;
        }

        public async Task Execute()
        {
            int time = await GetTaskWaitTimeInSeconds();
            Console.WriteLine("Время ожидания: " + time);
            var start = DateTime.Now;
            await Task.Delay(time * 1000 + 3000);
            var end = DateTime.Now;
            Console.WriteLine("Прошло: " + (end - start).ToString());
        }

        private async Task<int> GetTaskWaitTimeInSeconds()
        {
            //Wait message about task wait time
            await Task.Delay(2000);
            //Default time
            int time = 15;
            try
            {
                var messages = (await Params.TelegramClient.GetMessages(Params.Channel, Constants.ReadMessagesCount)).OfType<TLMessage>();

                //var earnedMessage = messages.FirstOrDefault(x => x.Message.StartsWith("You earned") && x.Message.EndsWith("for visiting a site!"));

                //if (earnedMessage != null)
                //{
                //    Console.WriteLine(earnedMessage.Message);
                //    return 5;
                //}

                messages.Where(x => x.Message.Contains("seconds"))
                    .FirstOrDefault()
                    .Message
                    .Split(new char[] { ' ' })
                    .FirstOrDefault(x => int.TryParse(x, out time));
            }
            catch (Exception) 
            {
                Console.WriteLine("Проблема со временем");
            }
            return time;
        }
    }
}

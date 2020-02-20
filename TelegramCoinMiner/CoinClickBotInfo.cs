using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCoinMiner
{
    public class CoinClickBotInfo
    {
        public string BotName { get; set; }
        public int ReadMessagesCount { get; set; }
     
        public static CoinClickBotInfo CreateBitcoinClickBotInfo()
        {
            return new CoinClickBotInfo { BotName = "BTC Click Bot" , ReadMessagesCount=2};
        }

        
    }
}

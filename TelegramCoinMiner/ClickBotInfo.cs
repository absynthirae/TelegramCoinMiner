namespace TelegramCoinMiner
{
    public class ClickBotInfo
    {
        public string BotName { get; set; }
        public int ReadMessagesCount { get; set; }

        public static ClickBotInfo CreateBitcoinClickBotInfo()
        {
            return new ClickBotInfo { BotName = "BTC Click Bot" , ReadMessagesCount = 2 };
            
        }
        
        public static ClickBotInfo CreateBitcoinCashClickBotInfo()
        {
            return new ClickBotInfo { BotName = "BCH Click Bot", ReadMessagesCount = 2 };
        }

        public static ClickBotInfo CreateDogecoinClickBotInfo()
        {
            return new ClickBotInfo { BotName = "DOGE Click Bot", ReadMessagesCount = 2 };
        }

        public static ClickBotInfo CreateZcashClickBotInfo()
        {
            return new ClickBotInfo { BotName = "ZEC Click Bot", ReadMessagesCount = 2 };
        }

        public static ClickBotInfo CreateLitecoinClickBotInfo()
        {
            return new ClickBotInfo { BotName = "LTC Click Bot", ReadMessagesCount = 2 };
        }
    }
}

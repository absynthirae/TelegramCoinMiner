namespace TelegramCoinMiner
{
    public class CoinClickBotInfo
    {
        public string BotName { get; set; }
        public int ReadMessagesCount { get; set; }

        public static CoinClickBotInfo CreateBitcoinClickBotInfo()
        {
            return new CoinClickBotInfo { BotName = "BTC Click Bot" , ReadMessagesCount = 2 };
        }
        
        public static CoinClickBotInfo CreateBitcoinCashClickBotInfo()
        {
            return new CoinClickBotInfo { BotName = "BCH Click Bot", ReadMessagesCount = 2 };
        }

        public static CoinClickBotInfo CreateDogecoinClickBotInfo()
        {
            return new CoinClickBotInfo { BotName = "DOGE Click Bot", ReadMessagesCount = 2 };
        }

        public static CoinClickBotInfo CreateZcashClickBotInfo()
        {
            return new CoinClickBotInfo { BotName = "ZEC Click Bot", ReadMessagesCount = 2 };
        }

        public static CoinClickBotInfo CreateLitecoinClickBotInfo()
        {
            return new CoinClickBotInfo { BotName = "LTC Click Bot", ReadMessagesCount = 2 };
        }
    }
}

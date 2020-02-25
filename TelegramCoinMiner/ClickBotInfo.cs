namespace TelegramCoinMiner
{
    public class ClickBotInfo
    {
        public string BotName { get; set; }
        public int ReadMessagesCount { get; set; }

        public string Title { get; set; }
        public static ClickBotInfo CreateBitcoinClickBotInfo()
        {
            return new ClickBotInfo 
            { 
                BotName = Constants.nameBitcoin, 
                ReadMessagesCount = Constants.readMessageCount, 
                Title = Constants.titleBitcoin 
            };
            
        }
        
        public static ClickBotInfo CreateBitcoinCashClickBotInfo()
        {
            return new ClickBotInfo 
            { 
                BotName = Constants.titleBchcoin, 
                ReadMessagesCount = Constants.readMessageCount, 
                Title= Constants.titleBchcoin 
            };
        }

        public static ClickBotInfo CreateDogecoinClickBotInfo()
        {
            return new ClickBotInfo 
            {
                BotName = Constants.titleDogecoin, 
                ReadMessagesCount = Constants.readMessageCount, 
                Title = Constants.titleDogecoin
            };
        }

        public static ClickBotInfo CreateZcashClickBotInfo()
        {
            return new ClickBotInfo 
            { 
                BotName = Constants.nameZcushcoin, 
                ReadMessagesCount = Constants.readMessageCount, 
                Title = Constants.titleZcushcoin 
            };
        }

        public static ClickBotInfo CreateLitecoinClickBotInfo()
        {
            return new ClickBotInfo 
            { 
                BotName = Constants.nameLitecoin,
                ReadMessagesCount = Constants.readMessageCount,
                Title = Constants.titleLitecoin 
            };
        }
    }
}

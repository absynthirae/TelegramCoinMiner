namespace TelegramCoinMiner
{
    public class ClickBotInfo
    {
        public string BotName { get; set; }
        public string Title { get; set; }

        public static ClickBotInfo CreateBitcoinClickBotInfo()
        {
            return new ClickBotInfo 
            { 
                BotName = Constants.NameBitcoin,
                Title = Constants.TitleBitcoin 
            };
            
        }
        
        public static ClickBotInfo CreateBitcoinCashClickBotInfo()
        {
            return new ClickBotInfo 
            { 
                BotName = Constants.NameBch, 
                Title= Constants.TitleBch 
            };
        }

        public static ClickBotInfo CreateDogecoinClickBotInfo()
        {
            return new ClickBotInfo 
            {
                BotName = Constants.NameDogecoin, 
                Title = Constants.TitleDogecoin
            };
        }

        public static ClickBotInfo CreateZcashClickBotInfo()
        {
            return new ClickBotInfo 
            { 
                BotName = Constants.NameZcash, 
                Title = Constants.TitleZcash 
            };
        }

        public static ClickBotInfo CreateLitecoinClickBotInfo()
        {
            return new ClickBotInfo 
            { 
                BotName = Constants.NameLitecoin,
                Title = Constants.TitleLitecoin 
            };
        }
    }
}

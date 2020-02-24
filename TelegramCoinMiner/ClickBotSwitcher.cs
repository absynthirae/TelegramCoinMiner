using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCoinMiner
{
    class ClickBotSwitcher
    {
        List<CoinClickBotInfo> listOfBots = new List<CoinClickBotInfo>()
        {
         CoinClickBotInfo.CreateDogecoinClickBotInfo(),
         CoinClickBotInfo.CreateZcashClickBotInfo(),
         CoinClickBotInfo.CreateLitecoinClickBotInfo()
        };

        int _currentNumber = 0;
        public CoinClickBotInfo GetNext()
        {
            _currentNumber += (_currentNumber + 1) % listOfBots.Count;
            return listOfBots[_currentNumber];
        }
        public void AddBot(CoinClickBotInfo cn)
        {
            listOfBots.Add(cn);
        }

        public void DeleteBot(CoinClickBotInfo cn)
        {
            listOfBots.Remove(cn);
        }


    }
}

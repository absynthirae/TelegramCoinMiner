using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCoinMiner
{
    class ClickBotSwitcher
    {
        List<ClickBotInfo> listOfBots = new List<ClickBotInfo>()
        {
         ClickBotInfo.CreateDogecoinClickBotInfo(),
         ClickBotInfo.CreateZcashClickBotInfo(),
         ClickBotInfo.CreateLitecoinClickBotInfo()
        };

        int _currentNumber = 0;
        public ClickBotInfo GetNext()
        {
            _currentNumber += (_currentNumber + 1) % listOfBots.Count;
            return listOfBots[_currentNumber];
        }
        public void AddBot(ClickBotInfo cn)
        {
            listOfBots.Add(cn);
        }

        public void DeleteBot(ClickBotInfo cn)
        {
            listOfBots.Remove(cn);
        }


    }
}

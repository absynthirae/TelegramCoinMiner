using System.Collections.Generic;

namespace TelegramCoinMiner
{
    class ClickBotSwitcher
    {
        private int _currentIndex = 0;
        private List<ClickBotInfo> _listOfBots;
        public ClickBotInfo CurrentBotInfo => _listOfBots[_currentIndex];

        public ClickBotSwitcher()
        {
            _listOfBots = new List<ClickBotInfo>()
            {
                ClickBotInfo.CreateDogecoinClickBotInfo(),
                ClickBotInfo.CreateBitcoinClickBotInfo()
            };
        }

        public ClickBotSwitcher(List<ClickBotInfo> clickBotInfos)
        {
            _listOfBots = clickBotInfos;
        }

        public ClickBotInfo Next()
        {
            _currentIndex = (_currentIndex + 1) % _listOfBots.Count;
            return _listOfBots[_currentIndex];
        }

        public void AddBot(ClickBotInfo cn)
        {
            _listOfBots.Add(cn);
        }

        public void DeleteBot(ClickBotInfo cn)
        {
            _listOfBots.Remove(cn);
        }
    }
}

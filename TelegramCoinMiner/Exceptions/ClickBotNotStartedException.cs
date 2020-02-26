using System;

namespace TelegramCoinMiner.Exceptions
{
    class ClickBotNotStartedException : Exception
    {
        public ClickBotNotStartedException()
        {
        }

        public ClickBotNotStartedException(string message) : base(message)
        {
        }
    }
}

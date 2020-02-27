using System;

namespace TelegramCoinMiner.Exceptions
{
    class CapchaException : Exception
    {
        public override string Message => "Capcha is here";

        public override string ToString()
        {
            return Message;
        }

    }
}

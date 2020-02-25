﻿using System;

namespace TelegramCoinMiner.Exceptions
{
    class MessageNotFoundException : Exception
    {
        public override string Message => "Not found Messages";

        public override string ToString()
        {
            return Message;
        }
    }
}

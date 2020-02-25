using System;
using System.Runtime.Serialization;

namespace TelegramCoinMiner.Exceptions
{
    class BrowserTimeoutException : Exception
    {
        public override string Message => "Browser Timeout done";

        public override string ToString() 
        {
            return Message;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace TelegramCoinMiner.Exceptions
{
    class TimeOutException : Exception
    {
        public override string Message => "Browser TimeOut done";

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

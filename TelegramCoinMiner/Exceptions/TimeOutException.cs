using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

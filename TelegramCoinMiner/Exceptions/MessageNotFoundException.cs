using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

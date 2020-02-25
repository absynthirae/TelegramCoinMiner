using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCoinMiner.Exceptions
{
    class CapchaException :Exception
    {

        public override string Message => "Capcha is here";
        public override string ToString()
        {

            return Message;
        }

    }
}

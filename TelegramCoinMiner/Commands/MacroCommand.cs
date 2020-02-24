using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCoinMiner.Commands
{
    public class MacroCommand : IAsyncCommand
    {
        private List<IAsyncCommand> commands;

        public MacroCommand(List<IAsyncCommand> commands)
        {
            this.commands = commands;
        }

        public void Execute()
        {
            foreach (IAsyncCommand c in commands)
                c.Execute();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCoinMiner.Commands
{
    public class MacroCommand : ICommand
    {
        private List<ICommand> commands;

        public MacroCommand(List<ICommand> commands)
        {
            this.commands = commands;
        }

        public void Execute()
        {
            foreach (ICommand c in commands)
                c.Execute();
        }
    }
}

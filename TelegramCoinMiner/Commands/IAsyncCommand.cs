﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCoinMiner.Commands
{
    public interface IAsyncCommand
    {
        Task Execute();
    }
}

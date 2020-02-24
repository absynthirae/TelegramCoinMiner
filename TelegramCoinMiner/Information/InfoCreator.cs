using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCoinMiner.Information
{
    static class InfoCreator
    {
        static int _current = 0;

        static List<Channel> _queue = new List<Channel>
        {
            new Channel(goWord: "go to website", skipWord: "/skip", visitWord: "/visit", messageCount:2),
            new Channel(goWord: "go to website", skipWord: "/skip", visitWord: "/visit", messageCount:2),
            new Channel(goWord: "go to website", skipWord: "/skip", visitWord: "/visit", messageCount:2),
            new Channel(goWord: "go to website", skipWord: "/skip", visitWord: "/visit", messageCount:2),
            new Channel(goWord: "go to website", skipWord: "/skip", visitWord: "/visit", messageCount:2)
        };

        static public Channel GetNextChannel()
        {

            _current += (_current + 1) % _queue.Count;
            return _queue[_current];


        }
        static public void AddChannel(Channel cn)
        {
            _queue.Add(cn);

        }

        static public void DeleteChannel(Channel cn)
        {
            _queue.Remove(cn);
        }

    }
}

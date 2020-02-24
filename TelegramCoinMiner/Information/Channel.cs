using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCoinMiner.Information
{
   public class Channel
    {
        string visitWord { get; set; }
        string gkipWord { get; set; }
        string goWord { get; set; }

        string botName
        public Channel() { 
        
        }
        public Channel(string VisitWord="/visit", string skipWord = "/skip", string goWord="go to website" )
        {
            this.visitWord = VisitWord;
            this.gkipWord = SkipWord;
            this.goWord = GoWord;
        }
  
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Customs.JsonQuests
{
    public class JsonQuestItem
    {
        public string Item { get; set; }
        public int Amount { get; set; }
        public Dictionary<string, object> Props { get; set; }
    }
}

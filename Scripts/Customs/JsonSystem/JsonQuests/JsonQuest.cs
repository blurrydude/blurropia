using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Server.ContextMenus;
using Server.Engines.Quests;

namespace Server.Customs.JsonSystem
{
    public class JsonQuest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<JsonQuestGiverConfig> Givers { get; set; }
    }
}

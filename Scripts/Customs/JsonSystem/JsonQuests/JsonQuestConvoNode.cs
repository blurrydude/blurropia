using System.Collections.Generic;

namespace Server.Customs.JsonQuests
{
    public class JsonQuestConvoNode
    {
        public int NodeId { get; set; }
        public string OptionText { get; set; }
        public List<int> Nodes { get; set; }
        public string Text { get; set; }
        public JsonQuestItem Item { get; set; }
        public JsonQuestItem TriggerItem { get; set; }
        public bool CompletesQuest { get; set; }
    }
}

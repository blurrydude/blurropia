using System.Collections.Generic;

namespace Server.Customs
{
    public class JsonQuestConvoNode
    {
        public JsonQuestConvoNode()
        {
            OptionText = "";
            Nodes = new List<int>();
            Text = "";
        }
        public int NodeId { get; set; }
        public string OptionText { get; set; }
        public List<int> Nodes { get; set; }
        public string Text { get; set; }
        public JsonItem Item { get; set; }
        public JsonItem TriggerItem { get; set; }
        public bool CompletesQuest { get; set; }
    }
}

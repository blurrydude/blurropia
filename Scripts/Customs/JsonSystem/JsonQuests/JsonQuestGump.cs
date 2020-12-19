using System;
using System.Linq;
using Server.Gumps;
using Server.Network;

namespace Server.Customs
{
    public class JsonQuestGump : Gump
    {
        private JsonQuestGiver _giver;
        public JsonQuestGump(JsonQuestGiver giver, int currentNode = 0) : base(50, 50)
        {
            _giver = giver;
            var convoNode = _giver.ConvoNodes.First(x => x.NodeId == currentNode);
            var outerWidth = 400;
            var outerHeight = 190 + convoNode.Nodes.Count * 30;
            AddPage(0);

            AddBackground(0, 0, outerWidth, outerHeight, PropsConfig.BackGumpID);
            AddHtml(20,20,360,150,convoNode.Text,true,true);

            var i = 0;

            foreach (var n in convoNode.Nodes)
            {
                var node = _giver.ConvoNodes.First(x => x.NodeId == n);
                AddButton(30,i*30 + 190,1896,1895,n,GumpButtonType.Reply,0);
                AddLabel(60,i*30 + 190,0,node.OptionText);
                i++;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                return;
            }
            var node = _giver.ConvoNodes.FirstOrDefault(x => x.NodeId == info.ButtonID);
            if (node == null)
            {
                _giver.PublicOverheadMessage(MessageType.Regular, 0x35, false, "There is something wrong with my configuration. Please let an admin know.");
                return;
            }

            if (node.Item == null)
            {
                sender.Mobile.CloseGump(typeof(JsonQuestGump));
                sender.Mobile.SendGump(new JsonQuestGump(_giver, node.NodeId));
                return;
            }

            JsonQuestHelper.CheckGiveItem(_giver, node, sender.Mobile);
            sender.Mobile.CloseGump(typeof(JsonQuestGump));
            sender.Mobile.SendGump(new JsonQuestGump(_giver, node.NodeId));
        }
    }
}

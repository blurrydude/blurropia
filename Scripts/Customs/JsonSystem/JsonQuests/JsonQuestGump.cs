using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Gumps;
using Server.Network;
using ServerUtilityExtensions;

namespace Server.Customs
{
    public class JsonQuestGumpConfig
    {
        public int Width { get; set; }
        public int Padding { get; set; }
        public int HtmlHeight { get; set; }
        public int LineHeight { get; set; }
        public int LineSpacing { get; set; }
        public int OptionLineCharLimit { get; set; }
        public int OptionMarginLeft { get; set; }
        public int OptionTextMarginLeft { get; set; }
        public int ButtonNormal { get; set; }
        public int ButtonPressed { get; set; }
        public int OptionTextHue { get; set; }
        public int Background { get;set; }
    }

    public class JsonQuestGump : Gump
    {
        private JsonQuestGiver _giver;
        public JsonQuestGump(JsonQuestGiver giver, int currentNode = 0) : base(50, 50)
        {
            var json = File.ReadAllText("Scripts/Customs/JsonSystem/Data/JsonQuestGumpConfig.json");
            var config = (JsonQuestGumpConfig) JsonUtility.Deserialize<JsonQuestGumpConfig>(json);
            _giver = giver;
            var convoNode = _giver.ConvoNodes.First(x => x.NodeId == currentNode);
            var outerWidth = config.Width;
            var outerHeight = config.HtmlHeight + config.Padding*2;

            var y = 0;
            var buttons = new List<object[]>();
            var labels = new List<object[]>();
            foreach (var n in convoNode.Nodes)
            {
                var node = _giver.ConvoNodes.First(x => x.NodeId == n);
                var lines = GetWrapped(node.OptionText,config.OptionLineCharLimit);
                buttons.Add(new object[] {config.Padding+config.OptionMarginLeft,y + config.HtmlHeight + config.Padding*2,config.ButtonNormal,config.ButtonPressed,n,GumpButtonType.Reply,0});
                foreach (var line in lines)
                {
                    labels.Add(new object[] {config.Padding+config.OptionMarginLeft+config.OptionTextMarginLeft, y + config.HtmlHeight + config.Padding*2, config.OptionTextHue, line});
                    y += config.LineHeight;
                    outerHeight += config.LineHeight;
                }

                y += config.LineSpacing;
                outerHeight += config.LineSpacing;
            }

            outerHeight += config.Padding - config.LineSpacing;
            AddPage(0);

            AddBackground(0, 0, outerWidth, outerHeight, config.Background);
            AddHtml(config.Padding,config.Padding,config.Width-config.Padding*2,config.HtmlHeight,convoNode.Text,true,true);
            foreach (var button in buttons)
            {
                AddButton((int)button[0],(int)button[1],(int)button[2],(int)button[3],(int)button[4],(GumpButtonType)button[5],(int)button[6]);
            }

            foreach (var label in labels)
            {
                AddLabel((int)label[0],(int)label[1],(int)label[2],(string)label[3]);
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

        public static string[] GetWrapped(string original, int linecharlimit) {
            var count = 0;
            var lastspace = -1;
            var output = new List<string>();
            while (original.Length > 0)
            {
                if (original[count] == ' ') lastspace = count;
                count++;
                if (count > linecharlimit)
                {
                    count = 0;
                    var str = original.Substring(0, lastspace + 1);
                    output.Add(str);
                    original = original.Substring(lastspace+1);
                }
                if(count == original.Length - 1) {
                    output.Add(original);
                    original = String.Empty;
                }
            }
            return output.ToArray();
        }
    }
}

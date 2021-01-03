using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Customs
{
    public class JsonQuestControlCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("questcontrol", AccessLevel.GameMaster, new CommandEventHandler(JsonQuestCommand_OnCommand));
        }

        [Usage("jsonquestload")]
        [Description("Json System Quest Control")]
        public static void JsonQuestCommand_OnCommand(CommandEventArgs arg)
        {
            arg.Mobile.SendGump(new JsonQuestControlGump(new JsonQuestControlGumpState()));
        }
    }

    public class JsonQuestControlGumpState
    {
        public JsonQuestControlGumpState()
        {
            JsonQuests = new Dictionary<string, JsonQuest>();
            var files = Directory.GetFiles("Scripts/Customs/JsonSystem/JsonQuests/Data", "*.json");
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var jQuest = JsonConvert.DeserializeObject<JsonQuest>(json);
                JsonQuests.Add(file.Split('\\').Last().Split('.').First(),jQuest);
            }
        }
        public Dictionary<string, JsonQuest> JsonQuests { get; set; }
        public string Quest { get; set; }
        public int? ConvoNode { get; set; }
        public int? Giver { get; set; }
        public bool WaitForResponse { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }

    public class JsonQuestControlGump : JsonGump
    {
        public JsonQuestControlGumpState State { get; set; }
        public JsonQuestControlGump(JsonQuestControlGumpState state) : base($@"{AppDomain.CurrentDomain.BaseDirectory}Scripts\Customs\JsonSystem\JsonQuests\Gumps\questcontrol.json")
        {
            State = state;
         
            if (State.JsonQuests == null) return;

            BuildControlButtons();
            BuildQuestList();
            BuildGiverList();
            BuildGiverDetails();
            BuildConvoNodeDetails();

            if (!String.IsNullOrEmpty(State.ErrorMessage)) BuildError();
            if (!String.IsNullOrEmpty(State.SuccessMessage)) BuildSuccess();
            if (State.WaitForResponse) BuildPrompt();
        }

        public void BuildSuccess()
        {
            AddLabel(160,11,64,State.SuccessMessage);
            State.ErrorMessage = string.Empty;
        }

        public void BuildError()
        {
            AddLabel(160,11,128,State.ErrorMessage);
            State.ErrorMessage = string.Empty;
        }

        public void BuildPrompt()
        {
            AddBackground(305,100,390,134,1755);
            AddLabel(323,118,192,"Please Enter A Name:");
            AddBackground(365,146,270,28,9350);
            AddTextEntry(373,150,250,20,0,1000,"newName");
            AddButton(435, 185, 40021, 40031, 3, GumpButtonType.Reply, 0);
            AddLabel(480, 188, 1024, "Save");
        }
        
        public void BuildGiverDetails()
        {
            if (State.Giver == null) return;
            var giver = State.JsonQuests[State.Quest].Givers[(int)State.Giver];
            AddLabel(18,448,192,"Name:");
            AddBackground(60,444,270,28,9350);
            AddTextEntry(68,448,250,20,0,0,giver.Name);
            AddLabel(18, 488, 192, "Title: ");
            AddBackground(60, 484, 270, 28, 9350);
            AddTextEntry(68, 488, 250, 20, 0, 1, giver.Title);
            AddCheck(18, 528, 210, 211, giver.RandomName, 5);
            AddLabel(43, 528, 192, "Auto Name");
            AddCheck(118, 528, 210, 211, giver.AutoDress, 4);
            AddLabel(143, 528, 192, "Auto Dress");
            AddCheck(228, 528, 210, 211, giver.CantWalk, 6);
            AddLabel(253, 528, 192, "Cant Walk");
            AddLabel(18, 568, 192, "Gender:");
            AddRadio(68, 568, 210, 211, giver.RandomGender, 1);
            AddLabel(98, 568, 192, "Auto");
            AddRadio(158, 568, 210, 211, !giver.RandomGender&&!giver.Female, 2);
            AddLabel(188, 568, 192, "Male");
            AddRadio(238, 568, 210, 211, !giver.RandomGender&&giver.Female, 3);
            AddLabel(268, 568, 192, "Female");
            AddLabel(18, 608, 192, "Map:");
            AddBackground(60, 604, 100, 28, 9350);
            AddTextEntry(68, 608, 100, 20, 0, 2, giver.StartMap);
            AddLabel(190, 608, 192, "Point:");
            AddBackground(235, 604, 100, 28, 9350);
            AddTextEntry(243, 608, 100, 20, 0, 3, String.Join(",",giver.StartLocation));
            AddLabel(18, 648, 192, "Body:");
            AddBackground(60, 644, 100, 28, 9350);
            AddTextEntry(68, 648, 100, 20, 0, 4, giver.Body.ToString());
            AddLabel(190, 648, 192, "Hue:");
            AddBackground(235, 644, 100, 28, 9350);
            AddTextEntry(243, 648, 100, 20, 0, 5, giver.Hue.ToString());
            AddLabel(18, 688, 192, "Hair:");
            AddBackground(60, 684, 100, 28, 9350);
            AddTextEntry(68, 688, 100, 20, 0, 6, giver.HairItemId.ToString());
            AddLabel(190, 688, 192, "Hue:");
            AddBackground(235, 684, 100, 28, 9350);
            AddTextEntry(243, 688, 100, 20, 0, 7, giver.HairHue.ToString());
            AddLabel(18, 728, 192, "Face:");
            AddBackground(60, 724, 100, 28, 9350);
            AddTextEntry(68, 728, 100, 20, 0, 8, giver.FacialHairItemId.ToString());
            AddLabel(190, 728, 192, "Hue:");
            AddBackground(235, 724, 100, 28, 9350);
            AddTextEntry(243, 28, 100, 20, 0, 9, giver.FacialHairHue.ToString());
            
            //{ "G": 0, "P": 0, "X": 345, "Y": 448, "W": 0, "H": 0, "U": 896, "T": "Label", "V": "Clothes/Equipment", "C": null },
            //{ "G": 0, "P": 0, "X": 670, "Y": 448, "W": 0, "H": 0, "U": 896, "T": "Label", "V": "Conversation", "C": null },
            AddLabel(345, 448, 896, "Clothes/Equipment");

            var i = 0;
            foreach (var itemName in giver.Clothes)
            {
                AddButton(360, 478+i*20, 11410, 11411, i+30000, GumpButtonType.Reply, 0);
                AddLabel(385, 478+i*20, 576, itemName);
                i++;
            }
            AddLabel(345, 728, 192, "Add Item:");
            AddBackground(405, 724, 100, 28, 9350);
            AddTextEntry(413, 728, 100, 20, 0, 10, string.Empty);
            AddButton(515, 725, 40021, 40031, 4, GumpButtonType.Reply, 0);
            AddLabel(565, 728, 1024, "Add");
            
            AddLabel(670, 45, 896, "Conversation");
            AddButton(860, 42, 40021, 40031, 6, GumpButtonType.Reply, 0);
            AddLabel(878, 45, 1024, "Add Dialog Node");

            i = 0;
            foreach (var node in giver.ConvoNodes)
            {
                AddButton(670, 78+i*20, 2104, 2103, i+40000, GumpButtonType.Reply, 0);
                AddLabel(691, 75+i*20, 256, $"{node.NodeId}");
                if(node.OptionText.Length > 0) AddLabel(720, 75+i*20, 640, $"{node.OptionText.Substring(0,Math.Min(12,node.OptionText.Length))}{(node.OptionText.Length>12?"...":string.Empty)}");
                AddLabel(820, 75+i*20, 768, $"{node.Text.Substring(0,Math.Min(24,node.Text.Length))}{(node.Text.Length>24?"...":string.Empty)}");
                i++;
            }

            AddButton(515, 445, 40021, 40031, 5, GumpButtonType.Reply, 0);
            AddLabel(565, 448, 1024, "Save");
        }

        public void BuildGiverList()
        {
            if (string.IsNullOrEmpty(State.Quest)) return;
            var jsonQuest = State.JsonQuests[State.Quest];

            if (jsonQuest.Givers == null) jsonQuest.Givers = new List<JsonQuestGiverConfig>();

            var i = 0;
            foreach (var g in jsonQuest.Givers)
            {
                AddButton(218, 64+i*20, 2104, 2103, i+20000, GumpButtonType.Reply, 0);
                AddLabel(236, 60+i*20, i==State.Giver?64:256, $"{g.Name} {g.Title}");
                i++;
            }
        }

        public void BuildQuestList()
        {
            var i = 0;
            foreach (var kvp in State.JsonQuests)
            {
                AddButton(18, 64+i*20, 2104, 2103, i+10000, GumpButtonType.Reply, 0);
                AddLabel(36, 60+i*20, kvp.Key==State.Quest?64:256, kvp.Key);
                i++;
            }
        }

        public void BuildControlButtons()
        {
            if (!State.WaitForResponse)
            {
                AddButton(510, 42, 40021, 40031, 1, GumpButtonType.Reply, 0);
                AddLabel(528, 45, 1024, "New Quest File");
                if (!string.IsNullOrEmpty(State.Quest))
                {
                    AddButton(510, 73, 40021, 40031, 2, GumpButtonType.Reply, 0);
                    AddLabel(523, 76, 1024, "New Quest Giver");
                }
                AddButton(515, 385, 40020, 40030, 8, GumpButtonType.Reply, 0);
                AddLabel(565, 388, 1024, "Load");
            }
        }

        public void BuildConvoNodeDetails()
        {
            if (State.ConvoNode == null || State.Giver == null) return;
            var node = State.JsonQuests[State.Quest].Givers[(int) State.Giver].ConvoNodes[(int) State.ConvoNode];
            AddBackground(0,30,650,770,1755);
            AddLabel(18,48,256,$"Node #{node.NodeId}");
            AddLabel(18,68,192,"Option Text:");
            AddBackground(100,64,530,28,9350);
            AddTextEntry(108,68,510,20,0,11,node.OptionText);
            AddLabel(18,98,192,"Dialog Text:");
            AddBackground(100,94,530,228,9350);
            AddTextEntry(108,98,510,220,0,12,node.Text,65536);
            AddLabel(18,328,192,"Node Options:");
            AddBackground(100,324,530,28,9350);
            AddTextEntry(108,328,510,20,0,13,string.Join(",",node.Nodes));

            AddLabel(18,358,192,"Trigger Item:");
            AddBackground(100,354,125,28,9350);
            AddTextEntry(108,358,120,20,0,14,node.TriggerItem?.Item);
            AddLabel(238,358,192,"Amount:");
            AddBackground(290,354,55,28,9350);
            AddTextEntry(298,358,47,20,0,15,node.TriggerItem?.Amount.ToString());
            AddLabel(358,358,192,"Props:");
            AddBackground(410,354,220,28,9350);

            var triggerItemProps = string.Empty;
            if (node.TriggerItem != null)
            {
                foreach (var kvp in node.TriggerItem.Props)
                {
                    triggerItemProps += $"{kvp.Key}:{kvp.Value},";
                }
                triggerItemProps = triggerItemProps.Substring(0, triggerItemProps.Length - 1);
            }

            AddTextEntry(418,358,212,20,0,16,triggerItemProps);

            AddLabel(18,388,192,"Reward Item:");
            AddBackground(100,384,125,28,9350);
            AddTextEntry(108,388,120,20,0,17,node.Item?.Item);
            AddLabel(238,388,192,"Amount:");
            AddBackground(290,384,55,28,9350);
            AddTextEntry(298,388,47,20,0,18,node.Item?.Amount.ToString());
            AddLabel(358,388,292,"Props:");
            AddBackground(410,384,220,28,9350);

            var rewardItemProps = string.Empty;
            if (node.Item != null)
            {
                foreach (var kvp in node.Item.Props)
                {
                    rewardItemProps += $"{kvp.Key}:{kvp.Value},";
                }
                rewardItemProps = rewardItemProps.Substring(0, rewardItemProps.Length - 1);
            }


            AddTextEntry(418,388,212,20,0,19,rewardItemProps);

            AddButton(500, 415, 40021, 40031, 7, GumpButtonType.Reply, 0);
            AddLabel(550, 418, 1024, "Save");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var json = JsonConvert.SerializeObject(info);
            //Console.WriteLine(json);
            //File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}output.json",json);
            //base.OnResponse(sender, info);
    
            var button = info.ButtonID;
            var switches = info.Switches;
            var text = info.TextEntries;
            
            switch (button)
            {
                case 0:
                    sender.Mobile.CloseGump(typeof(JsonQuestControlGump));
                    break;
                case 1:
                    State.WaitForResponse = true;
                    ResendGump(sender.Mobile);
                    break;
                case 2:
                    State.JsonQuests[State.Quest].Givers.Add(new JsonQuestGiverConfig()
                    {
                        Name="Questy",
                        Title="the Giver",
                        Clothes = new string[0],
                        ConvoNodes = new List<JsonQuestConvoNode>(),
                        StartLocation = new [] {0,0,0}
                    });
                    OpenGiver(sender.Mobile, State.JsonQuests[State.Quest].Givers.Count-1);
                    break;
                case 3:
                    var name = text.FirstOrDefault(x => x.EntryID == 1000)?.Text;
                    State.WaitForResponse = false;
                    State.JsonQuests.Add(name, new JsonQuest());
                    OpenQuest(sender.Mobile, name);
                    break;
                case 4:
                    var item = text.FirstOrDefault(x => x.EntryID == 10)?.Text;
                    var test = JsonSystemHelper.NewItemByTypeString(item);
                    if (test == null)
                    {
                        State.ErrorMessage = $"{item} is not a valid item";
                        ResendGump(sender.Mobile);
                        return;
                    }

                    ((Item)test).Delete();
                    var newType = test.GetType().ToString().Split('.').Last();
                    AddItemToGiver(sender.Mobile, newType);
                    break;
                case 5:
                    ApplyNonCollectionChangesToGiver(switches, text);
                    var file = $"Scripts/Customs/JsonSystem/JsonQuests/Data/{State.Quest}.json";
                    var j = JsonConvert.SerializeObject(State.JsonQuests[State.Quest]);
                    File.WriteAllText(file,j);
                    State.SuccessMessage = $"{State.Quest} saved.";
                    ResendGump(sender.Mobile);
                    break;
                case 6:
                    AddNewConvoNodeToGiver(sender.Mobile);
                    break;
                case 7:
                    SaveConvoNode(switches, text);
                    State.ConvoNode = null;
                    ResendGump(sender.Mobile);
                    break;
                case 8:
                    JsonQuestEngine.LoadQuests();
                    State.SuccessMessage = "Quests reloaded.";
                    ResendGump(sender.Mobile);
                    break;
                default:
                    
                    if (button >= 10000 && button < 20000)
                    {
                        var questName = State.JsonQuests.ElementAt(button - 10000).Key;
                        OpenQuest(sender.Mobile, questName);
                        return;
                    }
                    
                    if (button >= 20000 && button < 30000)
                    {
                        OpenGiver(sender.Mobile, button-20000);
                        return;
                    }
                    
                    if (button >= 30000 && button < 40000)
                    {
                        RemoveItemFromGiver(sender.Mobile, button-30000);
                        return;
                    }
                    
                    if (button >= 40000 && button < 50000)
                    {
                        OpenConvoNode(sender.Mobile, button-40000);
                        return;
                    }

                    ResendGump(sender.Mobile);
                    break;
            }
        }

        public void SaveConvoNode(int[] switches, TextRelay[] text)
        {
            var node = State.JsonQuests[State.Quest].Givers[(int) State.Giver].ConvoNodes[(int) State.ConvoNode];
            node.OptionText = text.FirstOrDefault(x => x.EntryID == 11)?.Text;
            node.Text = text.FirstOrDefault(x => x.EntryID == 12)?.Text;
            var options = text.FirstOrDefault(x => x.EntryID == 13)?.Text;
            var optionIds = new List<int>();
            if (!string.IsNullOrEmpty(options))
            {
                foreach (var option in options.Split(',')) optionIds.Add(Convert.ToInt32(option));
            }
            node.Nodes = optionIds;
            
            var triggerItem = text.FirstOrDefault(x => x.EntryID == 14)?.Text;
            if (!string.IsNullOrEmpty(triggerItem))
            {
                var ti = new JsonItem()
                {
                    Item = triggerItem,
                    Amount = Convert.ToInt32(text.FirstOrDefault(x => x.EntryID == 15)?.Text),
                    Props = new Dictionary<string, object>()
                };
                var triggerItemProps = text.FirstOrDefault(x => x.EntryID == 16)?.Text;
                if (!string.IsNullOrEmpty(triggerItemProps))
                {
                    var tiprops = triggerItemProps.Split(',');
                    foreach (var tip in tiprops)
                    {
                        var prop = tip.Split(':');
                        int vi;
                        double vd;
                        bool vb;
                        var i = Int32.TryParse(prop[1], out vi);
                        var d = Double.TryParse(prop[1], out vd);
                        var b = Boolean.TryParse(prop[1], out vb);
                        object v = i ? vi : d ? vd : b ? vb : (object)prop[1];
                        ti.Props.Add(prop[0],v);
                    }
                }
                node.TriggerItem = ti;
            }
            
            var rewardItem = text.FirstOrDefault(x => x.EntryID == 17)?.Text;
            if (!string.IsNullOrEmpty(rewardItem))
            {
                var ri = new JsonItem()
                {
                    Item = rewardItem,
                    Amount = Convert.ToInt32(text.FirstOrDefault(x => x.EntryID == 18)?.Text),
                    Props = new Dictionary<string, object>()
                };
                var rewardItemProps = text.FirstOrDefault(x => x.EntryID == 19)?.Text;
                if (!string.IsNullOrEmpty(rewardItemProps))
                {
                    var riprops = rewardItemProps.Split(',');
                    foreach (var rip in riprops)
                    {
                        var prop = rip.Split(':');
                        int vi;
                        double vd;
                        bool vb;
                        var i = Int32.TryParse(prop[1], out vi);
                        var d = Double.TryParse(prop[1], out vd);
                        var b = Boolean.TryParse(prop[1], out vb);
                        object v = i ? vi : d ? vd : b ? vb : (object)prop[1];
                        ri.Props.Add(prop[0],v);
                    }
                }
                node.Item = ri;
            }

            ApplyNonCollectionChangesToGiver(switches, text);
        }

        public void ApplyNonCollectionChangesToGiver(int[] switches, TextRelay[] text)
        {
            var giver = State.JsonQuests[State.Quest].Givers[(int) State.Giver];
            giver.Name = text.FirstOrDefault(x => x.EntryID == 0)?.Text;
            giver.Title = text.FirstOrDefault(x => x.EntryID == 1)?.Text;
            giver.StartMap = text.FirstOrDefault(x => x.EntryID == 2)?.Text;
            var pointString = text.FirstOrDefault(x => x.EntryID == 3)?.Text;
            var psSplit = pointString.Split(',');
            giver.StartLocation = new [] {Convert.ToInt32(psSplit[0]), Convert.ToInt32(psSplit[1]),
                Convert.ToInt32(psSplit[2])
            };
            var body = text.FirstOrDefault(x => x.EntryID == 4)?.Text;
            giver.Body = string.IsNullOrEmpty(body) ? (int?)null : Convert.ToInt32(body);
            var hue = text.FirstOrDefault(x => x.EntryID == 5)?.Text;
            giver.Hue = string.IsNullOrEmpty(hue) ? (int?)null : Convert.ToInt32(hue);
            var hair = text.FirstOrDefault(x => x.EntryID == 6)?.Text;
            giver.HairItemId = string.IsNullOrEmpty(hair) ? (int?)null : Convert.ToInt32(hair);
            var hairhue = text.FirstOrDefault(x => x.EntryID == 7)?.Text;
            giver.HairHue = string.IsNullOrEmpty(hairhue) ? (int?)null : Convert.ToInt32(hairhue);
            var face = text.FirstOrDefault(x => x.EntryID == 8)?.Text;
            giver.FacialHairItemId = string.IsNullOrEmpty(face) ? (int?)null : Convert.ToInt32(face);
            var facehue = text.FirstOrDefault(x => x.EntryID == 9)?.Text;
            giver.FacialHairHue = string.IsNullOrEmpty(facehue) ? (int?)null : Convert.ToInt32(facehue);
            giver.RandomGender = switches.Contains(1);
            giver.Female = switches.Contains(3);
            giver.AutoDress = switches.Contains(4);
            giver.RandomName = switches.Contains(5);
            giver.CantWalk = switches.Contains(6);
        }

        public void AddNewConvoNodeToGiver(Mobile from)
        {
            var node = new JsonQuestConvoNode(){ NodeId = State.JsonQuests[State.Quest].Givers[(int) State.Giver].ConvoNodes.Count, Nodes = new List<int>()};
            State.JsonQuests[State.Quest].Givers[(int) State.Giver].ConvoNodes.Add(node);
            State.ConvoNode = node.NodeId;
            ResendGump(from);
        }

        public void AddItemToGiver(Mobile from, string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                ResendGump(from);
                return;
            }
            State.JsonQuests[State.Quest].Givers[(int) State.Giver].Clothes = 
                State.JsonQuests[State.Quest].Givers[(int) State.Giver].Clothes.Append(item).ToArray();
            ResendGump(from);
        }

        public void RemoveItemFromGiver(Mobile from, int itemIndex)
        {
            State.JsonQuests[State.Quest].Givers[(int) State.Giver].Clothes = 
                State.JsonQuests[State.Quest].Givers[(int) State.Giver].Clothes.RemoveAt(itemIndex);
            ResendGump(from);
        }

        public void OpenGiver(Mobile from, int giver)
        {
            if (string.IsNullOrEmpty(State.Quest)) return;
            State.Giver = giver;
            State.ConvoNode = null;
            ResendGump(from);
        }

        public void OpenConvoNode(Mobile from, int node)
        {
            if (State.Giver == null) return;
            State.ConvoNode = node;
            ResendGump(from);
        }

        public void OpenQuest(Mobile from, string questName)
        {
            //Console.WriteLine($"OpenQuest: {questName}");
            State.Giver = null;
            State.ConvoNode = null;
            State.Quest = questName;
            ResendGump(from);
        }

        public void ResendGump(Mobile from)
        {
            from.SendGump(new JsonQuestControlGump(State));
        }
    }
}

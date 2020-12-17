using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using ServerUtilityExtensions;

namespace Server.Customs.JsonQuests
{
    public class JsonQuestGiver : BaseCreature 
    { 
        [Constructable] 
        public JsonQuestGiver()
            : base(AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4)
        { 
            InitStats(31, 41, 51); 

            SpeechHue = Utility.RandomDyedHue(); 

            Hue = Utility.RandomSkinHue(); 

            Body = 0x190; 
            Name = NameList.RandomName("male");
            Utility.AssignRandomHair(this);
            
            ConvoNodes = new List<JsonQuestConvoNode>();
        }
        
        public JsonQuestGiver(Serial serial)
            : base(serial)
        {
            ConvoNodes = new List<JsonQuestConvoNode>();
        }

        public List<JsonQuestConvoNode> ConvoNodes { get; set; }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return (from.Alive && InRange(from, 3));
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (e.Mobile.IsPlayer() || e.Mobile.IsStaff())
            {
                switch (e.Speech)
                {
                    case "hail":
                        if (ConvoNodes.Count > 0)
                        {
                            e.Mobile.SendGump(new JsonQuestGump(this));
                        }
                        else
                        {
                            PublicOverheadMessage(MessageType.Regular, 0x35, false, "I haven't been given a brain yet.");
                        }

                        break;
                }
            }
        }

        public override bool OnDragDrop(Mobile @from, Item dropped)
        {
            var trigger = dropped.GetType().ToString().Split('.').Last();
            var node = ConvoNodes.FirstOrDefault(x => JsonQuestHelper.IsEqualJsonQuestItem(dropped,x.TriggerItem));
            if (node == null)
            {
                PublicOverheadMessage(MessageType.Regular, 0x35, false, "That's not right");
                from.AddToBackpack(dropped);
                return false;
            }
            
            from.SendGump(new JsonQuestGump(this, node.NodeId));
            dropped.Delete();

            JsonQuestHelper.CheckGiveItem(this, node, from);

            return true;
        }

        public void LoadConfig(JsonQuestGiverConfig config)
        {
            foreach (var item in Items)
            {
                item.Delete();
            }
            ConvoNodes = config.ConvoNodes;
            Female = config.RandomGender ? Utility.RandomBool() : config.Female;
            Body = config.Body ?? (Female ? 0x191 : 0x190);
            Name = config.RandomName ? NameList.RandomName(Female ? "female" : "male") : config.Name;
            Title = config.Title;
            if (Map == null || Map == Map.Internal)
            {
                Map = Map.Parse(config.StartMap);
            }
            Location = new Point3D(config.StartLocation[0], config.StartLocation[1], config.StartLocation[2]);
            CantWalk = config.CantWalk;
            Hue = config.Hue??Utility.RandomSkinHue();
            SpeechHue = config.SpeechHue??Utility.RandomDyedHue();
            if (config.HairItemId != null) HairItemID = (int)config.HairItemId;
            if (config.HairHue != null) HairHue = (int)config.HairHue;
            if (config.FacialHairItemId != null) FacialHairItemID = (int)config.FacialHairItemId;
            if (config.FacialHairHue != null) FacialHairHue = (int)config.FacialHairHue;

            if (config.AutoDress)
            {
                if (Female)
                {
                    AddItem(new FancyDress(Utility.RandomDyedHue()));
                }
                else
                {
                    AddItem(new LongPants(Utility.RandomNeutralHue()));
                    AddItem(new FancyShirt(Utility.RandomDyedHue()));
                }
                AddItem(new Boots(Utility.RandomNeutralHue()));
            }
            else
            {
                foreach (var clothing in config.Clothes)
                {
                    var item = JsonQuestHelper.NewItemByTypeString(clothing);
                    if (item == null) continue;
                    AddItem((Item) item);
                }
            }
        }

        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt();
        }
    }
}

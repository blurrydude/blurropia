using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;
using Server.Mobiles;

namespace Server.Customs.JsonSystem.Theater
{
    public class JsonActor : BaseCreature
    {
        [Constructable] 
        public JsonActor()
            : base(AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4)
        { 
            this.InitStats(31, 41, 51);

            this.SpeechHue = Utility.RandomDyedHue(); 

            this.Hue = Utility.RandomSkinHue(); 

            if (this.Female = Utility.RandomBool()) 
            { 
                this.Body = 0x191; 
                this.Name = NameList.RandomName("female");
                this.AddItem(new FancyDress(Utility.RandomDyedHue())); 
                this.Title = "the actress"; 
            }
            else 
            { 
                this.Body = 0x190; 
                this.Name = NameList.RandomName("male");
                this.AddItem(new LongPants(Utility.RandomNeutralHue())); 
                this.AddItem(new FancyShirt(Utility.RandomDyedHue()));
                this.Title = "the actor";
            }

            this.AddItem(new Boots(Utility.RandomNeutralHue()));

            Utility.AssignRandomHair(this);

            Container pack = new Backpack(); 

            pack.DropItem(new Gold(250, 300)); 

            pack.Movable = false; 

            this.AddItem(pack); 
        }

        [Constructable] 
        public JsonActor(JsonActorConfig config)
            : base(AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4)
        { 
            this.InitStats(31, 41, 51);

            this.SpeechHue = config.SpeechHue??Utility.RandomDyedHue(); 

            this.Hue = config.Hue??Utility.RandomSkinHue(); 

            if (this.Female = (config.RandomGender?Utility.RandomBool():config.Female)) 
            { 
                this.Body = config.Body??0x191; 
                this.Name = config.RandomName?NameList.RandomName("female"):config.Name;
                if(config.AutoDress) this.AddItem(new FancyDress(Utility.RandomDyedHue())); 
            }
            else 
            { 
                this.Body = config.Body??0x190; 
                this.Name = config.RandomName?NameList.RandomName("male"):config.Name;
                if (config.AutoDress)
                {
                    this.AddItem(new LongPants(Utility.RandomNeutralHue()));
                    this.AddItem(new FancyShirt(Utility.RandomDyedHue()));
                }
            }

            Title = config.Title;

            if(config.AutoDress) this.AddItem(new Boots(Utility.RandomNeutralHue()));

            if (config.HairItemId == null)
            {
                Utility.AssignRandomHair(this);
            }
            else
            {
                this.HairItemID = (int)config.HairItemId;
            }
            
            if (config.HairHue != null) HairHue = (int) config.HairHue;
            if (config.FacialHairItemId != null) this.FacialHairItemID = (int) config.FacialHairItemId;
            if (config.FacialHairHue != null) FacialHairHue = (int) config.FacialHairHue;

            if (config.Clothes != null)
            {
                foreach (var clothing in config.Clothes)
                {
                    var item = JsonSystemHelper.NewItemByTypeString(clothing);
                    if (item == null) continue;
                    AddItem((Item) item);
                }
            }
        }

        public JsonActor(Serial serial)
            : base(serial)
        { 
        }
        
        public override bool ClickTitle
        {
            get
            {
                return false;
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

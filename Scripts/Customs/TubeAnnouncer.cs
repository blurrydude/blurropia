using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Customs
{
    public class TubeAnnouncer : BaseCreature
    {
        private Item m_teleporter;

        [Constructable] 
        public TubeAnnouncer()
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

            this.CantWalk = true;
        }

        public TubeAnnouncer(Serial serial)
            : base(serial)
        { 
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return (from.Alive && InRange(from, 12));
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (e.Mobile.IsStaff())
            {
                switch (e.Speech)
                {
                    case "start building station":
                        StartBuildingStation();
                        break;
                }
            }
        }

        private void StartBuildingStation()
        {
            if (m_teleporter == null)
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, false,
                    "I will begin building a Brittania Underground Station here.");
                NavPoints = new Dictionary<Map, List<Point2D>>();
                var basePoint = Location;
                var telePoint = basePoint + new Point3D(0, -3, 0);
                /*var list = new List<Point2D>();
                list.Add(basePoint + new Point2D(-2,-2));
                list.Add(basePoint + new Point2D(2,2));
                NavPoints.Add(Map, list);
                CantWalk = false;*/
                m_teleporter = new Teleporter(telePoint, Map);
                m_teleporter.MoveToWorld(telePoint, Map);
            }
            else
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, false,
                    "I've already built it. Are you blind?");
            }
        }


        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); // version
            writer.Write(m_teleporter);
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt();
            m_teleporter = reader.ReadItem();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Customs.JsonSystem
{
    public class AirshipCaptain : BaseCreature
    {
        private Item m_teleporter;

        [Constructable] 
        public AirshipCaptain()
            : base(AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4)
        { 
            this.InitStats(31, 41, 51);

            this.SpeechHue = Utility.RandomDyedHue(); 

            this.Hue = Utility.RandomSkinHue(); 

            if (this.Female = Utility.RandomBool()) 
            { 
                this.Body = 0x191; 
                this.Name = $"Cpt. {NameList.RandomName("female")}";
                this.AddItem(new FancyDress(Utility.RandomDyedHue())); 
            }
            else 
            { 
                this.Body = 0x190; 
                this.Name = $"Cpt. {NameList.RandomName("male")}";
                this.AddItem(new LongPants(Utility.RandomNeutralHue())); 
                this.AddItem(new FancyShirt(Utility.RandomDyedHue()));
            }
            
            this.AddItem(new Boots(Utility.RandomNeutralHue()));

            Utility.AssignRandomHair(this);

            Container pack = new Backpack(); 

            pack.DropItem(new Gold(250, 300)); 

            pack.Movable = false; 

            this.AddItem(pack);

            this.CantWalk = true;
        }

        public AirshipCaptain(Serial serial)
            : base(serial)
        { 
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return (from.Alive && InRange(from, 12));
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (e.Mobile.IsPlayer() || e.Mobile.IsStaff())
            {
                switch (e.Speech)
                {
                    case "book passage":
                        e.Mobile.SendGump(new AirshipGump(this));
                        break;
                    case "setup captains":
                        for (var i = 2; i < DestinationList.Count; i++)
                        {
                            var landing = DestinationList[i];
                            var captainLoc = landing.Location + new Point3D(-2, 0, 0);
                            var captain = new AirshipCaptain();
                            captain.Location = captainLoc;
                            captain.Map = landing.Map;
                            World.AddMobile(captain);
                            e.Mobile.SendMessage(0x49, "Airship captain stationed in "+landing.Name);
                        }
                        break;
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

        public List<AirshipDestination> DestinationList = new List<AirshipDestination> {
            new AirshipDestination() { Location = new Point3D(1416, 1702, 6), Map = Map.Trammel, Name = "West Britain" }, // this is here for a reason, promise. It offsets the collection and why not just copy it?
            new AirshipDestination() { Location = new Point3D(1416, 1702, 6), Map = Map.Trammel, Name = "West Britain" },
            new AirshipDestination() { Location = new Point3D(2662, 2107, 0), Map = Map.Trammel, Name = "Buccaneer's Den" },
            new AirshipDestination() { Location = new Point3D(2287, 1201, 0), Map = Map.Trammel, Name = "Cove" },
            new AirshipDestination() { Location = new Point3D(1383, 3871, 0), Map = Map.Trammel, Name = "Jhelom" },
            new AirshipDestination() { Location = new Point3D(3676, 2116, 20), Map = Map.Trammel, Name = "Magincia" },
            new AirshipDestination() { Location = new Point3D(2495, 445, 15), Map = Map.Trammel, Name = "Minoc" },
            new AirshipDestination() { Location = new Point3D(4461, 1175, 0), Map = Map.Trammel, Name = "Moonglow" },
            new AirshipDestination() { Location = new Point3D(3765, 1291, 0), Map = Map.Trammel, Name = "Nu Jel'm" },
            new AirshipDestination() { Location = new Point3D(3510, 2572, 14), Map = Map.Trammel, Name = "New Haven" },
            new AirshipDestination() { Location = new Point3D(3037, 3401, 15), Map = Map.Trammel, Name = "Serpents Hold" },
            new AirshipDestination() { Location = new Point3D(625, 2237, 0), Map = Map.Trammel, Name = "Skara Brae" },
            new AirshipDestination() { Location = new Point3D(1911, 2682, 0), Map = Map.Trammel, Name = "Trinsic" },
            new AirshipDestination() { Location = new Point3D(3001, 817, 0), Map = Map.Trammel, Name = "Vesper" },
            new AirshipDestination() { Location = new Point3D(529, 995, 0), Map = Map.Trammel, Name = "Yew" },
            new AirshipDestination() { Location = new Point3D(5207, 4057, 36), Map = Map.Trammel, Name = "Delucia" },
            new AirshipDestination() { Location = new Point3D(5819, 3253, -3), Map = Map.Trammel, Name = "Papua" },
            new AirshipDestination() { Location = new Point3D(1088, 510, -90), Map = Map.Malas, Name = "Luna" },
            new AirshipDestination() { Location = new Point3D(1944, 1310, -90), Map = Map.Malas, Name = "Umbra" },
            new AirshipDestination() { Location = new Point3D(698, 1185, 25), Map = Map.Tokuno, Name = "Zento" },
            new AirshipDestination() { Location = new Point3D(776, 3508, -19), Map = Map.TerMur, Name = "Royal City" },
            new AirshipDestination() { Location = new Point3D(968, 3954, -42), Map = Map.TerMur, Name = "Holy City" }
        };
    }

    public class AirshipDestination
    {
        public string Name { get; set; }
        public Point3D Location { get; set; }
        public Map Map { get; set; }

        public int Distance(Point3D from)
        {
            return Math.Abs(from.DistanceTo(Location));
        }

        public int Cost(Point3D from)
        {
            return Distance(from) / 4;
        }

        public TimeSpan TravelTime(Point3D from)
        {
            return TimeSpan.FromSeconds(Distance(from) / 10);
        }

        public void Serialize(GenericWriter writer) 
        { 
            writer.Write((int)0); // version
            writer.Write(Name);
            writer.Write(Location);
            writer.Write(Map);
        }

        public void Deserialize(GenericReader reader) 
        { 
            int version = reader.ReadInt();
            Name = reader.ReadString();
            Location = reader.ReadPoint3D();
            Map = reader.ReadMap();
        }
    }

    public class AirshipGump : Gump
    {
        public static readonly int SetWidth = 200;
        private static readonly int NameWidth = 107;
        private static readonly int ValueWidth = 128;
        private static readonly int EntryCount = 15;
        private static readonly int TypeWidth = NameWidth + PropsConfig.OffsetSize + ValueWidth;
        private static readonly int TotalWidth =PropsConfig. OffsetSize + NameWidth + PropsConfig.OffsetSize + ValueWidth + PropsConfig.OffsetSize + SetWidth + PropsConfig.OffsetSize;
        
        private static readonly int BackWidth = PropsConfig.BorderSize + TotalWidth + PropsConfig.BorderSize;
        private static readonly int IndentWidth = 12;
        private static AirshipCaptain _captain;

        public AirshipGump(AirshipCaptain captain)
            : base(PropsConfig.GumpOffsetX, PropsConfig.GumpOffsetY)
        {
            _captain = captain;
            int totalHeight = 520;

            this.AddPage(0);

            this.AddBackground(0, 0, BackWidth, PropsConfig.BorderSize + totalHeight + PropsConfig.BorderSize, PropsConfig.BackGumpID);
            this.AddImageTiled(PropsConfig.BorderSize, PropsConfig.BorderSize, TotalWidth - (PropsConfig.OldStyle ? SetWidth + PropsConfig.OffsetSize : 0), totalHeight, PropsConfig.OffsetGumpID);

            int x = 25;//PropsConfig.BorderSize + PropsConfig.OffsetSize;
            int y = 25;//PropsConfig.BorderSize + PropsConfig.OffsetSize;
            int i = 1;

            int emptyWidth = TotalWidth - PropsConfig.PrevWidth - PropsConfig.NextWidth - (PropsConfig.OffsetSize * 4) - (PropsConfig.OldStyle ? SetWidth + PropsConfig.OffsetSize : 0);

            this.AddLabel(x,y,256,"I will take you to the following desinations for a fee:");
            
            foreach (var dest in captain.DestinationList.Skip(1))
            {
                var name = dest.Name;
                var price = dest.Cost(captain.Location);
                if (captain.Map != dest.Map)
                {
                    price = price * 2;
                }
                var time = dest.TravelTime(captain.Location);
                if (captain.Map != dest.Map)
                {
                    time = time.Add(TimeSpan.FromMinutes(3));
                }

                if (price > 20)
                {
                    y += 24;
                    this.AddButton(x, y, PropsConfig.NextButtonID1, PropsConfig.NextButtonID2, i, GumpButtonType.Reply,
                        0);
                    var text = "";
                    if (time.Hours > 0) text += $"{time.Hours} hour{(time.Hours > 1 ? "s" : "")} ";
                    if (time.Minutes > 0) text += $"{time.Minutes} minute{(time.Minutes > 1 ? "s" : "")} ";
                    if (time.Seconds > 0) text += $"{time.Seconds} second{(time.Seconds > 1 ? "s" : "")}";
                    this.AddLabel(x + 20, y, 256, $"{name} for {price} gold. Trip time: {text}");
                }

                i++;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                return;
            }
            var dest = _captain.DestinationList[info.ButtonID];
            var cost = dest.Cost(_captain.Location);
            if (_captain.Map != dest.Map)
            {
                cost = cost * 2;
            }
            var time = dest.TravelTime(_captain.Location);
            if (_captain.Map != dest.Map)
            {
                time = time.Add(TimeSpan.FromMinutes(3));
            }

            var gold = sender.Mobile.Backpack.TotalGold;
            if (gold < cost)
            {
                _captain.PublicOverheadMessage(MessageType.Regular, 256, false, "You better go get enough gold to cover your passage or don't waste my time!");
                return;
            }

            sender.Mobile.Backpack.ConsumeTotal(typeof(Gold), cost);
            sender.Mobile.Map = Map.Trammel;
            sender.Mobile.Location = new Point3D(5301, 1079, 24);
            var timer = Timer.DelayCall(time,MoveToDestination,dest.Location,dest.Map,sender.Mobile);
            Timer.DelayCall(TimeSpan.FromSeconds(5), TimeRemainingNotifier, timer, sender.Mobile, dest.Name);
        }

        private void TimeRemainingNotifier(Timer timer, Mobile passenger, string destName)
        {
            var arrival = timer.Next;
            var now = DateTime.UtcNow;
            var remaining = arrival - now;
            var arrivingIn = $"Arriving at {destName} in ";
            if (remaining.Hours > 0) arrivingIn += $"{remaining.Hours} hours ";
            if (remaining.Minutes > 0) arrivingIn += $"{remaining.Minutes} minutes ";
            if (remaining.Seconds > 0) arrivingIn += $"{remaining.Seconds} seconds ";
            passenger.SendMessage(259,arrivingIn);
            if (remaining > TimeSpan.FromSeconds(15))
            {
                Timer.DelayCall(TimeSpan.FromSeconds(15), TimeRemainingNotifier, timer, passenger, destName);
            }
        }

        private void MoveToDestination(Point3D dest, Map map, Mobile mobile)
        {
            mobile.Map = map;
            mobile.Location = dest;
        }
    }
}

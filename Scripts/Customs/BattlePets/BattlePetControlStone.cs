using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Customs.BattlePets
{
    public class BattlePetControlStone : Item
    {
        [Constructable]
        public BattlePetControlStone() : base(9242)
        {
            Hue = 1259;
            Name = "Pet Battle Stone";
            Fence = new List<int>();
        }

        public BattlePetControlStone(Serial serial) : base(serial)
        {

        }

        private List<int> Fence { get; set; }

        private BattlePet _LeftBattlePet;
        [CommandProperty(AccessLevel.GameMaster)]
        public BattlePet LeftBattlePet
        {
            get
            {
                return _LeftBattlePet;
            }
            set
            {
                _LeftBattlePet = value;
            }
        }

        private BattlePet _RightBattlePet;
        [CommandProperty(AccessLevel.GameMaster)]
        public BattlePet RightBattlePet
        {
            get
            {
                return _RightBattlePet;
            }
            set
            {
                _RightBattlePet = value;
            }
        }

        private BattlePetStone _LeftBattlePetStone;
        [CommandProperty(AccessLevel.GameMaster)]
        public BattlePetStone LeftBattlePetStone
        {
            get
            {
                return _LeftBattlePetStone;
            }
            set
            {
                _LeftBattlePetStone = value;
            }
        }

        private BattlePetStone _RightBattlePetStone;
        [CommandProperty(AccessLevel.GameMaster)]
        public BattlePetStone RightBattlePetStone
        {
            get
            {
                return _RightBattlePetStone;
            }
            set
            {
                _RightBattlePetStone = value;
            }
        }

        private Mobile _LeftBattlePetStoneFrom;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile LeftBattlePetStoneFrom
        {
            get
            {
                return _LeftBattlePetStoneFrom;
            }
            set
            {
                _LeftBattlePetStoneFrom = value;
            }
        }

        private Mobile _RightBattlePetStoneFrom;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile RightBattlePetStoneFrom
        {
            get
            {
                return _RightBattlePetStoneFrom;
            }
            set
            {
                _RightBattlePetStoneFrom = value;
            }
        }

        public void InitializeBattle()
        {
            PublicOverheadMessage(MessageType.Regular, 166, false,
                "Battle will commence in thirty seconds!");
            MakeFence();
            Timer.DelayCall(TimeSpan.FromSeconds(25), PopPets);
            Timer.DelayCall(TimeSpan.FromSeconds(30), SetPetTeams);
        }

        public void PopPets()
        {
            _LeftBattlePet = CreatePetFromStone(_LeftBattlePetStone);
            _RightBattlePet = CreatePetFromStone(_RightBattlePetStone);
            _LeftBattlePet.MoveToWorld(Location + new Point3D(-4, 5, 0),Map);
            _RightBattlePet.MoveToWorld(Location + new Point3D(4, 5, 0),Map);
        }

        public void SetPetTeams()
        {
            PublicOverheadMessage(MessageType.Regular, 166, false,
                "FIGHT!!");
            _LeftBattlePet.Team = 1;
            _RightBattlePet.Team = 2;
        }

        public BattlePet CreatePetFromStone(BattlePetStone stone)
        {
            var body = stone.BattlePetBody;
            var iAi = stone.AIType;
            var str = stone.Str;
            var dex = stone.Dex;
            var inte = stone.Int;
            var arm = stone.Armor;
            var dam = stone.Damage;
            var hue = stone.Hue;
            var enhancements = stone.Enhancements;
            var pet = new BattlePet(this,body,iAi,str,inte,dex,arm,dam,hue,enhancements);
            pet.Name = $"{stone.Owner.Name}'s {stone.Name}";
            return pet;
        }

        public void MakeFence()
        {
            var x = Location.X - 8;
            var y = Location.Y + 1;
            var list = new List<Point3D>();
            for (var xx = x; xx < Location.X + 18; xx++)
            {
                if (xx == x || xx == Location.X + 17)
                {
                    for (var yy = y; yy < Location.Y + 10; yy++)
                    {
                        list.Add(new Point3D(xx, yy, 0));
                        list.Add(new Point3D(xx, yy, 20));
                        list.Add(new Point3D(xx, yy, 40));
                    }
                }
                else
                {
                    list.Add(new Point3D(xx, y, 0));
                    list.Add(new Point3D(xx, y, 20));
                    list.Add(new Point3D(xx, y, 40));
                    list.Add(new Point3D(xx, y+9, 0));
                    list.Add(new Point3D(xx, y+9, 20));
                    list.Add(new Point3D(xx, y+9, 40));
                }
            }

            foreach (var point in list)
            {
                var blocker = new Blocker();
                blocker.MoveToWorld(point, Map);
                Fence.Add(blocker.Serial);
            }
        }

        public void DestroyFence()
        {
            foreach (var serial in Fence)
            {
                var blocker = World.FindItem(serial);
                //World.RemoveItem(blocker);
                blocker.Delete();
            }
            Fence.Clear();
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.InRange( this.GetWorldLocation(), 2 ) == false )
            {
                from.SendLocalizedMessage( 500486 );	//That is too far away.
            }
            else if (from == _LeftBattlePetStoneFrom)
            {
                _LeftBattlePetStoneFrom.AddToBackpack(_LeftBattlePetStone);
                _LeftBattlePetStone = null;
                _LeftBattlePetStoneFrom = null;
            }
            else if (from == _RightBattlePetStoneFrom)
            {
                _RightBattlePetStoneFrom.AddToBackpack(_RightBattlePetStone);
                _RightBattlePetStone = null;
                _RightBattlePetStoneFrom = null;
            }
            else if (_LeftBattlePetStone != null && _RightBattlePetStone != null)
            {
                from.SendMessage("Cannot accept any more Battle Pets.");
            }
            else
            {
                from.Target=new BattlePetControlStoneTarget( this );
                from.SendMessage( "Target your Battle Pet Stone" );
            }
        }

        private class BattlePetControlStoneTarget : Target
        {
            private BattlePetControlStone _ControlStone;

            public BattlePetControlStoneTarget(BattlePetControlStone controlStone) : base( 3, false, TargetFlags.None )
            {
                _ControlStone = controlStone;
            }
			
            protected override void OnTarget( Mobile from, object targ )
            {
                if (!(targ is BattlePetStone))
                {
                    from.SendMessage("That is not a Battle Pet Stone");
                    return;
                }
                var bps = (BattlePetStone) targ;
                if (bps.Owner == null)
                {
                    from.SendMessage("That Battle Pet Stone has no owner.");
                    return;
                }

                if (_ControlStone.LeftBattlePetStone == null)
                {
                    from.SendMessage("Waiting on one more Battle Pet");
                    _ControlStone.LeftBattlePetStoneFrom = from;
                    _ControlStone.LeftBattlePetStone = (BattlePetStone) targ;
                    _ControlStone.LeftBattlePetStone.Internalize();
                    return;
                }

                if (_ControlStone.RightBattlePetStone == null)
                {
                    from.SendMessage("Battle Pets Ready. Beginning battle in 30 seconds.");
                    _ControlStone.RightBattlePetStoneFrom = from;
                    _ControlStone.RightBattlePetStone = (BattlePetStone) targ;
                    _ControlStone.RightBattlePetStone.Internalize();
                    _ControlStone.InitializeBattle();
                    return;
                }
                from.SendMessage("Cannot accept any more Battle Pets.");

                return;
            }
        }

        public void OnBattlePetDeath(int petSerial)
        {
            AwardExperience(_LeftBattlePet.Serial != petSerial);
            RollForEnhancement(_LeftBattlePet.Serial != petSerial);
            PublicOverheadMessage(MessageType.Regular, 166, false,
                "We have a winner!");
            PublicOverheadMessage(MessageType.Regular, 166, false,
                _LeftBattlePet.Serial != petSerial ? _LeftBattlePet.Name : _RightBattlePet.Name);
            Timer.DelayCall(TimeSpan.FromSeconds(2),ClearBattle);
        }

        public void AwardExperience(bool leftWins)
        {
            var leftExperience = leftWins ? _RightBattlePetStone.ExpValue : Math.Max(1,_RightBattlePetStone.ExpValue / 10);
            var rightExperience = leftWins ? Math.Max(1,_LeftBattlePetStone.ExpValue / 10) : _LeftBattlePetStone.ExpValue;

            _LeftBattlePetStoneFrom.SendMessage($"Your Battle Pet has earned {leftExperience} experience.");
            _RightBattlePetStoneFrom.SendMessage($"Your Battle Pet has earned {rightExperience} experience.");

            _LeftBattlePetStone.Exp = _LeftBattlePetStone.Exp + leftExperience;
            _RightBattlePetStone.Exp = _RightBattlePetStone.Exp + rightExperience;
        }

        public void RollForEnhancement(bool leftWins)
        {
            var roll = Utility.Random(1, 100);

            if (roll < 90)
            {
                return;
            }

            var enhancement = new BattlePetStoneEnhancement();

            if (leftWins)
            {
                _LeftBattlePetStoneFrom.AddToBackpack(enhancement);
                _LeftBattlePetStoneFrom.SendMessage($"You receive one {enhancement.Rarity} enhancement.");
                return;
            }

            _RightBattlePetStoneFrom.AddToBackpack(enhancement);
            _RightBattlePetStoneFrom.SendMessage($"You receive one {enhancement.Rarity} enhancement.");
        }

        public void ClearBattle()
        {
            if (_LeftBattlePet.Alive) _LeftBattlePet.Delete(); //World.RemoveMobile(_LeftBattlePet);
            if (_RightBattlePet.Alive) _RightBattlePet.Delete(); //World.RemoveMobile(_RightBattlePet);
            _LeftBattlePet = null;
            _RightBattlePet = null;
            EjectStones();
            ClearCorpses();
            DestroyFence();
        }

        public void EjectStones()
        {
            if (_LeftBattlePetStone != null)
            {
                _LeftBattlePetStoneFrom.AddToBackpack(_LeftBattlePetStone);
                _LeftBattlePetStoneFrom.SendMessage("Your Battle Pet Stone has been returned to you.");
                _LeftBattlePetStone = null;
                _LeftBattlePetStoneFrom = null;
            }
            if (_RightBattlePetStone != null)
            {
                _RightBattlePetStoneFrom.AddToBackpack(_RightBattlePetStone);
                _RightBattlePetStoneFrom.SendMessage("Your Battle Pet Stone has been returned to you.");
                _RightBattlePetStone = null;
                _RightBattlePetStoneFrom = null;
            }
        }

        public void ClearCorpses()
        {
            var items = this.GetItemsInRange(10);
            var corpses = items?.Where(x => x.GetType() == typeof(Corpse)).ToList()??new List<Item>();
            foreach (var corpse in corpses)
            {
                foreach (var item in corpse.Items.ToList())
                {
                    item.Delete();
                }
                corpse.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
            writer.Write(_LeftBattlePetStone != null);
            writer.Write(_RightBattlePetStone != null);
            writer.Write(_LeftBattlePet != null);
            writer.Write(_RightBattlePet != null);
            writer.Write(_LeftBattlePetStoneFrom != null);
            writer.Write(_RightBattlePetStoneFrom != null);
            writer.Write(Fence.Count);
            if (_LeftBattlePetStone != null) writer.WriteItem(_LeftBattlePetStone);
            if (_RightBattlePetStone != null) writer.WriteItem(_RightBattlePetStone);
            if (_LeftBattlePet != null) writer.Write(_LeftBattlePet.Serial);
            if (_RightBattlePet != null) writer.Write(_RightBattlePet.Serial);
            foreach (var blocker in Fence)
            {
                writer.Write(blocker);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            Fence = new List<int>();
            base.Deserialize(reader);
            var version = reader.ReadInt();
            var leftExists = reader.ReadBool();
            var rightExists = reader.ReadBool();
            var leftPetExists = reader.ReadBool();
            var rightPetExists = reader.ReadBool();
            var leftPetFromExists = reader.ReadBool();
            var rightPetFromExists = reader.ReadBool();
            var fenceCount = reader.ReadInt();
            if(leftExists) _LeftBattlePetStone = reader.ReadItem<BattlePetStone>();
            if(rightExists) _RightBattlePetStone = reader.ReadItem<BattlePetStone>();
            if (leftPetExists) _LeftBattlePet = (BattlePet) World.FindMobile(reader.ReadInt());
            if (rightPetExists) _RightBattlePet = (BattlePet) World.FindMobile(reader.ReadInt());
            if (leftPetFromExists) _LeftBattlePetStoneFrom = World.FindMobile(reader.ReadInt());
            if (rightPetFromExists) _RightBattlePetStoneFrom = World.FindMobile(reader.ReadInt());
            for (var b = 0; b < fenceCount; b++)
            {
                Fence.Add(reader.ReadInt());
            }
        }
    }
}

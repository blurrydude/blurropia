using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    public class BattlePetStone : Item
    {
        [Constructable]
        public BattlePetStone() : base(3699)
        {
            Enhancements = new List<BattlePetStoneEnhancement>();
            _Level = 1;
            _AIType = AIType.AI_Melee;
            _BattlePetBody = BattlePetBody.Squirrel;
            Hue = 35 + _Level * 5;
            Name = $"Level {_Level} {GetAiName()} {_BattlePetBody}";
        }

        [Constructable]
        public BattlePetStone(BattlePetBody battlePetBody) : base(3699)
        {
            Enhancements = new List<BattlePetStoneEnhancement>();
            _Level = 1;
            _AIType = AIType.AI_Melee;
            _BattlePetBody = battlePetBody;
            Hue = 35 + _Level * 5;
            Name = $"Level {_Level} {GetAiName()} {_BattlePetBody}";
        }
	
        public BattlePetStone(Serial serial) : base(serial) {
            Hue = 1910 + _Level;
            Name = $"Level {_Level} {GetAiName()} {_BattlePetBody}";
        }

        public List<BattlePetStoneEnhancement> Enhancements { get; set; }
	
        private AIType _AIType;
        [CommandProperty(AccessLevel.GameMaster)]
        public AIType AIType
        {
            get
            {
                return _AIType;
            }
            set
            {
                _AIType = value;
                Name = $"Level {_Level} {GetAiName()} {_BattlePetBody}";
            }
        }
	
        private Mobile _Owner;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                _Owner = value;
            }
        }
	
        private int _Level;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get
            {
                return _Level;
            }
            set
            {
                _Level = value;
                Name = $"Level {_Level} {GetAiName()} {_BattlePetBody}";
                Hue = 35 + _Level * 5;
            }
        }
	
        private int _Exp;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Exp
        {
            get
            {
                return _Exp;
            }
            set
            {
                var nextLevel = Math.Pow(2, _Level + 2);
                if (value > nextLevel)
                {
                    Level = _Level + 1;
                }
                _Exp = value;
            }
        }

        public int ExpValue => (int)Math.Pow(2, _Level);
	
        private BattlePetBody _BattlePetBody;
        [CommandProperty(AccessLevel.GameMaster)]
        public BattlePetBody BattlePetBody
        {
            get
            {
                return _BattlePetBody;
            }
            set
            {
                _BattlePetBody = value;
                Name = $"Level {_Level} {GetAiName()} {_BattlePetBody}";
            }
        }
	
        public int Str => 10 + (_Level * 10) + Enhancements.Sum(x => x.StrMod);
	
        public int Int => 10 + (_Level * 10) + Enhancements.Sum(x => x.IntMod);
	
        public int Dex => 10 + (_Level * 10) + Enhancements.Sum(x => x.DexMod);
	
        public int Armor => 10 + (_Level * 5) + Enhancements.Sum(x => x.ArmorMod);

        public int Damage => 5 + (_Level * 5) + Enhancements.Sum(x => x.DamageMod);

        public override void OnDoubleClick(Mobile @from)
        {
            if (_Owner == null)
            {
                _Owner = from;
                from.SendMessage("This Battle Pet Stone belongs to you now.");
                return;
            }
            if (from != _Owner)
            {
                from.SendMessage("This Battle Pet Stone does not belong to you.");
                return;
            }

            from.SendGump(new BattlePetGump(this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
            writer.Write(_Owner?.Serial??0);
            writer.Write((int)_AIType);
            writer.Write((int)_BattlePetBody);
            writer.Write(_Level);
            writer.Write(_Exp);
            writer.Write(Enhancements.Count);
            foreach (var enhancement in Enhancements)
            {
                writer.WriteItem(enhancement);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            Enhancements = new List<BattlePetStoneEnhancement>();
            base.Deserialize(reader);
            var version = reader.ReadInt();
            var ownerSerial = reader.ReadInt();
            _Owner = ownerSerial == 0 ? null : World.FindMobile(ownerSerial);
            _AIType = (AIType)reader.ReadInt();
            _BattlePetBody = (BattlePetBody)reader.ReadInt();
            _Level = reader.ReadInt();
            _Exp = reader.ReadInt();
            var count = reader.ReadInt();
            for (var i = 0; i < count; i++)
            {
                Enhancements.Add(reader.ReadItem<BattlePetStoneEnhancement>());
            }
        }

        private string GetAiName()
        {
            return _AIType == AIType.AI_Melee ? "Battle" : _AIType.ToString().Replace("AI_", string.Empty);
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Server.Customs;
using Server.Items;
using Server.Mobiles;

namespace Server.Customs.JsonSystem
{
    public enum ArenaFighterType
    {
        Healer,
        Fencer,
        Swordsman,
        Macer,
        Supermage,
        Assassin
    }

    public class ArenaFighter
    {
        public int Index { get; set; }
        public Serial Serial { get; set; }
        public ArenaFighterType FighterType { get; set; }
        public int Team { get;set; }
    }

    public class ArenaBattleWager
    {
        public int Team { get; set; }
        public int Amount { get; set; }
        public Serial Serial { get; set; }
    }

    public static class ArenaUtility
    {
        public static List<Point2D> ArenaStoneField()
        {
            var list = new List<Point2D>();
            for (var x = 1; x <= 16; x++)
            {
                for (var y = 1; y <= 8; y++)
                {
                    list.Add(new Point2D(x,y));
                }
            }

            return list;
        }
    }

    public class BaseArenaFighter : BaseCreature
    {
        public ArenaBattleStone ControlStone { get; set; }
        public int TeamHue => 20 + Team * 40;
        public BaseArenaFighter(AIType iAI, ArenaBattleStone controlStone)
            : base(iAI, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            ControlStone = controlStone;
        }

        public BaseArenaFighter(Serial serial) : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {

            base.OnDeath(c);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
            writer.Write(ControlStone.Serial);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
            ControlStone = (ArenaBattleStone)World.Items[reader.ReadInt()];
        }
    }

    public class ArenaSwordsman : BaseArenaFighter
    {
        public ArenaSwordsman(ArenaBattleStone controlStone)
            : base(AIType.AI_Melee, controlStone)
        {
        }

        public ArenaSwordsman(Serial serial) : base(serial)
        {
        }
    }
}

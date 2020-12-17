using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    /*
    public class BattlePetSquirrel : BattlePet
    {
        
        [Constructable]
        public BattlePetSquirrel() : base(AIType.AI_Melee)
        {
            this.Name = "a squirrel";
            this.Body = 0x116;

            this.SetStr(44, 50);
            this.SetDex(35);
            this.SetInt(5);

            this.SetHits(42, 50);

            this.SetDamage(1, 2);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 34);
            this.SetResistance(ResistanceType.Fire, 10, 14);
            this.SetResistance(ResistanceType.Cold, 30, 35);
            this.SetResistance(ResistanceType.Poison, 20, 25);
            this.SetResistance(ResistanceType.Energy, 20, 25);

            this.SetSkill(SkillName.MagicResist, 4.0);
            this.SetSkill(SkillName.Tactics, 4.0);
            this.SetSkill(SkillName.Wrestling, 4.0);
        }
        public BattlePetSquirrel(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
        }
    }
    */
}

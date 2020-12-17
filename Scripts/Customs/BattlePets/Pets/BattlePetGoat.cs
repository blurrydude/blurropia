using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    /*
    public class BattlePetGoat : BattlePet
    {
        
        [Constructable]
        public BattlePetGoat() : base(AIType.AI_Melee)
        {
            this.Name = "a goat";
            this.Body = 0xD1;
            this.BaseSoundID = 0x99;

            this.SetStr(19);
            this.SetDex(15);
            this.SetInt(5);

            this.SetHits(12);
            this.SetMana(0);

            this.SetDamage(3, 4);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 15);

            this.SetSkill(SkillName.MagicResist, 5.0);
            this.SetSkill(SkillName.Tactics, 5.0);
            this.SetSkill(SkillName.Wrestling, 5.0);

            this.Fame = 150;
            this.Karma = 0;

            this.VirtualArmor = 10;
        }
        public BattlePetGoat(Serial serial) : base(serial)
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

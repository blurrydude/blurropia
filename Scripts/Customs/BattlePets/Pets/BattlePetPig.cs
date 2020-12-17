using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    /*
    public class BattlePetPig : BattlePet
    {
        
        [Constructable]
        public BattlePetPig() : base(AIType.AI_Melee)
        {
            this.Name = "a pig";
            this.Body = 0xCB;
            this.BaseSoundID = 0xC4;

            this.SetStr(20);
            this.SetDex(20);
            this.SetInt(5);

            this.SetHits(12);
            this.SetMana(0);

            this.SetDamage(2, 4);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 10, 15);

            this.SetSkill(SkillName.MagicResist, 5.0);
            this.SetSkill(SkillName.Tactics, 5.0);
            this.SetSkill(SkillName.Wrestling, 5.0);

            this.Fame = 150;
            this.Karma = 0;

            this.VirtualArmor = 12;
        }
        public BattlePetPig(Serial serial) : base(serial)
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

using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    /*
    public class BattlePetOstard : BattlePet
    {
        
        [Constructable]
        public BattlePetOstard() : base(AIType.AI_Melee)
        {
            this.BaseSoundID = 0x270;
            this.Body = 0xD2;

            this.SetStr(94, 170);
            this.SetDex(56, 75);
            this.SetInt(6, 10);

            this.SetHits(71, 88);
            this.SetMana(0);

            this.SetDamage(5, 11);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Fire, 5, 15);

            this.SetSkill(SkillName.MagicResist, 25.1, 30.0);
            this.SetSkill(SkillName.Tactics, 25.3, 40.0);
            this.SetSkill(SkillName.Wrestling, 29.3, 44.0);

            this.Fame = 450;
            this.Karma = 0;
        }
        public BattlePetOstard(Serial serial) : base(serial)
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

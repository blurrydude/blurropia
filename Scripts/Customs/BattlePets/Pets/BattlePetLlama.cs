using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    /*
    public class BattlePetLlama : BattlePet
    {
        
        [Constructable]
        public BattlePetLlama() : base(AIType.AI_Melee)
        {
            this.Name = "a llama";
            this.Body = 0xDC;
            this.BaseSoundID = 0x3F3;

            this.SetStr(21, 49);
            this.SetDex(36, 55);
            this.SetInt(16, 30);

            this.SetHits(15, 27);
            this.SetMana(0);

            this.SetDamage(3, 5);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);

            this.SetSkill(SkillName.MagicResist, 15.1, 20.0);
            this.SetSkill(SkillName.Tactics, 19.2, 29.0);
            this.SetSkill(SkillName.Wrestling, 19.2, 29.0);

            this.Fame = 300;
            this.Karma = 0;

            this.VirtualArmor = 16;
        }
        public BattlePetLlama(Serial serial) : base(serial)
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

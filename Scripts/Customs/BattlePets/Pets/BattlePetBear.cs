using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    /*
    public class BattlePetBear : BattlePet
    {
        
        [Constructable]
        public BattlePetBear() : base(AIType.AI_Melee)
        {
            this.Name = "a polar bear";
            this.Body = 213;
            this.BaseSoundID = 0xA3;

            this.SetStr(116, 140);
            this.SetDex(81, 105);
            this.SetInt(26, 50);

            this.SetHits(70, 84);
            this.SetMana(0);

            this.SetDamage(7, 12);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Cold, 60, 80);
            this.SetResistance(ResistanceType.Poison, 15, 25);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.MagicResist, 45.1, 60.0);
            this.SetSkill(SkillName.Tactics, 60.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 45.1, 70.0);

            this.Fame = 1500;
            this.Karma = 0;

            this.VirtualArmor = 18;
        }
        public BattlePetBear(Serial serial) : base(serial)
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

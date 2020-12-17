using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    /*
    public class BattlePetWalrus : BattlePet
    {
        
        [Constructable]
        public BattlePetWalrus() : base(AIType.AI_Melee)
        {
            this.Name = "a walrus";
            this.Body = 0xDD;
            this.BaseSoundID = 0xE0;

            this.SetStr(21, 29);
            this.SetDex(46, 55);
            this.SetInt(16, 20);

            this.SetHits(14, 17);
            this.SetMana(0);

            this.SetDamage(4, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 20, 25);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.MagicResist, 15.1, 20.0);
            this.SetSkill(SkillName.Tactics, 19.2, 29.0);
            this.SetSkill(SkillName.Wrestling, 19.2, 29.0);

            this.Fame = 150;
            this.Karma = 0;

            this.VirtualArmor = 18;
        }
        public BattlePetWalrus(Serial serial) : base(serial)
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

using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    /*
    public class BattlePetChicken : BattlePet
    {
        
        [Constructable]
        public BattlePetChicken() : base(AIType.AI_Melee)
        {
            Name = "a chicken";
            Body = 0xD0;
            BaseSoundID = 0x6E;

            SetStr(5);
            SetDex(15);
            SetInt(35);

            SetHits(3);
            SetMana(0);

            SetDamage(1);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 1, 5);

            SetSkill(SkillName.MagicResist, 4.0);
            SetSkill(SkillName.Tactics, 5.0);
            SetSkill(SkillName.Wrestling, 5.0);
            
            VirtualArmor = 2;
            
            AddSpellAttack(typeof(Spells.First.MagicArrowSpell));
            AddSpellDefense(typeof(Spells.First.HealSpell));
        }
        public BattlePetChicken(Serial serial) : base(serial)
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

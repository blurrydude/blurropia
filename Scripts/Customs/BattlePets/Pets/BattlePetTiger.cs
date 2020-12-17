using Server.Items;
using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    /*
    public class BattlePetTiger : BattlePet
    {
        
        [Constructable]
        public BattlePetTiger() : base(AIType.AI_Melee)
        {
            Body = 1254;
            SetStr(496, 554);
            SetDex(88, 124);
            SetInt(94, 163);

            SetHits(352, 450);

            SetDamage(18, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 56, 75);
            SetResistance(ResistanceType.Fire, 21, 40);
            SetResistance(ResistanceType.Cold, 55, 64);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 90.8, 97.5);
            SetSkill(SkillName.Anatomy, 0);
            SetSkill(SkillName.Tactics, 100.2, 102.5);
            SetSkill(SkillName.Wrestling, 90.1, 94.4);

            Fame = 11000;
            Karma = -11000;

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetSpecialAbility(SpecialAbility.GraspingClaw);
        }
        public BattlePetTiger(Serial serial) : base(serial)
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

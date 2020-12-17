using Server.Items;
using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    /*
    public class BattlePetFox : BattlePet
    {
        
        [Constructable]
        public BattlePetFox() : base(AIType.AI_Melee)
        {
            Name = "a fox";
            Body = 0x58f;
            Female = true;

            SetStr(300, 320);
            SetDex(190, 200);
            SetInt(170, 210);

            SetHits(190, 200);

            SetDamage(16, 22);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 35);

            SetSkill(SkillName.MagicResist, 40.0, 50.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 75.0, 90.0);
            SetSkill(SkillName.DetectHidden, 50.0, 60.0);

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 72.0;

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetSpecialAbility(SpecialAbility.GraspingClaw);
        }
        public BattlePetFox(Serial serial) : base(serial)
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

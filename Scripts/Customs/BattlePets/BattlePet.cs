using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Mobiles;

namespace Server.Customs.BattlePets
{
    public class BattlePet : BaseCreature
    {
        private BattlePetControlStone _ControlStone;
        public BattlePet(BattlePetControlStone controlStone, BattlePetBody body, AIType iAi, int str, int @int, int dex, int arm, int dam, int hue, List<BattlePetStoneEnhancement> enhancements) : base(iAi, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            _ControlStone = controlStone;
            SetBody(body);
            SetStr(str);
            SetInt(@int);
            SetDex(dex);
            VirtualArmor = arm;
            SetDamage(dam);
            Hue = hue;
            var skills = new Dictionary<SkillName, double>();
            foreach (var enhancement in enhancements.Where(x => x.EnhancementType == BattlePetEnhancementType.Skill))
            {
                if (!skills.ContainsKey(enhancement.Skill))
                {
                    skills[enhancement.Skill] = 0;
                }
                skills[enhancement.Skill] += enhancement.SkillMod;
            }

            foreach (var skill in skills)
            {
                SetSkill(skill.Key, skill.Value);
            }

            foreach (var enhancement in enhancements.Where(x => x.EnhancementType == BattlePetEnhancementType.Spell))
            {
                var spell = enhancement.Spell;
                if (spell.ValidateBeneficial(this))
                {
                    AddSpellDefense(spell.GetType());
                }
                else
                {
                    AddSpellAttack(spell.GetType());
                }
            }
        }

        public BattlePet(Serial serial) : base(serial)
        {
        }

        public void SetBody(BattlePetBody body)
        {
            switch (body)
            {
                case BattlePetBody.Bear: Body = 213; BaseSoundID = 0xA3; break;
                case BattlePetBody.Chicken: Body = 0xD0; BaseSoundID = 0x6E; break;
                case BattlePetBody.Fox: Body = 0x58f; break;
                case BattlePetBody.Frog: Body = 81; BaseSoundID = 0x266; break;
                case BattlePetBody.Goat: Body = 0xD1; BaseSoundID = 0x99; break;
                case BattlePetBody.Llama: Body = 0xDC; BaseSoundID = 0x3F3; break;
                case BattlePetBody.Ostard: Body = 0xD2; BaseSoundID = 0x270; break;
                case BattlePetBody.Pig: Body = 0xCB; BaseSoundID = 0xC4; break;
                case BattlePetBody.Squirrel: Body = 0x116; break;
                case BattlePetBody.Tiger: Body = 1254; break;
                case BattlePetBody.Walrus: Body = 0xDD; BaseSoundID = 0xE0; break;
            }
        }

        public override void OnDeath(Container c)
        {
            _ControlStone.OnBattlePetDeath(this.Serial);
            base.OnDeath(c);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
            writer.Write(_ControlStone.Serial);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
            _ControlStone = (BattlePetControlStone)World.FindItem(reader.ReadInt());
        }
    }
}

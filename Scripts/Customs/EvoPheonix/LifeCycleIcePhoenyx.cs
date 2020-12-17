   
    //////////////////////////////////////////
   //  LifeCycle IcePhoenyx (New Type) by  //
  //  IcePhoenyxRising of Runuo / Servuo  //
 //  Created from evo phoenix by Raelis  //
//////////////////////////////////////////

using System;
using System.Collections; 
using Server.Mobiles;
using Server.Items;
using Server.Network; 
using Server.Targeting;
using Server.Gumps;

namespace Server.Mobiles
{
    [CorpseName("a Ice Phoenyx corpse")]
    public class LifeCycleIcePhoenyx : BaseCreature
    {
        public int m_Stage;
        public int m_KP;

        public bool m_S1;
        public bool m_S2;
        public bool m_S3;
        public bool m_S4;
        public bool m_S5;
        public bool m_S6;

        public bool S1
        {
            get { return m_S1; }
            set { m_S1 = value; }
        }

        public bool S2
        {
            get { return m_S2; }
            set { m_S2 = value; }
        }

        public bool S3
        {
            get { return m_S3; }
            set { m_S3 = value; }
        }

        public bool S4
        {
            get { return m_S4; }
            set { m_S4 = value; }
        }

        public bool S5
        {
            get { return m_S5; }
            set { m_S5 = value; }
        }

        public bool S6
        {
            get { return m_S6; }
            set { m_S6 = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int KP
        {
            get { return m_KP; }
            set { m_KP = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Stage
        {
            get { return m_Stage; }
            set { m_Stage = value; }
        }

        [Constructable]
        public LifeCycleIcePhoenyx()
            : base(AIType.AI_NecroMage, FightMode.Weakest, 10, 1, 0.1, 0.2)
        {
            Name = "a ice phoenyx";
            Body = 6;
            BaseSoundID = 0x8F;
            Stage = 1;
            Hue = 2591;
            Female = Utility.RandomBool();
            this.Tamable = true;



            S1 = true;
            S2 = true;
            S3 = true;
            S4 = true;
            S5 = true;
            S6 = true;

            this.Hue = 2591;
            this.SetResistance(ResistanceType.Physical, 75);
            this.SetResistance(ResistanceType.Fire, 75);
            this.SetResistance(ResistanceType.Cold, 75);
            this.SetResistance(ResistanceType.Poison, 75);
            this.SetResistance(ResistanceType.Energy, 75);

            SetSkill(SkillName.MagicResist, 90.4, 97.8);
            SetSkill(SkillName.Tactics, 99.0, 99.5);
            SetSkill(SkillName.Wrestling, 84.2, 87.7);
            SetSkill(SkillName.EvalInt, 84.2, 97.7);
            SetSkill(SkillName.Magery, 94.0, 99.5);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.Anatomy, 84.2, 97.7);
            SetStr(696, 750);
            SetDex(288, 325);
            SetInt(395, 450);
            SetHits(950, 1125);
            SetDamage(22, 30);
            SetDamageType(ResistanceType.Energy, 50);
            SetDamageType(ResistanceType.Cold, 50);
            ControlSlots = 2;
            ControlSlotsMax = 4;
            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetWeaponAbility(WeaponAbility.ColdWind);
            // SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override bool IsScaredOfScaryThings => false;
        public override bool BleedImmune => true;
        public override bool BardImmune => Controlled;
        public override Poison PoisonImmune => Poison.Lethal;

        public override int Hides => 10;
        //public override int GetAngerSound()
        //{
        //   return 541;
        //}

        public override int Meat
        {
            get { return 1; }
        }

        public override MeatType MeatType
        {
            get { return MeatType.Bird; }
        }

        public override int Feathers
        {
            get { return (Body == 6 ? 15 : 36); }
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.Meat | FoodType.Fish; }
        }


        public LifeCycleIcePhoenyx(Serial serial) : base(serial)
        {
        }

        private void KpGain(Mobile defender)
        {
            if (!(defender is BaseCreature) || ((BaseCreature) defender).Controlled) return;
            var kpgainmatrix = new[]
            {
                new[] {3, 1}, new[] {5, 2}, new[] {7, 3}, new[] {9, 4}, new[] {10, 5}, new[] {10, 5}
            }; // could be static or loaded externally
            var b = 5 + ((BaseCreature) defender).HitsMax;
            KP += Utility.RandomList(b + kpgainmatrix[Stage - 1][0], b + kpgainmatrix[Stage - 1][1]);
        }

        private void EvoCheck()
        {
            var stageindex = Stage - 1;
            var stagelimit = new[] {5000, 25000, 75000, 150000, 225000, 750000}; // could be static or loaded externally
            if (KP < stagelimit[stageindex]) return; // no point in continuing to use memory if we don't need to
            var stagebody = new[] {6, 831, 5, 254, 243, 832}; // could be static or loaded externally
            var evomessage = new[] // could be static or loaded externally
            {
                "*{0} has shattered into a cloud of frost to been reborn*",
                "*{0} has become a fledgling Ice Pheonix*",
                "*{0} has become a stronger Ice Pheonix*",
                "*{0} has become an adolescent Ice Pheonix*",
                "*{0} has become an adult Ice Pheonix*",
                "*{0} has become a legendary Ice Pheonix*"
            };
            var evotitle = new[] // could be static or loaded externally
            {
                "", "", "", "", "",
                "The Legendary Ice Pheonix"
            };
            var evodrop = new[] // could be static or loaded externally
            {
                new Item[] {new phoenixtears()},
                new Item[] {new phoenixtears()},
                new Item[] {new phoenixtears()},
                new Item[] {new phoenixtears()},
                new Item[] {new phoenixtears(), new phoenixash()},
                new Item[] {new phoenixtears()}
            };
            var evosound = new int?[]
            {
                null, null, null, null, null, 0x208
            };
            var evoeffect = new[]
            {
                null, null, null, null, null,
                new EffectConfig(0x3709, 10, 30, 5052)
            };
            Stage = Stage == 6 ? 1 : Stage + 1; // progress the stage
            stageindex = Stage - 1; // redundant, but we need it again, so might as well reassign it
            Say(String.Format(evomessage[stageindex], Name));
            BodyValue = stagebody[stageindex];
            foreach (var item in evodrop[stageindex])
            {
                AddToBackpack(item);
            }

            Title = evotitle[stageindex];
            if (evosound[stageindex] != null) PlaySound((int) evosound[stageindex]);
            if (evoeffect[stageindex] != null)
            {
                FixedParticles(evoeffect[stageindex].ItemId, evoeffect[stageindex].Speed,
                    evoeffect[stageindex].Duration,
                    evoeffect[stageindex].Effect, EffectLayer.Waist);
            }

            // not sure why we have to have a bool for every stage, but I'll leave this here
            S1 = Stage == 1;
            S2 = Stage == 2;
            S3 = Stage == 3;
            S4 = Stage == 4;
            S5 = Stage == 5;
            S6 = Stage == 6;
        }

        private class EffectConfig
        {
            public EffectConfig(int itemid, int speed, int duration, int effect)
            {
                ItemId = itemid;
                Speed = speed;
                Duration = duration;
                Effect = effect;
            }

            public int ItemId { get; set; }
            public int Speed { get; set; }
            public int Duration { get; set; }
            public int Effect { get; set; }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            KpGain(defender);
            EvoCheck();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            {
                writer.Write((int) 0);

                writer.Write(m_S1);
                writer.Write(m_S2);
                writer.Write(m_S3);
                writer.Write(m_S4);
                writer.Write(m_S5);
                writer.Write(m_S6);
                writer.Write((int) m_KP);
                writer.Write((int) m_Stage);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetWeaponAbility(WeaponAbility.ColdWind);

            m_S1 = reader.ReadBool();
            m_S2 = reader.ReadBool();
            m_S3 = reader.ReadBool();
            m_S4 = reader.ReadBool();
            m_S5 = reader.ReadBool();
            m_S6 = reader.ReadBool();
            m_KP = reader.ReadInt();
            m_Stage = reader.ReadInt();
        }
    }
}

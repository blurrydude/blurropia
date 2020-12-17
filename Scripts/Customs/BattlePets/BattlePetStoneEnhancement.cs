using System;
using System.Linq;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Server.Customs.BattlePets
{
    public class BattlePetStoneEnhancement : Item
    {
        [Constructable]
        public BattlePetStoneEnhancement() : base(7847)
        {
            Name = "Battle Pet Enhancement";
            CreateRandomEnhancement();
            Hue = 137 + (int)Rarity * 10;
        }
	
        public BattlePetStoneEnhancement(Serial serial) : base(serial) {
            Hue = 137 + (int)Rarity * 10;
        }

        private Rarity _Rarity;
        [CommandProperty(AccessLevel.GameMaster)]
        public Rarity Rarity
        {
            get { return _Rarity; }
            set { _Rarity = value; }
        }
	
        private int _StrMod;
        [CommandProperty(AccessLevel.GameMaster)]
        public int StrMod
        {
            get { return _StrMod; }
            set { _StrMod = value; }
        }
	
        private int _IntMod;
        [CommandProperty(AccessLevel.GameMaster)]
        public int IntMod
        {
            get { return _IntMod; }
            set { _IntMod = value; }
        }
	
        private int _DexMod;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DexMod
        {
            get { return _DexMod; }
            set { _DexMod = value; }
        }
	
        private int _ArmorMod;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ArmorMod
        {
            get { return _ArmorMod; }
            set { _ArmorMod = value; }
        }
	
        private int _DamageMod;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DamageMod
        {
            get { return _DamageMod; }
            set { _DamageMod = value; }
        }
	
        private int _SkillMod;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SkillMod
        {
            get { return _SkillMod; }
            set { _SkillMod = value; }
        }
	
        private Spell _Spell;
        [CommandProperty(AccessLevel.GameMaster)]
        public Spell Spell
        {
            get { return _Spell; }
            set { _Spell = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string SpellName
        {
            get { return _Spell.Name; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpellId
        {
            get { return _Spell?.ID??0; }
            set { _Spell = value == 0 ? null : SpellRegistry.NewSpell(value, null, null); }
        }

        private SkillName _Skill;
        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get { return _Skill; }
            set { _Skill = value; }
        }
	
        /*[CommandProperty(AccessLevel.GameMaster)]
        public WeaponAbility WeaponAbility { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SpecialAbility SpecialAbility { get; set; }*/

        [CommandProperty(AccessLevel.GameMaster)]
        public BattlePetEnhancementType EnhancementType { get; set; }

        public string ModName => StrMod > 0 ? $"Strength {StrMod}" :
            IntMod > 0 ? $"Intelligence {IntMod}" :
            DexMod > 0 ? $"Dexterity {DexMod}" :
            ArmorMod > 0 ? $"Armor {ArmorMod}" :
            DamageMod > 0 ? $"Damage {DamageMod}" :
            SkillMod > 0 ? $"{Skill.ToString()} {SkillMod}" :
            Spell != null ? $"{Spell.Name}" : "BAD";

        public override void AddNameProperties(ObjectPropertyList list)
        {
            list.Add($"{Rarity} Battle Pet Enhancement");
            list.Add(ModName);
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.InRange( this.GetWorldLocation(), 2 ) == false )
            {
                from.SendLocalizedMessage( 500486 );	//That is too far away.
            }
            else
            {
                from.Target=new BattlePetStoneTarget( this );
                from.SendMessage( "Target your Battle Pet Stone" );
            }
        }

        private class BattlePetStoneTarget : Target
        {
            private BattlePetStoneEnhancement _EnhancementStone;

            public BattlePetStoneTarget(BattlePetStoneEnhancement controlStone) : base( 3, false, TargetFlags.None )
            {
                _EnhancementStone = controlStone;
            }
			
            protected override void OnTarget( Mobile from, object targ )
            {

                if (!(targ is BattlePetStone))
                {
                    from.SendMessage("That is not a Battle Pet Stone");
                    return;
                }

                var bps = (BattlePetStone) targ;
                if (bps.Owner == null)
                {
                    from.SendMessage("That Battle Pet Stone has no owner.");
                    return;
                }

                if (bps.Enhancements.Count(x => x.EnhancementType == _EnhancementStone.EnhancementType) >= 5)
                {
                    from.SendMessage("That stone already has five of that enhancement type.");
                    return;
                }

                bps.Enhancements.Add(_EnhancementStone);
                from.SendMessage("Battle Pet enhancement installed.");
                _EnhancementStone.Internalize();
                if (bps.Owner.HasGump(typeof(BattlePetGump)))
                {
                    bps.Owner.CloseGump(typeof(BattlePetGump));
                    bps.Owner.SendGump(new BattlePetGump(bps));
                }
                return;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
            writer.Write((int)_Rarity);
            writer.Write(_StrMod);
            writer.Write(_IntMod);
            writer.Write(_DexMod);
            writer.Write(_ArmorMod);
            writer.Write(_DamageMod);
            writer.Write(_SkillMod);
            writer.Write(_Spell?.ID??0);
            writer.Write((int)_Skill);
            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
            _Rarity = (Rarity) reader.ReadInt();
            _StrMod = reader.ReadInt();
            _IntMod = reader.ReadInt();
            _DexMod = reader.ReadInt();
            _ArmorMod = reader.ReadInt();
            _DamageMod = reader.ReadInt();
            _SkillMod = reader.ReadInt();
            var spellId = reader.ReadInt();
            _Spell = spellId == 0 ? null : SpellRegistry.NewSpell(spellId, null, null);
            _Skill = (SkillName) reader.ReadInt();
        }

        private void CreateRandomEnhancement()
        {
            int rarity;
            int armorChance;
            int damageChance;
            int abilityChance;
            int specialChance;
            int spellChance;
            int skillChance;
            int statChance;
            var roll = Utility.Random(0,1000);
            if (roll < 200)      { rarity = 0; armorChance = 65; damageChance = 35; abilityChance =  0; specialChance =  0; spellChance =  0; skillChance =  0; statChance =  0; } // Junk
            else if (roll < 700) { rarity = 1; armorChance = 60; damageChance = 35; abilityChance =  0; specialChance =  0; spellChance =  5; skillChance =  0; statChance =  0; } // Common
            else if (roll < 850) { rarity = 2; armorChance = 50; damageChance = 30; abilityChance =  0; specialChance =  0; spellChance = 10; skillChance = 10; statChance =  0; } // Uncommon
            else if (roll < 950) { rarity = 3; armorChance = 30; damageChance = 30; abilityChance =  0; specialChance =  0; spellChance = 20; skillChance = 20; statChance =  0; } // Rare
            else if (roll < 980) { rarity = 4; armorChance = 30; damageChance = 30; abilityChance =  0; specialChance =  0; spellChance = 20; skillChance = 18; statChance =  2; } // Legendary
            else if (roll < 995) { rarity = 5; armorChance = 30; damageChance = 25; abilityChance =  0; specialChance =  0; spellChance = 20; skillChance = 20; statChance =  5; } // Mythical
            else                 { rarity = 6; armorChance = 25; damageChance = 25; abilityChance =  0; specialChance =  0; spellChance = 20; skillChance = 20; statChance = 10; } // Divine
            Rarity = (Rarity)rarity;
            var typeRoll = Utility.Random(0, 100);
            if(typeRoll < armorChance) {MakeArmorEnhancement();}
            else if(typeRoll < armorChance + damageChance) {MakeDamageEnhancement();}
            //else if(typeRoll < armorChance + damageChance + abilityChance) {MakeWeaponEnhancement();}
            //else if(typeRoll < armorChance + damageChance + abilityChance + specialChance) {MakeSpecialEnhancement();}
            else if(typeRoll < armorChance + damageChance + abilityChance + specialChance + spellChance) {MakeSpellEnhancement();}
            else if(typeRoll < armorChance + damageChance + abilityChance + specialChance + spellChance + skillChance) {MakeSkillEnhancement();}
            else if(typeRoll < armorChance + damageChance + abilityChance + specialChance + spellChance + skillChance + statChance) {MakeStatEnhancement();}
        }

        private void MakeStatEnhancement()
        {
            var roll = Utility.Random(0, 4);
            var value = (int) Rarity - 3;
            switch (roll)
            {
                case 0: StrMod = value; break;
                case 1: IntMod = value; break;
                default: DexMod = value; break;
            }
            EnhancementType = BattlePetEnhancementType.Stat;
        }

        private void MakeSkillEnhancement()
        {
            var roll = Utility.Random(0,100);
            var valueRoll = Utility.Random(1,5);
            switch (Rarity)
            {
                case Rarity.Uncommon:

                    if(roll < 25) Skill = SkillName.Forensics;
                    else if(roll < 50) Skill = SkillName.Parry;
                    else if(roll < 75) Skill = SkillName.Meditation;
                    else Skill = SkillName.EvalInt;
                    break;
                case Rarity.Rare:
                    if(roll < 25) Skill = SkillName.Focus;
                    else if(roll < 50) Skill = SkillName.Tactics;
                    else if(roll < 75) Skill = SkillName.Anatomy;
                    else Skill = SkillName.MagicResist;
                    break;
                case Rarity.Legendary:
                    if(roll < 34) Skill = SkillName.Healing;
                    else if(roll < 67) Skill = SkillName.SpiritSpeak;
                    else Skill = SkillName.Wrestling;
                    break;
                case Rarity.Mythical:
                    if(roll < 34) Skill = SkillName.Bushido;
                    else if(roll < 67) Skill = SkillName.Ninjitsu;
                    else Skill = SkillName.Mysticism;
                    break;
                case Rarity.Divine:
                    if(roll < 40) Skill = SkillName.Necromancy;
                    else if(roll < 80) Skill = SkillName.Chivalry;
                    else Skill = SkillName.Magery;
                    break;
            }

            SkillMod = valueRoll * 5;
            EnhancementType = BattlePetEnhancementType.Skill;
        }

        private void MakeSpellEnhancement()
        {
            var roll = Utility.Random(0, 100);
            switch (Rarity)
            {
                case Rarity.Common:
                    if(roll < 12) Spell = new Spells.First.ClumsySpell(null, null);
                    else if(roll < 24) Spell = new Spells.First.FeeblemindSpell(null, null);
                    else if(roll < 36) Spell = new Spells.Second.HarmSpell(null, null);
                    else if(roll < 48) Spell = new Spells.Third.WallOfStoneSpell(null, null);
                    else if(roll < 60) Spell = new Spells.Necromancy.PainSpikeSpell(null, null);
                    else if(roll < 72) Spell = new Spells.Mysticism.EagleStrikeSpell(null, null);
                    else if(roll < 84) Spell = new Spells.Bushido.Confidence(null, null);
                    else Spell = new Spells.Bushido.CounterAttack(null, null);
                    break;
                case Rarity.Rare:
                    if(roll < 12) Spell = new Spells.First.HealSpell(null, null);
                    else if(roll < 24) Spell = new Spells.First.MagicArrowSpell(null, null);
                    else if(roll < 36) Spell = new Spells.Second.CureSpell(null, null);
                    else if(roll < 48) Spell = new Spells.Fifth.MindBlastSpell(null, null);
                    else if(roll < 60) Spell = new Spells.Necromancy.MindRotSpell(null, null);
                    else if(roll < 72) Spell = new Spells.Necromancy.StrangleSpell(null, null);
                    else if(roll < 84) Spell = new Spells.Mysticism.NetherBoltSpell(null, null);
                    else Spell = new Spells.Chivalry.DivineFurySpell(null, null);
                    break;
                case Rarity.Legendary:
                    if(roll < 12) Spell = new Spells.Third.FireballSpell(null, null);
                    else if(roll < 24) Spell = new Spells.Third.PoisonSpell(null, null);
                    else if(roll < 36) Spell = new Spells.Fourth.GreaterHealSpell(null, null);
                    else if(roll < 48) Spell = new Spells.Fourth.ManaDrainSpell(null, null);
                    else if(roll < 60) Spell = new Spells.Fifth.ParalyzeSpell(null, null);
                    else if(roll < 72) Spell = new Spells.Necromancy.PoisonStrikeSpell(null, null);
                    else if(roll < 84) Spell = new Spells.Necromancy.WitherSpell(null, null);
                    else Spell = new Spells.Chivalry.CloseWoundsSpell(null, null);
                    break;
                case Rarity.Mythical:
                    if(roll < 16) Spell = new Spells.Fourth.LightningSpell(null, null);
                    else if(roll < 32) Spell = new Spells.Sixth.EnergyBoltSpell(null, null);
                    else if(roll < 48) Spell = new Spells.Sixth.ExplosionSpell(null, null);
                    else if(roll < 64) Spell = new Spells.Seventh.ManaVampireSpell(null, null);
                    else if(roll < 80) Spell = new Spells.Mysticism.BombardSpell(null, null);
                    else Spell = new Spells.Chivalry.EnemyOfOneSpell(null, null);
                    break;
                case Rarity.Divine:
                    if(roll < 16) Spell = new Spells.Fifth.BladeSpiritsSpell(null, null);
                    else if(roll < 32) Spell = new Spells.Seventh.FlameStrikeSpell(null, null);
                    else if(roll < 48) Spell = new Spells.Seventh.MeteorSwarmSpell(null, null);
                    else if(roll < 64) Spell = new Spells.Eighth.EnergyVortexSpell(null, null);
                    else if(roll < 80) Spell = new Spells.Necromancy.VengefulSpiritSpell(null, null);
                    else Spell = new Spells.Ninjitsu.MirrorImage(null, null);
                    break;
            }

            EnhancementType = BattlePetEnhancementType.Spell;
        }

        /*private void MakeSpecialEnhancement()
        {
            var roll = Utility.Random(0, 100);
            switch (Rarity)
            {
                case Rarity.Rare:
                    if(roll < 34) SpecialAbility = SpecialAbility.Repel;
                    else if(roll < 67) SpecialAbility = SpecialAbility.TailSwipe;
                    else SpecialAbility = SpecialAbility.GraspingClaw;
                    break;
                case Rarity.Legendary:
                    if(roll < 34) SpecialAbility = SpecialAbility.Rage;
                    else if(roll < 67) SpecialAbility = SpecialAbility.FlurryForce;
                    else SpecialAbility = SpecialAbility.ManaDrain;
                    break;
                case Rarity.Mythical:
                    if(roll < 25) SpecialAbility = SpecialAbility.ViciousBite;
                    else if(roll < 50) SpecialAbility = SpecialAbility.ColossalRage;
                    else if(roll < 75) SpecialAbility = SpecialAbility.LifeDrain;
                    else SpecialAbility = SpecialAbility.PoisonSpit;
                    break;
                case Rarity.Divine:
                    if(roll < 25) SpecialAbility = SpecialAbility.VenomousBite;
                    else if(roll < 50) SpecialAbility = SpecialAbility.ColossalBlow;
                    else if(roll < 75) SpecialAbility = SpecialAbility.LifeLeech;
                    else SpecialAbility = SpecialAbility.DragonBreath;
                    break;
            }
        }

        private void MakeWeaponEnhancement()
        {
            var roll = Utility.Random(0, 100);
            switch (Rarity)
            {
                case Rarity.Rare:
                    if(roll < 25) WeaponAbility = WeaponAbility.Feint;
                    else if(roll < 50) WeaponAbility = WeaponAbility.Block;
                    else if(roll < 75) WeaponAbility = WeaponAbility.TalonStrike;
                    else WeaponAbility = WeaponAbility.BleedAttack;
                    break;
                case Rarity.Legendary:
                    if(roll < 25) WeaponAbility = WeaponAbility.ArmorPierce;
                    else if(roll < 50) WeaponAbility = WeaponAbility.ConcussionBlow;
                    else if(roll < 75) WeaponAbility = WeaponAbility.ForceOfNature;
                    else WeaponAbility = WeaponAbility.ColdWind;
                    break;
                case Rarity.Mythical:
                    if(roll < 25) WeaponAbility = WeaponAbility.NerveStrike;
                    else if(roll < 50) WeaponAbility = WeaponAbility.ShadowStrike;
                    else if(roll < 75) WeaponAbility = WeaponAbility.WhirlwindAttack;
                    else WeaponAbility = WeaponAbility.FrenziedWhirlwind;
                    break;
                case Rarity.Divine:
                    if(roll < 25) WeaponAbility = WeaponAbility.CrushingBlow;
                    else if(roll < 50) WeaponAbility = WeaponAbility.DoubleStrike;
                    else if(roll < 75) WeaponAbility = WeaponAbility.MortalStrike;
                    else WeaponAbility = WeaponAbility.InfectiousStrike;
                    break;
            }
        }*/

        private void MakeDamageEnhancement()
        {
            var value = Utility.Random(1, 3);
            DamageMod = value + (int)Rarity * 2;
            EnhancementType = BattlePetEnhancementType.Damage;
        }

        private void MakeArmorEnhancement()
        {
            var value = Utility.Random(1, 3);
            ArmorMod = value + (int)Rarity * 2;
            EnhancementType = BattlePetEnhancementType.Armor;
        }
    }
}

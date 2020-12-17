using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells.Necromancy;

namespace Server.Customs
{
    public class AvalonTrainingDummy : BaseCreature
    {
        public int CurrentlyCopying { get; set; }

        [Constructable] 
        public AvalonTrainingDummy()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        { 
            InitStats(31, 41, 51);
            /*Skills[SkillName.Swords].Base = 120;
            Skills[SkillName.Anatomy].Base = 120;
            Skills[SkillName.Healing].Base = 120;
            Skills[SkillName.Tactics].Base = 120;
            Skills[SkillName.Parry].Base = 120;*/

            SpeechHue = Utility.RandomDyedHue(); 

            Hue = Utility.RandomSkinHue(); 

            if (Female = Utility.RandomBool()) 
            { 
                Body = 0x191; 
                Name = NameList.RandomName("female");
            }
            else 
            { 
                Body = 0x190; 
                Name = NameList.RandomName("male");
            }

            Utility.AssignRandomHair(this);
            int iHue = 25 + Team * 40;
            
            Katana kat = new Katana();
            kat.Crafter = this;
            kat.Movable = true;
            kat.Quality = ItemQuality.Normal;
            AddItem(kat);

            Boots bts = new Boots();
            bts.Hue = iHue;
            AddItem(bts);

            ChainChest cht = new ChainChest();
            cht.Movable = false;
            cht.LootType = LootType.Newbied;
            cht.Crafter = this;
            cht.Quality = ItemQuality.Normal;
            AddItem(cht);

            ChainLegs chl = new ChainLegs();
            chl.Movable = false;
            chl.LootType = LootType.Newbied;
            chl.Crafter = this;
            chl.Quality = ItemQuality.Normal;
            AddItem(chl);

            PlateArms pla = new PlateArms();
            pla.Movable = false;
            pla.LootType = LootType.Newbied;
            pla.Crafter = this;
            pla.Quality = ItemQuality.Normal;
            AddItem(pla);

            Bandage band = new Bandage(50);
            AddToBackpack(band);
        }

        public AvalonTrainingDummy(Serial serial)
            : base(serial)
        { 
        }

        public void Dopplegang(Mobile from)
        {
            CurrentlyCopying = from.Serial;
            Str = from.Str;
            Dex = from.Dex;
            Int = from.Int;
            Say(false,"Ha! I am now as strong and smart as you are!");
            foreach (var skill in from.Skills)
            {
                var fromSkill = from.Skills[skill.SkillName].Base;
                //SetSkill(skill.SkillName,fromSkill);
                Skills[skill.SkillName].Base = fromSkill;
            }
            var madSkill = GetHighestSkillOfConsequence();
            Say(false,$"I am just as good at {madSkill} as you are!");
            SetAIType(madSkill);
            Say(false,"I can do what you can do!");
            Equip(madSkill);
            Say(false,"I have the tools to defeat you!");
            Hits = HitsMax;
            //TODO: recreate equipment based on skills
        }

        public SkillName GetHighestSkillOfConsequence()
        {
            var skillsOfConsequence = new System.Collections.Generic.Dictionary<int, double>
            {
                {(int)SkillName.Macing,Skills[SkillName.Macing].Base},
                {(int)SkillName.Fencing,Skills[SkillName.Fencing].Base},
                {(int)SkillName.Swords,Skills[SkillName.Swords].Base},
                {(int)SkillName.Magery,Skills[SkillName.Magery].Base},
                {(int)SkillName.Necromancy,Skills[SkillName.Necromancy].Base},
                {(int)SkillName.Wrestling,Skills[SkillName.Wrestling].Base},
                {(int)SkillName.Bushido,Skills[SkillName.Bushido].Base},
                {(int)SkillName.Ninjitsu,Skills[SkillName.Ninjitsu].Base},
                {(int)SkillName.Chivalry,Skills[SkillName.Chivalry].Base}
            };
            var highestSkill = new System.Collections.Generic.KeyValuePair<int,double>((int)SkillName.Wrestling,0);
            foreach (var skill in skillsOfConsequence)
            {
                if (skill.Value > highestSkill.Value)
                {
                    highestSkill = skill;
                }
            }

            return (SkillName) highestSkill.Key;

        }

        public void SetAIType(SkillName skill)
        {
            switch (skill)
            {
                case SkillName.Macing:
                case SkillName.Fencing:
                case SkillName.Swords:
                case SkillName.Wrestling:
                    AI = AIType.AI_Melee;
                    break;
                case SkillName.Magery:
                    AI = AIType.AI_Mage;
                    break;
                case SkillName.Necromancy:
                    AI = AIType.AI_Necro;
                    break;
                case SkillName.Bushido:
                    AI = AIType.AI_Samurai;
                    break;
                case SkillName.Ninjitsu:
                    AI = AIType.AI_Ninja;
                    break;
                case SkillName.Chivalry:
                    AI = AIType.AI_Paladin;
                    break;

            }
        }

        public void Equip(SkillName skill)
        {
            foreach (var item in Items.ToList())
            {
                RemoveItem(item);
            }

            switch (skill)
            {
                case SkillName.Macing:
                    EquipAsMacer();
                    break;
                case SkillName.Fencing:
                    EquipAsFencer();
                    break;
                case SkillName.Swords:
                    EquipAsSwordsman();
                    break;
                case SkillName.Wrestling:
                    EquipAsWrestler();
                    break;
                case SkillName.Magery:
                    EquipAsMage();
                    break;
                case SkillName.Necromancy:
                    EquipAsNecromancer();
                    break;
                case SkillName.Bushido:
                    EquipAsSamurai();
                    break;
                case SkillName.Ninjitsu:
                    EquipAsNinja();
                    break;
                case SkillName.Chivalry:
                    EquipAsPaladin();
                    break;

            }
        }

        public void EquipAsMacer()
        {
            WarHammer war = new WarHammer();
            war.Movable = true;
            war.Crafter = this;
            war.Quality = ItemQuality.Normal;
            AddItem(war);

            Boots bts = new Boots();
            bts.Hue = 25;
            AddItem(bts);

            ChainChest cht = new ChainChest();
            cht.Movable = false;
            cht.LootType = LootType.Newbied;
            cht.Crafter = this;
            cht.Quality = ItemQuality.Normal;
            AddItem(cht);

            ChainLegs chl = new ChainLegs();
            chl.Movable = false;
            chl.LootType = LootType.Newbied;
            chl.Crafter = this;
            chl.Quality = ItemQuality.Normal;
            AddItem(chl);

            PlateArms pla = new PlateArms();
            pla.Movable = false;
            pla.LootType = LootType.Newbied;
            pla.Crafter = this;
            pla.Quality = ItemQuality.Normal;
            AddItem(pla);

            Bandage band = new Bandage(50);
            AddToBackpack(band);
        }

        public void EquipAsFencer()
        {
            Spear ssp = new Spear();
            ssp.Movable = true;
            ssp.Crafter = this;
            ssp.Quality = ItemQuality.Normal;
            AddItem(ssp);

            Boots snd = new Boots();
            snd.Hue = 25;
            snd.LootType = LootType.Newbied;
            AddItem(snd);

            ChainChest cht = new ChainChest();
            cht.Movable = false;
            cht.LootType = LootType.Newbied;
            cht.Crafter = this;
            cht.Quality = ItemQuality.Normal;
            AddItem(cht);

            ChainLegs chl = new ChainLegs();
            chl.Movable = false;
            chl.LootType = LootType.Newbied;
            chl.Crafter = this;
            chl.Quality = ItemQuality.Normal;
            AddItem(chl);

            PlateArms pla = new PlateArms();
            pla.Movable = false;
            pla.LootType = LootType.Newbied;
            pla.Crafter = this;
            pla.Quality = ItemQuality.Normal;
            AddItem(pla);

            Bandage band = new Bandage(50);
            AddToBackpack(band);
        }

        public void EquipAsSwordsman()
        {
            Katana kat = new Katana();
            kat.Crafter = this;
            kat.Movable = true;
            kat.Quality = ItemQuality.Normal;
            AddItem(kat);

            Boots bts = new Boots();
            bts.Hue = 25;
            AddItem(bts);

            ChainChest cht = new ChainChest();
            cht.Movable = false;
            cht.LootType = LootType.Newbied;
            cht.Crafter = this;
            cht.Quality = ItemQuality.Normal;
            AddItem(cht);

            ChainLegs chl = new ChainLegs();
            chl.Movable = false;
            chl.LootType = LootType.Newbied;
            chl.Crafter = this;
            chl.Quality = ItemQuality.Normal;
            AddItem(chl);

            PlateArms pla = new PlateArms();
            pla.Movable = false;
            pla.LootType = LootType.Newbied;
            pla.Crafter = this;
            pla.Quality = ItemQuality.Normal;
            AddItem(pla);

            Bandage band = new Bandage(50);
            AddToBackpack(band);
        }

        public void EquipAsWrestler()
        {
            AddItem(new LeatherChest());
            AddItem(new LeatherArms());
            AddItem(new LeatherGloves());
            AddItem(new LeatherGorget());
            AddItem(new LeatherLegs());
        }

        public void EquipAsMage()
        {
            Spellbook book = new Spellbook();
            book.Movable = false;
            book.LootType = LootType.Newbied;
            book.Content = 0xFFFFFFFFFFFFFFFF;
            AddItem(book);

            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Newbied;
            lea.Crafter = this;
            lea.Quality = ItemQuality.Normal;
            AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Newbied;
            lec.Crafter = this;
            lec.Quality = ItemQuality.Normal;
            AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Newbied;
            leg.Crafter = this;
            leg.Quality = ItemQuality.Normal;
            AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Newbied;
            lel.Crafter = this;
            lel.Quality = ItemQuality.Normal;
            AddItem(lel);

            Robe robe = new Robe();
            robe.Hue = 25;
            AddItem(robe);

            Sandals snd = new Sandals();
            snd.Hue = 25;
            snd.LootType = LootType.Newbied;
            AddItem(snd);

            MagicWizardsHat jhat = new MagicWizardsHat();
            jhat.Hue = 25;
            AddItem(jhat);

            Doublet dblt = new Doublet();
            dblt.Hue = 25;
            AddItem(dblt);

            // Spells
            AddSpellAttack(typeof(Spells.First.MagicArrowSpell));
            AddSpellAttack(typeof(Spells.First.WeakenSpell));
            AddSpellAttack(typeof(Spells.Third.FireballSpell));
            AddSpellDefense(typeof(Spells.Third.WallOfStoneSpell));
            AddSpellDefense(typeof(Spells.First.HealSpell));
        }

        public void EquipAsNecromancer()
        {
            NecromancerSpellbook nsb = new NecromancerSpellbook();
            nsb.Movable = false;
            nsb.LootType = LootType.Newbied;
            nsb.Content = 0xFFFFFFFFFFFFFFFF;
            AddItem(nsb);
            
            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Newbied;
            lea.Crafter = this;
            lea.Quality = ItemQuality.Normal;
            AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Newbied;
            lec.Crafter = this;
            lec.Quality = ItemQuality.Normal;
            AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Newbied;
            leg.Crafter = this;
            leg.Quality = ItemQuality.Normal;
            AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Newbied;
            lel.Crafter = this;
            lel.Quality = ItemQuality.Normal;
            AddItem(lel);

            Robe robe = new Robe();
            robe.Hue = 25;
            AddItem(robe);

            Sandals snd = new Sandals();
            snd.Hue = 25;
            snd.LootType = LootType.Newbied;
            AddItem(snd);

            SkullCap skc = new SkullCap();
            skc.LootType = LootType.Newbied;
            AddItem(skc);

            Scythe scy = new Scythe();
            scy.LootType = LootType.Newbied;
            AddItem(scy);

            AddSpellAttack(typeof(Spells.Necromancy.PainSpikeSpell));
            AddSpellAttack(typeof(Spells.Necromancy.PoisonStrikeSpell));
            AddSpellAttack(typeof(Spells.Necromancy.VampiricEmbraceSpell));
            AddSpellDefense(typeof(Spells.Necromancy.SummonFamiliarSpell));
        }

        public void EquipAsSamurai()
        {
            
            switch ( Utility.Random(3) )
            {
                case 0:
                    this.AddItem(new Lajatang());
                    break;
                case 1:
                    this.AddItem(new Wakizashi());
                    break;
                case 2:
                    this.AddItem(new NoDachi());
                    break;
            }

            switch ( Utility.Random(3) )
            {
                case 0:
                    this.AddItem(new LeatherSuneate());
                    break;
                case 1:
                    this.AddItem(new PlateSuneate());
                    break;
                case 2:
                    this.AddItem(new StuddedHaidate());
                    break;
            }

            switch ( Utility.Random(4) )
            {
                case 0:
                    this.AddItem(new LeatherJingasa());
                    break;
                case 1:
                    this.AddItem(new ChainHatsuburi());
                    break;
                case 2:
                    this.AddItem(new HeavyPlateJingasa());
                    break;
                case 3:
                    this.AddItem(new DecorativePlateKabuto());
                    break;
            }

            this.AddItem(new LeatherDo());
            this.AddItem(new LeatherHiroSode());
            this.AddItem(new SamuraiTabi(Utility.RandomNondyedHue())); // TODO: Hue
        }

        public void EquipAsNinja()
        {
            if (!this.Female)
                this.AddItem(new LeatherNinjaHood());

            this.AddItem(new LeatherNinjaPants());
            this.AddItem(new LeatherNinjaBelt());
            this.AddItem(new LeatherNinjaJacket());
            this.AddItem(new NinjaTabi());
        }

        public void EquipAsPaladin()
        {
            switch (Utility.Random(5))
            {
                case 0: SetWearable(new Helmet()); break;
                case 1: SetWearable(new NorseHelm()); break;
                case 2: SetWearable(new PlateHelm()); break;
                case 3: SetWearable(new Bascinet()); break;
                case 4: SetWearable(new ChainCoif()); break;
            }

            SetWearable(new PlateLegs());
            SetWearable(new PlateArms());
            SetWearable(new PlateGloves());
            SetWearable(new PlateChest());
            SetWearable(new StuddedGorget());
            SetWearable(new VikingSword());
            SetWearable(new MetalKiteShield(), 1158);

            switch (Utility.Random(3))
            {
                case 0: SetWearable(new Tunic(), GetRandomHue()); break;
                case 1: SetWearable(new Doublet(), GetRandomHue()); break;
                case 2: SetWearable(new BodySash(), GetRandomHue()); break;
            }
        }
        public virtual int GetRandomHue()
        {
            switch (Utility.Random(5))
            {
                default:
                case 0:
                    return Utility.RandomBlueHue();
                case 1:
                    return Utility.RandomGreenHue();
                case 2:
                    return Utility.RandomRedHue();
                case 3:
                    return Utility.RandomYellowHue();
                case 4:
                    return Utility.RandomNeutralHue();
            }
        }
        
        public override bool HandlesOnSpeech(Mobile from)
        {
            return (from.Alive && InRange(from, 12));
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            switch (e.Speech)
            {
                case "I challenge thee":
                case "I challenge thee!":
                case "i challenge thee":
                case "i challenge thee!":
                case "Have at":
                case "have at":
                case "Have at thee":
                case "have at thee":
                case "Have at thee!":
                case "have at thee!":
                    Notice(e.Mobile);
                    break;
            }
        }

        private void Notice(Mobile from)
        {
            if (CurrentlyCopying != from.Serial)
            {
                Criminal = true;
                Dopplegang(from);
                //Aggressors.Add(AggressorInfo.Create(this,from,false));
                OnGotMeleeAttack(from);
            }
        }
        

        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt();
        }
    }
}

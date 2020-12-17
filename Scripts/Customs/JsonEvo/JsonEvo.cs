using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;
using Server.Mobiles;
using ServerUtilityExtensions;

namespace Server.Customs.JsonEvo
{
    [CorpseName("a corpse")]
    public class JsonEvo : BaseCreature
    {
        private string _fileName;
        private int _level;
        private int _experience;
        private bool _debugging;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Debugging
        {
            get => _debugging;
            set => _debugging = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string FileName
        {
            get => _fileName;
            set {
                _fileName = value;
                LoadFromFile();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Experience
        {
            get => _experience;
            set
            {
                _experience = value;
                EvoCheck();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get => _level;
            set
            {
                _level = Math.Max(Math.Min(Config?.Levels.Count ?? 1, value), 1);
                if (Config != null)
                {
                    var pl = _level == 1 ? Config.Levels.Count : _level - 1;
                    var xp = Config.Levels[pl - 1].ExpLimit;
                    Experience = xp;
                }
            }
        }

        public JsonEvoConfig Config { get; set; }

        [Constructable]
        public JsonEvo()
            : base(AIType.AI_NecroMage, FightMode.Weakest, 10, 1, 0.1, 0.2)
        {
            Name = "blank json evo";
            BodyValue = 400;
            Level = 1;
        }

        public JsonEvo(Serial serial) : base(serial)
        {

        }

        private void LoadFromFile()
        {
            if (String.IsNullOrEmpty(FileName)) return;
            try
            {
                var json = File.ReadAllText($"JsonEvo/{FileName}.json");
                Config = (JsonEvoConfig) JsonUtility.Deserialize<JsonEvoConfig>(json);
                if (Config.RandomGender) Female = Utility.RandomBool();
                if (!String.IsNullOrEmpty(Config.AI))
                {
                    AIType ai = AI;
                    Enum.TryParse(Config.AI, out ai);
                    AI = ai;
                }
            }
            catch (Exception e)
            {
                if(_debugging) Console.Write(e);
            }
        }

        private void ExperienceGain(Mobile defender)
        {
            if (Config == null) return;
            try
            {
                if (!(defender is BaseCreature) || ((BaseCreature) defender).Controlled) return;
                var currentLevel = Config.Levels[Level - 1];
                var b = 5 + ((BaseCreature) defender).HitsMax;
                Experience += Utility.RandomList(b + currentLevel.ExpGain.Min, b + currentLevel.ExpGain.Max);
            } 
            catch (Exception e)
            {
                if(_debugging) Console.Write(e);
            }

            EvoCheck();
        }

        private void EvoCheck()
        {
            if (Config == null) return;
            var currentLevel = Config.Levels[Level - 1];
            if (Experience < currentLevel.ExpLimit) return;
            Level = Level + 1;
            SetLevel();
        }

        private void SetLevel()
        {
            if (Config == null) return;
            try
            {
                var currentLevel = Config.Levels[Level - 1];
                if (!String.IsNullOrEmpty(currentLevel.EvoMessage)) Say($"{Name} {currentLevel.EvoMessage}");
                Name = $"{currentLevel.NameMod} {Config.BaseName}";
                foreach (var prop in currentLevel.Props)
                {
                    var bcprops = typeof(BaseCreature).GetProperties();
                    foreach (var bcprop in bcprops)
                    {
                        if (bcprop.Name == prop.Key)
                        {
                            try
                            {
                                bcprop.SetValue(this, prop.Value);
                            }
                            catch (Exception e)
                            {
                                try
                                {
                                    bcprop.SetValue(this, Convert.ToInt32(prop.Value));
                                }
                                catch (Exception e2)
                                {
                                    if (_debugging)
                                    {
                                        Say($"Failed to set {bcprop.Name}.");
                                        Console.Write(e2);
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var jsonitem in currentLevel.EvoItemDrops)
                {
                    AddToBackpack(jsonitem.GetItem());
                }

                foreach (var data in currentLevel.DamageTypes)
                {
                    Enum.TryParse<ResistanceType>(data.Key, true, out var t);
                    SetDamageType(t, (int) data.Value);
                }

                foreach (var data in currentLevel.Resistances)
                {
                    Enum.TryParse<ResistanceType>(data.Key, true, out var t);
                    SetResistance(t, (int) data.Value);
                }

                foreach (var data in currentLevel.Skills)
                {
                    var d = (RangeDouble) data.Value;
                    Enum.TryParse<SkillName>(data.Key, true, out var t);
                    if (d.Min < 0)
                    {
                        SetSkill(t, d.Max);
                        continue;
                    }

                    SetSkill(t, d.Min, d.Max);
                }

                foreach (var data in currentLevel.Stats)
                {
                    var k = ((string) data.Key).ToLower();
                    var v = (RangeInt) data.Value;
                    switch (k)
                    {
                        case "str":
                            if (v.Min < 0)
                            {
                                SetStr(v.Max);
                            }
                            else
                            {
                                SetStr(v.Min, v.Max);
                            }

                            break;
                        case "stam":
                            if (v.Min < 0)
                            {
                                SetStam(v.Max);
                            }
                            else
                            {
                                SetStam(v.Min, v.Max);
                            }

                            break;
                        case "int":
                            if (v.Min < 0)
                            {
                                SetInt(v.Max);
                            }
                            else
                            {
                                SetInt(v.Min, v.Max);
                            }

                            break;
                        case "dex":
                            if (v.Min < 0)
                            {
                                SetDex(v.Max);
                            }
                            else
                            {
                                SetDex(v.Min, v.Max);
                            }

                            break;
                        case "hits":
                            if (v.Min < 0)
                            {
                                SetHits(v.Max);
                            }
                            else
                            {
                                SetHits(v.Min, v.Max);
                            }

                            break;
                        case "damage":
                            if (v.Min < 0)
                            {
                                SetDamage(v.Max);
                            }
                            else
                            {
                                SetDamage(v.Min, v.Max);
                            }

                            break;
                    }
                }

                foreach (var data in currentLevel.SpecialAbilities)
                {
                    var sa = SpecialAbility.Abilities.FirstOrDefault(x => x.ToString().Split('.').Last() == data);
                    if (sa == null) continue;
                    SetSpecialAbility(sa);
                }

                foreach (var data in currentLevel.WeaponAbilities)
                {
                    var wa = WeaponAbility.Abilities.FirstOrDefault(x => x.ToString().Split('.').Last() == data);
                    if (wa == null) continue;
                    SetWeaponAbility(wa);
                }

                if (currentLevel.EvoSound != 0) PlaySound(currentLevel.EvoSound);
                if (currentLevel.EffectConfig.ItemId != 0)
                {
                    FixedParticles(currentLevel.EffectConfig.ItemId, currentLevel.EffectConfig.Speed,
                        currentLevel.EffectConfig.Duration,
                        currentLevel.EffectConfig.Effect, EffectLayer.Waist);
                }
            }
            catch (Exception e)
            {
                if(_debugging) Console.Write(e);
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            ExperienceGain(defender);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            {
                writer.Write(0);

                writer.Write(Experience);
                writer.Write(Level);
                Config.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Experience = reader.ReadInt();
            Level = reader.ReadInt();
            Config = new JsonEvoConfig();
            Config.Deserialize(reader);
        }
    }
}

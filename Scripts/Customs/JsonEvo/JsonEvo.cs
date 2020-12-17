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
                    var pl = _level - 1;
                    var xp = pl < 1 ? 0 : Config.Levels[pl-1].ExpLimit;
                    _experience = xp;
                    SetLevel();
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
            Female = Utility.RandomBool();
        }

        [Constructable]
        public JsonEvo(string fileName)
            : base(AIType.AI_NecroMage, FightMode.Weakest, 10, 1, 0.1, 0.2)
        {
            Name = "blank json evo";
            BodyValue = 400;
            Level = 1;
            Female = Utility.RandomBool();
            FileName = fileName;
        }

        public JsonEvo(Serial serial) : base(serial)
        {

        }

        private void LoadFromFile()
        {
            if (String.IsNullOrEmpty(FileName)) return;
            try
            {
                var json = File.ReadAllText($"Scripts/Customs/JsonEvo/Data/{FileName}.json");
                Config = (JsonEvoConfig) JsonUtility.Deserialize<JsonEvoConfig>(json);
                Console.Write(Config);
                if (Config.RandomGender) 
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
            Level = Math.Max(Math.Min(Config.Levels.Count, Level + 1), 1);
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
                    try
                    {
                        AddToBackpack(jsonitem.GetItem());
                    }
                    catch (Exception e)
                    {
                        if (_debugging)
                        {
                            Say($"Failed to add item {jsonitem.Item}.");
                            Console.Write(e);
                        }
                    }
                }

                foreach (var data in currentLevel.DamageTypes)
                {
                    try
                    {
                        Enum.TryParse<ResistanceType>(data.Key, true, out var t);
                        SetDamageType(t, (int) data.Value);
                    }
                    catch (Exception e)
                    {
                        if (_debugging)
                        {
                            Say($"Failed to set {data.Key} damage type.");
                            Console.Write(e);
                        }
                    }
                }

                foreach (var data in currentLevel.Resistances)
                {
                    try
                    {
                        Enum.TryParse<ResistanceType>(data.Key, true, out var t);
                        SetResistance(t, (int) data.Value);
                    }
                    catch (Exception e)
                    {
                        if (_debugging)
                        {
                            Say($"Failed to set {data.Key} resistance.");
                            Console.Write(e);
                        }
                    }
                }

                foreach (var data in currentLevel.Skills)
                {
                    try
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
                    catch (Exception e)
                    {
                        if (_debugging)
                        {
                            Say($"Failed to set {data.Key} skill.");
                            Console.Write(e);
                        }
                    }
                }

                foreach (var data in currentLevel.Stats)
                {
                    try
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
                    catch (Exception e)
                    {
                        if (_debugging)
                        {
                            Say($"Failed to set {data.Key} stat.");
                            Console.Write(e);
                        }
                    }
                }

                foreach (var data in currentLevel.SpecialAbilities)
                {
                    try
                    {
                        var sa = SpecialAbility.Abilities.FirstOrDefault(x => x.ToString().Split('.').Last() == data);
                        if (sa == null) continue;
                        SetSpecialAbility(sa);
                    }
                    catch (Exception e)
                    {
                        if (_debugging)
                        {
                            Say($"Failed to set {data} special ability.");
                            Console.Write(e);
                        }
                    }
                }

                foreach (var data in currentLevel.WeaponAbilities)
                {
                    try
                    {
                        var wa = WeaponAbility.Abilities.FirstOrDefault(x => x.ToString().Split('.').Last() == data);
                        if (wa == null) continue;
                        SetWeaponAbility(wa);
                    }
                    catch (Exception e)
                    {
                        if (_debugging)
                        {
                            Say($"Failed to set {data} special ability.");
                            Console.Write(e);
                        }
                    }
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
                
                writer.Write(_debugging);
                writer.Write(_fileName);
                writer.Write(_experience);
                writer.Write(_level);
                //Config.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            _debugging = reader.ReadBool();
            _fileName = reader.ReadString();
            _experience = reader.ReadInt();
            _level = reader.ReadInt();
            // getting stack overflows, probably the samt Int64 to Int32 problem as above, but don't really need it
            //Config = new JsonEvoConfig();
            //Config.Deserialize(reader);
            // I'll make a service that stores these and updates them realtime later so this doesn't have ot happen for each instance of the mob
            if(!String.IsNullOrEmpty(_fileName)) LoadFromFile();
        }
    }

    public class JsonEvoEgg : Item
    {
        private string _fileName;
        [CommandProperty(AccessLevel.GameMaster)]
        public string FileName
        {
            get => _fileName;
            set {
                _fileName = value;
                LoadFromFile();
            }
        }
        public JsonEvoConfig Config { get; set; }

        [Constructable]
        public JsonEvoEgg() : base(5928)
        {
            Weight = 0.0;
            Name = "An evo egg";
            Hue = 2591;
        }

        public JsonEvoEgg(Serial serial) : base(serial)
        {
        }

        public virtual int FollowerSlots
        {
            get { return 1; }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Config == null) return;
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("You must have the egg in your backpack to hatch it.");
            }

            else if ((from.Followers + FollowerSlots) > from.FollowersMax)
            {
                from.SendMessage("You have too many followers.");
            }


            else
            {
                this.Delete();
                from.SendMessage($"{Config.Levels[0].NameMod} {Config.BaseName} {Config.Levels[0].EvoMessage}");

                JsonEvo evo = new JsonEvo();
                evo.Config = Config;

                evo.Map = from.Map;
                evo.Location = from.Location;

                evo.Controlled = true;

                evo.ControlMaster = from;

                evo.IsBonded = true;
                evo.Level = 1;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 1);
            writer.Write(_fileName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            _fileName = reader.ReadString();
            if(!String.IsNullOrEmpty(_fileName)) LoadFromFile();
        }
        private void LoadFromFile()
        {
            if (String.IsNullOrEmpty(FileName)) return;
            try
            {
                var json = File.ReadAllText($"Scripts/Customs/JsonEvo/Data/{FileName}.json");
                Config = (JsonEvoConfig) JsonUtility.Deserialize<JsonEvoConfig>(json);
                Hue = ((int?) Config.Levels[0].Props.FirstOrDefault(x => x.Key == "Hue").Value) ?? 0;
                Name = $"{Config.BaseName} Egg";
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
    }
}

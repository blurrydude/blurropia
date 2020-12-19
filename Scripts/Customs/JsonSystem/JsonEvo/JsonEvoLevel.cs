using System.Collections.Generic;

namespace Server.Customs
{
    public class JsonEvoLevel
    {
        public string NameMod { get; set; }
        public int ExpLimit { get; set; }
        public RangeInt ExpGain { get; set; }
        public string EvoMessage { get; set; }
        public int EvoSound { get; set; }
        public EffectConfig EffectConfig { get; set; }
        public List<JsonItem> EvoItemDrops { get; set; }
        public Dictionary<string, object> Props { get; set; }
        public Dictionary<string, RangeDouble> Skills { get; set; }
        public Dictionary<string, RangeInt> Stats { get; set; }
        public Dictionary<string, int> Resistances { get; set; }
        public Dictionary<string, int> DamageTypes { get; set; }
        public List<string> WeaponAbilities { get; set; }
        public List<string> SpecialAbilities { get; set; }

        /*public void Serialize(GenericWriter writer)
        {
            writer.Write(NameMod);
            writer.Write(ExpLimit);
            ExpGain.Serialize(writer);
            writer.Write(EvoMessage);
            writer.Write(EvoSound);
            EffectConfig.Serialize(writer);
            writer.Write(EvoItemDrops.Count);
            foreach (var drop in EvoItemDrops)
            {
                drop.Serialize(writer);
            }
            DictionarySerializer.Serialize(Props, writer);
            DictionarySerializer.Serialize(Skills, writer);
            DictionarySerializer.Serialize(Stats, writer);
            DictionarySerializer.Serialize(Resistances, writer);
            DictionarySerializer.Serialize(DamageTypes, writer);
            writer.Write(WeaponAbilities.Count);
            foreach (var ab in WeaponAbilities)
            {
                writer.Write(ab);
            }
            writer.Write(SpecialAbilities.Count);
            foreach (var ab in SpecialAbilities)
            {
                writer.Write(ab);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            NameMod = reader.ReadString();
            ExpLimit = reader.ReadInt();
            ExpGain = new RangeInt();
            ExpGain.Deserialize(reader);
            EvoMessage = reader.ReadString();
            EvoSound = reader.ReadInt();
            EffectConfig = new EffectConfig();
            EffectConfig.Deserialize(reader);
            EvoItemDrops = new List<JsonItem>();
            var count = reader.ReadInt();
            for (var i = 0; i < count; i++)
            {
                var item = new JsonItem();
                item.Deserialize(reader);
                EvoItemDrops.Add(item);
            }

            Props = DictionarySerializer.Deserialize(reader);
            Skills = DictionarySerializer.Deserialize(reader);
            Stats = DictionarySerializer.Deserialize(reader);
            Resistances = DictionarySerializer.Deserialize(reader);
            DamageTypes = DictionarySerializer.Deserialize(reader);
            WeaponAbilities = new List<string>();
            count = reader.ReadInt();
            for (var i = 0; i < count; i++)
            {
                WeaponAbilities.Add(reader.ReadString());
            }
            SpecialAbilities = new List<string>();
            count = reader.ReadInt();
            for (var i = 0; i < count; i++)
            {
                SpecialAbilities.Add(reader.ReadString());
            }
        }*/
    }
}

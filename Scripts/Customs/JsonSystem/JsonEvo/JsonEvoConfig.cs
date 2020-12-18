using System.Collections.Generic;

namespace Server.Customs.JsonSystem
{
    public class JsonEvoConfig
    {
        public string BaseName { get; set; }
        public bool RandomGender { get; set; }
        public string AI { get; set; }
        public Dictionary<string, object> Props { get; set; }
        public List<JsonEvoLevel> Levels { get; set; }

        /*public void Serialize(GenericWriter writer)
        {
            writer.Write(BaseName);
            writer.Write(RandomGender);

            DictionarySerializer.Serialize(Props, writer);

            writer.Write(Levels.Count);
            foreach (var level in Levels)
            {
                level.Serialize(writer);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            BaseName = reader.ReadString();
            RandomGender = reader.ReadBool();
            AI = reader.ReadString();
            Props = DictionarySerializer.Deserialize(reader);
            var count = reader.ReadInt();
            Levels = new List<JsonEvoLevel>();
            for(var i = 0; i < count; i++)
            {
                var level = new JsonEvoLevel();
                level.Deserialize(reader);
                Levels.Add(level);
            }
        }*/
    }
}

using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Server.Customs
{
    public static class JsonQuestEngine
    {
        public static bool Enabled = true;
        public static bool Ready = false;
        public static DateTime LastUpdate;
        public static JsonQuestEngineConfig Config;

        public static void LoadQuests()
        {
            if (!Directory.Exists("Scripts/Customs/JsonSystem/JsonQuests/Data"))
            {
                Directory.CreateDirectory("Scripts/Customs/JsonSystem/JsonQuests/Data");
            }
            
            var existingMobiles = World.Mobiles.Select(x => x.Key).Where(x => Config.Givers.Contains(x.Value)).ToList();
            foreach (var existingMobile in existingMobiles)
            {
                World.FindMobile(existingMobile)?.Delete();
            }

            Config.Givers.Clear();

            var files = Directory.GetFiles("Scripts/Customs/JsonSystem/JsonQuests/Data", "*.json");
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var jsonQuest = JsonConvert.DeserializeObject<JsonQuest>(json);
                foreach (var giver in jsonQuest.Givers)
                {
                    var giverMobile = new JsonQuestGiver();
                    giverMobile.LoadConfig(giver);
                    Config.Givers.Add(giverMobile.Serial);
                    World.AddMobile(giverMobile);
                }
            }
            
            Config.UpdatePending = false;
            SaveEngineData();
        }

        public static void SaveEngineData()
        {
            var json = JsonConvert.SerializeObject(Config);
            File.WriteAllText("Scripts/Customs/JsonSystem/Data/JsonQuestEngineConfig.json",json);
        }

        public static void LoadEngineData()
        {
            var json = File.ReadAllText("Scripts/Customs/JsonSystem/Data/JsonQuestEngineConfig.json");
            Config = JsonConvert.DeserializeObject<JsonQuestEngineConfig>(json);
        }
    }
}

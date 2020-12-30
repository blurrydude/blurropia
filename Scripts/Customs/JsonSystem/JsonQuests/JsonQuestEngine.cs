using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Customs
{
    public class JsonQuestEngine : Timer
    {
        private static bool _enabled = true;
        private bool _ready = false;
        private DateTime _lastUpdate;
        private JsonQuestEngineConfig _config;
        public JsonQuestEngine()
            : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(15.0))
        {
            Priority = TimerPriority.FiveSeconds;
        }

        public static void Initialize()
        {
            if (!_enabled) return;
            new JsonQuestEngine().Start();
            Console.WriteLine("Json Quest Engine Started");
        }

        protected override void OnTick()
        {
            if (!_ready)
            {
                _lastUpdate = DateTime.Now;
                LoadEngineData();
                LoadQuests();
                _ready = true;
                return;
            }

            var now = DateTime.Now;

            if (_lastUpdate < now.AddMinutes(-1))
            {
                _lastUpdate = DateTime.Now;
                LoadEngineData();
                if (_config.UpdatePending)
                {
                    LoadQuests();
                }
            }
        }

        private void LoadQuests()
        {
            if (!Directory.Exists("Scripts/Customs/JsonSystem/JsonQuests/Data"))
            {
                Directory.CreateDirectory("Scripts/Customs/JsonSystem/JsonQuests/Data");
            }
            
            var existingMobiles = World.Mobiles.Select(x => x.Key).Where(x => _config.Givers.Contains(x.Value)).ToList();
            foreach (var existingMobile in existingMobiles)
            {
                World.FindMobile(existingMobile)?.Delete();
            }

            _config.Givers.Clear();

            var files = Directory.GetFiles("Scripts/Customs/JsonSystem/JsonQuests/Data", "*.json");
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var jsonQuest = JsonConvert.DeserializeObject<JsonQuest>(json);
                foreach (var giver in jsonQuest.Givers)
                {
                    var giverMobile = new JsonQuestGiver();
                    giverMobile.LoadConfig(giver);
                    _config.Givers.Add(giverMobile.Serial);
                    World.AddMobile(giverMobile);
                }
            }
            
            _config.UpdatePending = false;
            SaveEngineData();
        }

        private void SaveEngineData()
        {
            var json = JsonConvert.SerializeObject(_config);
            File.WriteAllText("Scripts/Customs/JsonSystem/Data/JsonQuestEngineConfig.json",json);
        }

        private void LoadEngineData()
        {
            var json = File.ReadAllText("Scripts/Customs/JsonSystem/Data/JsonQuestEngineConfig.json");
            _config = JsonConvert.DeserializeObject<JsonQuestEngineConfig>(json);
        }
    }

    public class JsonQuestEngineConfig
    {
        public List<int> Givers { get; set; }
        public bool UpdatePending { get; set; }
    }
}

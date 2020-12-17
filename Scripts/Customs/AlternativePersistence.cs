using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ServerUtilityExtensions;

namespace Server.Customs
{
    public static class AlternativePersistence
    {
        public static void SaveMobiles(Dictionary<Serial, Mobile> mobiles)
        {
            
            Utility.PushColor(ConsoleColor.Red);
            ConsoleUtility.OutputLine("Attempting alternative save");
            Utility.PopColor();
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var players = World.Mobiles.Select(x => x.Value);
                /*var json = JsonUtility.Serialize(players.Select(x => new
                {
                    Serial = x.Key,
                    x.Value.AccessLevel,
                    x.Value.Location,
                    x.Value.Map
                }));*/
                ConsoleUtility.OutputLine($"Count {players.Count()}");
                var i = 0;
                foreach (var mob in players)
                {
                    var data = JsonUtility.Serialize(new
                    {
                        mob.Serial.Value,
                        mob.Location,
                        mob.Name
                    });
                    var json = JsonUtility.Serialize(new
                    {
                        Serial = mob.Serial.Value,
                        Type = mob.GetType(),
                        Data = $"'{data}'"
                    });
                    ConsoleUtility.OutputLine($"\t{json}");
                    var response = RestUtility.ExecuteRestCall("https://api.blurrydude.com/api/UoData", true, null, json);
                    ConsoleUtility.OutputLine($"\n\t{response}");
                    i++;
                }
                sw.Stop();
                Utility.PushColor(ConsoleColor.Red);
                ConsoleUtility.OutputLine($"Jsonified in {sw.Elapsed}");
                Utility.PopColor();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                ConsoleUtility.OutputLine($"Failed to save mobiles json. {ex.Message}");
                Utility.PopColor();
            }
        }
    }
}

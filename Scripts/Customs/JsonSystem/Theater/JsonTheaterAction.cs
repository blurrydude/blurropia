using System;

namespace Server.Customs.JsonSystem.Theater
{
    public class JsonTheaterAction
    {
        public TimeSpan TriggerTime { get; set; }
        public int Actor { get; set; }
        public string Action { get; set; }
        public int[] Data { get; set; }
        public string Text { get; set; }
    }
}

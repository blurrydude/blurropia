using System.Collections.Generic;

namespace Server.Customs.JsonSystem
{
    public class JsonQuestGiverConfig
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public bool Female { get; set; }
        public bool RandomGender { get; set; }
        public bool RandomName { get; set; }
        public bool AutoDress { get; set; }
        public string[] Clothes { get; set; }
        public string StartMap { get; set; }
        public bool CantWalk { get; set; }
        public int[] StartLocation { get; set; }
        public List<JsonQuestConvoNode> ConvoNodes { get; set; }
        public int? Hue { get; set; }
        public int? SpeechHue { get; set; }
        public int? Body { get; set; }
        public int? HairItemId { get; set; }
        public int? HairHue { get; set; }
        public int? FacialHairItemId { get; set; }
        public int? FacialHairHue { get; set; }
    }
}

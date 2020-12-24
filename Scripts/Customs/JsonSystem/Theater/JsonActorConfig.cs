namespace Server.Customs.JsonSystem.Theater
{
    public class JsonActorConfig
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public bool Female { get; set; }
        public bool RandomGender { get; set; }
        public bool RandomName { get; set; }
        public bool AutoDress { get; set; }
        public string[] Clothes { get; set; }
        public int? Hue { get; set; }
        public int? SpeechHue { get; set; }
        public int? Body { get; set; }
        public int? HairItemId { get; set; }
        public int? HairHue { get; set; }
        public int? FacialHairItemId { get; set; }
        public int? FacialHairHue { get; set; }
        public Point2D StartingPlace { get; set; }
    }
}

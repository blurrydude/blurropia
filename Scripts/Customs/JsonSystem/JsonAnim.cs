namespace Server.Customs.JsonSystem
{
    public class JsonAnim
    {
        public JsonAnim()
        {
            Forward = true;
        }
        public int Action { get; set; }
        public int FrameCount { get; set; }
        public int RepeatCount { get; set; }
        public bool Forward { get; set; }
        public bool Repeat { get; set; }
        public int Delay { get; set; }

    }
}

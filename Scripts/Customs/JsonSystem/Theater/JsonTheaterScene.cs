using System.Collections.Generic;

namespace Server.Customs.JsonSystem.Theater
{
    public class JsonTheaterScene
    {
        public List<JsonActorConfig> Cast { get; set; }
        public List<JsonTheaterProp> SetProps { get; set; }
        public List<JsonTheaterAction> Actions { get; set; }
    }
}

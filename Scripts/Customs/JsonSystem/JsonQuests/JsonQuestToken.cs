namespace Server.Customs.JsonSystem
{
    public class JsonQuestToken : Item
    {
        [Constructable]
        public JsonQuestToken() : base(3699) {

        }
        [Constructable]
        public JsonQuestToken(int itemId, string name) : base(itemId)
        {
            Name = name;
        }
        public JsonQuestToken( Serial serial ) : base( serial )
        {

        }
    }
}

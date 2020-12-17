namespace Server.Customs.JsonEvo
{
    public class EffectConfig
    {
        public EffectConfig() {}
        public EffectConfig(int itemid, int speed, int duration, int effect)
        {
            ItemId = itemid;
            Speed = speed;
            Duration = duration;
            Effect = effect;
        }

        public int ItemId { get; set; }
        public int Speed { get; set; }
        public int Duration { get; set; }
        public int Effect { get; set; }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(ItemId);
            writer.Write(Speed);
            writer.Write(Duration);
            writer.Write(Effect);
        }

        public void Deserialize(GenericReader reader)
        {
            ItemId = reader.ReadInt();
            Speed = reader.ReadInt();
            Duration = reader.ReadInt();
            Effect = reader.ReadInt();
        }
    }
}
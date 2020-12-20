namespace Server.Customs
{
    public class JsonAddonComponent
    {
        public JsonAddonComponent() { }

        public JsonAddonComponent(int i, int x, int y, int z) { 
            I = i;
            X = x;
            Y = y;
            Z = z;
        }

        public int I { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}
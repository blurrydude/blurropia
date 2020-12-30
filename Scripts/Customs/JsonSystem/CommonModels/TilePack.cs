namespace Server.Customs
{
    public class TilePack
    {
        public TilePack()
        {
        }

        public TilePack(int wnc, int wsw, int ws, int we, int wew, int wsc, int rnc, int rs, int re, int rsc, int f,
            int bnwc, int bnec, int bswc, int bsec, int bn, int bs, int bw, int be, int bf)
        {
            WallNorthwestCorner = wnc;
            WallSouthWindowed = wsw;
            WallSouth = ws;
            WallEast = we;
            WallEastWindowed = wew;
            WallSoutheastCorner = wsc;
            RoofedgeNorthwestCorner = rnc;
            RoofedgeSouth = rs;
            RoofedgeEast = re;
            RoofedgeSoutheastCorner = rsc;
            Floor = f;
            BaseNorthwestCorner = bnwc;
            BaseNortheastCorner = bnec;
            BaseSouthwestCorner = bswc;
            BaseSoutheastCorner = bsec;
            BaseNorth = bn;
            BaseSouth = bs;
            BaseWest = bw;
            BaseEast = be;
            BaseFill = bf;
        }

        public int WallNorthwestCorner { get; set; }
        public int WallSouthWindowed { get; set; }
        public int WallSouth { get; set; }
        public int WallEast { get; set; }
        public int WallEastWindowed { get; set; }
        public int WallSoutheastCorner { get; set; }
        public int RoofedgeNorthwestCorner { get; set; }
        public int RoofedgeSouth { get; set; }
        public int RoofedgeEast { get; set; }
        public int RoofedgeSoutheastCorner { get; set; }
        public int Floor { get; set; }
        public int BaseNorthwestCorner { get; set; }
        public int BaseNortheastCorner { get; set; }
        public int BaseSouthwestCorner { get; set; }
        public int BaseSoutheastCorner { get; set; }
        public int BaseNorth { get; set; }
        public int BaseSouth { get; set; }
        public int BaseWest { get; set; }
        public int BaseEast { get; set; }
        public int BaseFill { get; set; }
    }
}

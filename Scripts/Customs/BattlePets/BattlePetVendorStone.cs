namespace Server.Customs.BattlePets
{
    public class BattlePetVendorStone : Item
    {
        [Constructable]
        public BattlePetVendorStone() : base(9243)
        {
            Hue = 1271;
            Name = "Battle Pet Vendor Stone";
        }

        public BattlePetVendorStone(Serial serial) : base(serial)
        {

        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(BattlePetGump))) from.CloseGump(typeof(BattlePetGump));
            from.SendGump(new BattlePetVendorGump());
        }
    }
}
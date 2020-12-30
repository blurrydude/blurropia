namespace Server.Customs
{
    public class SudoStone : Item
    {
        private string _menu;
        [CommandProperty(AccessLevel.GameMaster)]
        public string Menu
        {
            get => _menu;
            set
            {
                _menu = value;
                Name = $"Sudo Stone - {_menu}";
            }
        }
        [Constructable]
        public SudoStone() : base(3631)
        {
            Name = $"Sudo Stone - {_menu}";
            _menu = "sudo";
        }
        public SudoStone( Serial serial ) : base( serial )
        {

        }

        public override void OnDoubleClick(Mobile @from)
        {
            from.SendGump(new SudoTool(_menu));
        }

        public override void Serialize(GenericWriter writer)
        {
            writer.Write(_menu);
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            _menu = reader.ReadString();
            base.Deserialize(reader);
        }
    }
}

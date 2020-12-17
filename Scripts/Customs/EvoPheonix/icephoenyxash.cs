using System;

namespace Server.Items
{
	public class phoenixash : Item
	{
		[Constructable]
		public phoenixash() : this( 1 )
		{
		}

		[Constructable]
		public phoenixash( int amount ) : base( 0xF8C )
		{
			Name = "Phoenix Ash";
			Hue = 1107;
			Weight = 1.0;
			Stackable = true;
			Amount = amount;
		}

		public phoenixash( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
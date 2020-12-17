using System;
using Server;
using Server.Targeting;

namespace Server.Items
{
	[Flipable( 0x14E8, 0x14E7 )]
	public class BetterHitchingPost : AddonComponent
	{
		#region Constructors
		[Constructable]
		public BetterHitchingPost() : this( 0x14E7 )
		{
		}

		[Constructable]
		public BetterHitchingPost( int itemID ) : base( itemID )
		{
		}
		
		public BetterHitchingPost( Serial serial ) : base( serial )
		{
		}
		#endregion

		public override void OnDoubleClick( Mobile from )
		{
			if( from.InRange( this.GetWorldLocation(), 2 ) == false )
			{
				from.SendLocalizedMessage( 500486 );	//That is too far away.
			}
			else
			{
				from.Target=new HitchingPostTarget( this );
				from.SendMessage( "What do you wish to shrink?" );
			}
		}

		private class HitchingPostTarget : Target
		{
			private BetterHitchingPost m_Post;

			public HitchingPostTarget( Item i ) : base( 3, false, TargetFlags.None )
			{
				m_Post=(BetterHitchingPost)i;
			}
			
			protected override void OnTarget( Mobile from, object targ )
			{
				if ( !(m_Post.Deleted) )
				{
					ShrinkFunctions.Shrink( from, targ );
				}

				return;
			}
		}
        

		#region Serialization
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		} 
		#endregion
	}


	public class BetterHitchingPostEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BetterHitchingPostEastDeed(); } }

		[Constructable]
		public BetterHitchingPostEastAddon()
		{
			AddComponent( new BetterHitchingPost( 0x14E7 ), 0, 0, 0);
		}

		public BetterHitchingPostEastAddon( Serial serial ) : base( serial )
		{
		}

		#region Serialization
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		} 
		#endregion
	}

	public class BetterHitchingPostEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new BetterHitchingPostEastAddon(); } }

		[Constructable]
		public BetterHitchingPostEastDeed()
		{
			Name="Hitching Post (east)";
		}

		public BetterHitchingPostEastDeed( Serial serial ) : base( serial )
		{
		}

		#region Serialization
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		} 
		#endregion
	}

	
	public class BetterHitchingPostSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BetterHitchingPostSouthDeed(); } }

		[Constructable]
		public BetterHitchingPostSouthAddon()
		{
			AddComponent( new BetterHitchingPost( 0x14E8 ), 0, 0, 0);
		}

		public BetterHitchingPostSouthAddon( Serial serial ) : base( serial )
		{
		}

		#region Serialization
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		} 
		#endregion
	}

	public class BetterHitchingPostSouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new BetterHitchingPostSouthAddon(); } }

		[Constructable]
		public BetterHitchingPostSouthDeed()
		{
			Name="Hitching Post (south)";
		}

		public BetterHitchingPostSouthDeed( Serial serial ) : base( serial )
		{
		}

		#region Serialization
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		} 
		#endregion
	}

}

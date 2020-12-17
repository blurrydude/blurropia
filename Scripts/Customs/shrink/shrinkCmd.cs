using System;
using System.Reflection;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using System.Collections;

namespace Server.Commands
{
	public class ShrinkCmd
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Shrink", AccessLevel.GameMaster, new CommandEventHandler( Shrink_OnCommand ) );
		}   
     
		[Usage( "Shrink" )]
		[Description( "Shrinks a targeted Mobile" )]

		public static void Shrink_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new ShrinkCmdTarget();
			e.Mobile.SendMessage( "What do you wish to shrink?" );
		}	


		private class ShrinkCmdTarget : Target
		{
			public ShrinkCmdTarget() : base( 15, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				ShrinkFunctions.Shrink( from, targ, false );
			}
		}
	}
}
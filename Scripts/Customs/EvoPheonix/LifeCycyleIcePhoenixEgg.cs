    ///////////////////////////////////////////
   //   Evo IcePhoenyx (New Bodyvalues) by  //
  //  IcePhoenyxRising of Runuo/Servuo     //
 //  Created from evo phoenix by Raelis   //
///////////////////////////////////////////
using System; 
using System.Collections;
using Server.Items; 
using Server.Mobiles; 
using Server.Misc;
using Server.Network;

namespace Server.Items
{
    public class LifeCycleIcePhoenyxEgg : Item
    {
        [Constructable]
        public LifeCycleIcePhoenyxEgg() : base(5928)
        {
            Weight = 0.0;
            Name = "A Legendary Ice Phoenyx egg";
            Hue = 2591;
        }

        public LifeCycleIcePhoenyxEgg(Serial serial) : base(serial)
        {
        }

        public virtual int FollowerSlots
        {
            get { return 1; }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("You must have the IcePhoenyx egg in your backpack to hatch it.");
            }

            else if ((from.Followers + FollowerSlots) > from.FollowersMax)
            {
                from.SendMessage("You have too many followers.");
            }


            else
            {
                this.Delete();
                from.SendMessage("You are now the proud owner of a IcePhoenyx hatchling!!");

                LifeCycleIcePhoenyx dragon = new LifeCycleIcePhoenyx();

                dragon.Map = from.Map;
                dragon.Location = from.Location;

                dragon.Controlled = true;

                dragon.ControlMaster = from;

                dragon.IsBonded = true;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();


        }


    }
}

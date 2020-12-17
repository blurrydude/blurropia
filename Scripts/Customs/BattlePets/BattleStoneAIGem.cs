using System.Linq;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Customs.BattlePets
{
    public class BattleStoneAIGem : Item
    {
        [Constructable]
        public BattleStoneAIGem() : base(0xF16)
        {
            Name = "a Mage AI stone";
            _AIType = AIType.AI_Mage;
        }
	
        public BattleStoneAIGem(Serial serial) : base(serial) {
		    
        }
	
        private AIType _AIType;
        [CommandProperty(AccessLevel.GameMaster)]
        public AIType AIType
        {
            get
            {
                return _AIType;
            }
            set
            {
                _AIType = value;
                Name = $"a {GetAiName()} AI stone";
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.InRange( this.GetWorldLocation(), 2 ) == false )
            {
                from.SendLocalizedMessage( 500486 );	//That is too far away.
            }
            else
            {
                from.Target=new BattleStoneAIGemTarget( this );
                from.SendMessage( "Target your Battle Pet Stone" );
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
            writer.Write((int)_AIType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
            _AIType = (AIType)reader.ReadInt();
        }

        private class BattleStoneAIGemTarget : Target
        {
            private BattleStoneAIGem _AiGem;

            public BattleStoneAIGemTarget(BattleStoneAIGem aiGem) : base( 3, false, TargetFlags.None )
            {
                _AiGem = aiGem;
            }
			
            protected override void OnTarget( Mobile from, object targ )
            {
                if (!(targ is BattlePetStone))
                {
                    from.SendMessage("That is not a Battle Pet Stone");
                    return;
                }

                var bps = (BattlePetStone) targ;
                bps.AIType = _AiGem.AIType;
                from.SendMessage("Battle Pet AI changed.");
                _AiGem.Delete();
                if (bps.Owner.HasGump(typeof(BattlePetGump)))
                {
                    bps.Owner.CloseGump(typeof(BattlePetGump));
                    bps.Owner.SendGump(new BattlePetGump(bps));
                }
                return;
            }
        }

        private string GetAiName()
        {
            return _AIType == AIType.AI_Melee ? "Battle" : _AIType.ToString().Replace("AI_", string.Empty);
        }
    }
}

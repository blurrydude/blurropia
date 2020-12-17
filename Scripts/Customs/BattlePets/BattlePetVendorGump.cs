using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Customs.BattlePets
{
    public class BattlePetVendorGump : Gump
    {
        public BattlePetVendorGump() : base(20, 20)
        {
            AddBackground(0, 0, 200, 300, 40000);
            AddLabel(10,12,1074,"Buy A Battle Pet");

            var y = 30;

            foreach (var data in _PriceList)
            {
                AddButton(10, y, 11411, 11412, (int)data.Key+10, GumpButtonType.Reply, 0);
                AddLabel(40,y,1166,$"{data.Key}");
                AddLabel(150,y,1173,$"{data.Value}g");
                
                y += 20;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender.Mobile.HasGump(typeof(BattlePetGump))) sender.Mobile.CloseGump(typeof(BattlePetGump));
            var buttonId = info.ButtonID;
            if (buttonId == 0)
            {
                return;
            }

            var bodyId = buttonId - 10;
            var body = (BattlePetBody) bodyId;

            if (sender.Mobile.Backpack.TotalGold < _PriceList[body])
            {
                sender.Mobile.SendMessage("You cannot afford that.");
                return;
            }

            sender.Mobile.Backpack.ConsumeTotal(typeof(Gold), _PriceList[body]);

            var bps = new BattlePetStone(body);
            bps.Owner = sender.Mobile;
            sender.Mobile.AddToBackpack(bps);
            sender.Mobile.SendMessage($"You've purchased a {bps.Name}.");
        }

        private Dictionary<BattlePetBody, int> _PriceList = new Dictionary<BattlePetBody, int>
        {
            {BattlePetBody.Squirrel, 1000},
            {BattlePetBody.Chicken, 1000},
            {BattlePetBody.Frog, 1500},
            {BattlePetBody.Fox, 1500},
            {BattlePetBody.Pig, 1500},
            {BattlePetBody.Goat, 2000},
            {BattlePetBody.Tiger, 4000},
            {BattlePetBody.Llama, 5000},
            {BattlePetBody.Ostard, 5000},
            {BattlePetBody.Walrus, 5000},
            {BattlePetBody.Bear, 5000}
        };
    }
}

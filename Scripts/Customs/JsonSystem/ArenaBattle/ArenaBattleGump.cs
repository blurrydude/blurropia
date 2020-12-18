using System;
using System.Linq;
using Server.ArenaBattleEngine;
using Server.Gumps;
using Server.Network;

namespace Server.Customs
{
    public class ArenaBattleGump : Gump
    {
        private ArenaBattleStone _BattleStone;
        private Mobile _From;
        public ArenaBattleGump(Mobile from, ArenaBattleStone battleStone) : base(25, 25)
        {
            _BattleStone = battleStone;
            _From = from;
            var w = _BattleStone.ArenaXRange * 2 + 1;
            var h = _BattleStone.ArenaYRange * 2 + 1;
            var stonePosX = -_BattleStone.OffsetX;
            var stonePosY = -_BattleStone.OffsetY;
            var offset = 5;
            AddBackground(0,0,w*64+offset*2,h*64+120,1755);
            for (var x = 0; x < w; x++)
            {
                for (var y = 0; y < h; y++)
                {
                    if(x == stonePosX && y == stonePosY) {
                        AddImage(x*64+offset,y*64+offset,1005);
                        continue;
                    }
                    //AddBackground(x*64, y*64, 64, 64, 1755);
                    var index = MathHelper.GetIndex(x, y, w);
                    var existing = _BattleStone.Grid.FirstOrDefault(g => g.Index == index);
                    var buttonId = 0;
                    switch (existing?.FighterType)
                    {
                        case ArenaFighterType.Assassin: buttonId = 1002; break;
                        case ArenaFighterType.Healer: buttonId = 1032; break;
                        case ArenaFighterType.Fencer: buttonId = 1008; break;
                        case ArenaFighterType.Macer: buttonId = 1009; break;
                        case ArenaFighterType.Swordsman: buttonId = 1007; break;
                        case ArenaFighterType.Supermage: buttonId = 1006; break;
                        default: buttonId = 1033; break;
                    }
                    AddButton(x*64+offset,y*64+offset,buttonId,buttonId,index+1,GumpButtonType.Reply,0);
                }
            }

            if (!_BattleStone.Automatic)
            {
                AddButton(10, h * 64 + 10, 2026, 2025, 1000, GumpButtonType.Reply, 0);
                AddButton(10, h * 64 + 36, 2023, 2022, 1001, GumpButtonType.Reply, 0);
                AddButton(84, h * 64 + 62, 2113, 2112, 1003, GumpButtonType.Reply, 0);
            }
            else
            {
                AddButton(84,h*64+62,2116,2115,1004,GumpButtonType.Reply,0);
            }
            AddButton(w * 64 - 84, h * 64 + 62, 12003, 12005, 1002, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender.Mobile.HasGump(typeof(ArenaBattleGump))) sender.Mobile.CloseGump(typeof(ArenaBattleGump));
            if (info.ButtonID < 1) return;
            if (info.ButtonID >= 1000)
            {
                FireCommand(info.ButtonID-1000);
                sender.Mobile.SendGump(new ArenaBattleGump(_From, _BattleStone));
                return;
            }
            var i = info.ButtonID - 1;
            var existing = _BattleStone.Grid.FirstOrDefault(g => g.Index == i);
            if (existing == null)
            {
                _BattleStone.Spawn(i);
                sender.Mobile.SendGump(new ArenaBattleGump(_From, _BattleStone));
                return;
            }
            _BattleStone.Swap(i);
            sender.Mobile.SendGump(new ArenaBattleGump(_From, _BattleStone));
        }

        public void FireCommand(int id)
        {
            switch (id)
            {
                case 0: _BattleStone.War();
                    break;
                case 1: _BattleStone.Peace();
                    break;
                case 2: _BattleStone.RemoveAll();
                    break;
                case 3: _BattleStone.Auto();
                    break;
                case 4: _BattleStone.Manual();
                    break;
            }
        }
    }
    public class ArenaBattleBettingGump : JsonGump
    {
        private ArenaBattleStone _BattleStone;
        private Mobile _From;
        private int _Wager;
        public ArenaBattleBettingGump(Mobile from, ArenaBattleStone battleStone, int wager = 100) : base($"Scripts/Customs/JsonSystem/ArenaBattle/Data/arenaBattleBettingGump.json")
        {
            _BattleStone = battleStone;
            _From = from;
            _Wager = wager;
            Timer.DelayCall(TimeSpan.FromSeconds(1), BettingOpenCheck);
            
            AddLabel(10,10,256,"Team 1");
            AddLabel(210,10,256,"Team 2");
            var y = 30;
            foreach (var fighter in _BattleStone.Grid.Where(x => x.Team == 1))
            {
                var m = World.Mobiles[fighter.Serial];
                AddLabel(10,y,256,$"{fighter.FighterType.ToString()} s:{m.Str} d:{m.Dex} i:{m.Int}");
                y += 18;
            }
            y = 30;
            foreach (var fighter in _BattleStone.Grid.Where(x => x.Team == 2))
            {
                var m = World.Mobiles[fighter.Serial];
                AddLabel(210,y,256,$"{fighter.FighterType.ToString()} s:{m.Str} d:{m.Dex} i:{m.Int}");
                y += 18;
            }
            AddLabel(158, 230, 256, $"Wager: {_Wager}");

        }

        public void BettingOpenCheck()
        {
            if (!_BattleStone.BettingOpen)
            {
                _From.CloseGump(typeof(ArenaBattleBettingGump));
                return;
            }
            Timer.DelayCall(TimeSpan.FromSeconds(1), BettingOpenCheck);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender.Mobile.HasGump(typeof(ArenaBattleBettingGump))) sender.Mobile.CloseGump(typeof(ArenaBattleBettingGump));
            if (info.ButtonID < 1) return;
            switch (info.ButtonID)
            {
                case 1:
                    _Wager = _Wager > 100 ? _Wager -= 100 : 100;
                    sender.Mobile.SendGump( new ArenaBattleBettingGump(sender.Mobile,_BattleStone,_Wager));
                    break;
                case 2:
                    _Wager = _Wager < 10000 ? _Wager += 100 : 10000;
                    sender.Mobile.SendGump( new ArenaBattleBettingGump(sender.Mobile,_BattleStone,_Wager));
                    break;
                case 3:
                    _BattleStone.PlaceWager(1,_Wager,sender.Mobile);
                    break;
                case 4:
                    _BattleStone.PlaceWager(2,_Wager,sender.Mobile);
                    break;
            }
        }
    }
}

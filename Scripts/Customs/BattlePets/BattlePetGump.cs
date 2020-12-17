using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Network;

namespace Server.Customs.BattlePets
{
    public class BattlePetGump : Gump
    {
        private BattlePetStone _Stone;
        public BattlePetGump(BattlePetStone stone) : base(20,20)
        {
            _Stone = stone;
            var h =
                _Stone.Enhancements.Count * 20 + 164;
            AddBackground(0,0,200,h,40000);

            AddLabel(10,12,1074,"Level");
            AddLabel(150,12,1084,$"{_Stone.Level}");
            
            AddLabel(10,32,1074,"Strength");
            AddLabel(150,32,1084,$"{_Stone.Str}");
            
            AddLabel(10,52,1074,"Intelligence");
            AddLabel(150,52,1084,$"{_Stone.Int}");
            
            AddLabel(10,72,1074,"Dexterity");
            AddLabel(150,72,1084,$"{_Stone.Dex}");
            
            AddLabel(10,92,1074,"Armor");
            AddLabel(150,92,1084,$"{_Stone.Armor}");
            
            AddLabel(10,112,1074,"Damage");
            AddLabel(150,112,1084,$"{_Stone.Damage}");

            var list = new Dictionary<SkillName, double>();
            foreach (var enhancement in _Stone.Enhancements.Where(x => x.EnhancementType == BattlePetEnhancementType.Skill))
            {
                if (!list.ContainsKey(enhancement.Skill))
                {
                    list.Add(enhancement.Skill, 0);
                }

                list[enhancement.Skill] += enhancement.SkillMod;
            }
            
            var y = 132;
            foreach (var enhancement in _Stone.Enhancements.Where(x => x.EnhancementType != BattlePetEnhancementType.Skill && x.EnhancementType != BattlePetEnhancementType.Spell))
            {
                AddButton(10, y, 11411, 11412, _Stone.Enhancements.IndexOf(enhancement)+200, GumpButtonType.Reply, 0);
                AddLabel(40,y,1168,$"{enhancement.ModName}");
                
                y += 20;
            }

            foreach (var data in list)
            {
                AddButton(10, y, 11411, 11412, (int)data.Key+100, GumpButtonType.Reply, 0);
                AddLabel(40,y,1166,$"{data.Key}");
                AddLabel(150,y,1173,$"{data.Value}");
                
                y += 20;
            }

            foreach (var enhancement in _Stone.Enhancements.Where(x =>
                x.EnhancementType == BattlePetEnhancementType.Spell))
            {
                AddButton(10, y, 11411, 11412, _Stone.Enhancements.IndexOf(enhancement)+200, GumpButtonType.Reply, 0);
                AddLabel(40,y,1149,$"{enhancement.Spell.Name}");
                
                y += 20;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var button = info.ButtonID;
            if (sender.Mobile.HasGump(typeof(BattlePetGump))) sender.Mobile.CloseGump(typeof(BattlePetGump));
            if (button == 0) return;

            if (button >= 200)
            {
                var enhancementIndex = button - 200;
                var enhancement = _Stone.Enhancements[enhancementIndex];
                _Stone.Owner.AddToBackpack(enhancement);
                _Stone.Enhancements.RemoveAt(enhancementIndex);
                _Stone.Owner.SendMessage("You remove the enhancement from the Battle Pet Stone.");
            }
            else if (button >= 100)
            {
                var skillName = (SkillName)(button - 100);
                var enhancement = _Stone.Enhancements.First(x => x.Skill == skillName);
                _Stone.Owner.AddToBackpack(enhancement);
                _Stone.Enhancements.Remove(enhancement);
                _Stone.Owner.SendMessage("You remove the enhancement from the Battle Pet Stone.");
            }

            sender.Mobile.SendGump(new BattlePetGump(_Stone));
        }
    }
}
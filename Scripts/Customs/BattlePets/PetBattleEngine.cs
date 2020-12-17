using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Customs.BattlePets
{
    public static class PetBattleEngine {
	    
    }

    public enum Rarity
    {
        Junk, // 20%
        Common, // 50%
        Uncommon, // 15%
        Rare, // 10%
        Legendary, // 3%
        Mythical, // 1.5%
        Divine // 0.5%
    }

    public enum BattlePetEnhancementType
    {
        Armor,
        Damage,
        Attack,
        Special,
        Spell,
        Skill,
        Stat
    }

    public enum BattlePetBody
    {
        Squirrel,
        Chicken,
        Frog,
        Fox,
        Pig,
        Goat,
        Ostard,
        Llama,
        Walrus,
        Tiger,
        Bear
    }
}

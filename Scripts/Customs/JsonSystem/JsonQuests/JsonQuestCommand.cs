using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Commands;

namespace Server.Customs
{
    public class JsonQuestCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("sudo", AccessLevel.GameMaster, new CommandEventHandler(JsonQuestCommand_OnCommand));
        }

        [Usage("jsonquestload")]
        [Description("Loads/Reloads Json Quests.")]
        public static void JsonQuestCommand_OnCommand(CommandEventArgs arg)
        {
            
        }
    }
}

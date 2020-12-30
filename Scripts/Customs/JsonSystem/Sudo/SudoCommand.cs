using System;
using System.IO;
using Server.Commands;

namespace Server.Customs
{
    public class SudoCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("sudo", AccessLevel.GameMaster, SudoToolCommand_OnCommand);
            Console.WriteLine("sudo command registered");
            var menus = Directory.GetFiles($@"{AppDomain.CurrentDomain.BaseDirectory}Scripts\Customs\JsonSystem\Sudo\Data\", "*.json");
            foreach (var menu in menus)
            {
                Console.WriteLine(menu);
            }
        }

        [Usage("sudo")]
        [Description("Opens the sudo tool.")]
        public static void SudoToolCommand_OnCommand(CommandEventArgs arg)
        {
            Console.WriteLine("sudo command fired");
            arg.Mobile.SendGump(new SudoTool(arg.Length > 0 ? arg.GetString(0) : "sudo"));
        }
    }
}

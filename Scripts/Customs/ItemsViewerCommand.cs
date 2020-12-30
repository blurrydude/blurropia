using Server.Commands;

namespace Server.Customs
{
    public class ItemsViewerCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("ItemViewer", AccessLevel.GameMaster, new CommandEventHandler(ItemsViewerCommand_OnCommand));
        }

        [Usage("ItemViewer")]
        [Description("Opens the item viewer.")]
        public static void ItemsViewerCommand_OnCommand(CommandEventArgs arg)
        {
            arg.Mobile.SendGump(new ItemsViewer(arg.Length > 0 ? (int)arg.GetDouble(0) : 0));
        }
    }
}

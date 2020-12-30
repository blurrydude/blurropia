using System;
using System.Linq;
using System.Reflection;
using Server.Network;

namespace Server.Customs
{
    public static class JsonQuestHelper
    {
        public static void CheckGiveItem(JsonQuestGiver giver, JsonQuestConvoNode node, Mobile @from)
        {
            if (!String.IsNullOrEmpty(node.Item.Item))
            {
                var built = JsonSystemHelper.ItemFromJsonItem(node.Item);

                if (built != null)
                {
                    from.AddToBackpack(built);
                }
                else
                {
                    giver.PublicOverheadMessage(MessageType.Regular, 0x35, false, "I was supposed to give you something, but I can't. You should tell an admin about this.");
                }
            }
        }
    }
}

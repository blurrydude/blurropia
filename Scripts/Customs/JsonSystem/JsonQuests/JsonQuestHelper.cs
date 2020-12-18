using System;
using System.Linq;
using System.Reflection;
using Server.Network;

namespace Server.Customs.JsonSystem
{
    public static class JsonQuestHelper
    {
        public static object NewItemByTypeString(string typeString) {
            object built = null;
            
            try
            {
                Type type = ScriptCompiler.FindTypeByName(typeString);
                var ctors = type.GetConstructors();
                for (int i = 0; i < ctors.Length; ++i)
                {
                    ConstructorInfo ctor = ctors[i];

                    ParameterInfo[] paramList = ctor.GetParameters();

                    if (paramList.Length == 0)
                    {
                        built = ctor.Invoke(new object[0]);
                    }
                }
            } catch(Exception e) {}

            return built;
        }

        public static bool IsEqualJsonItem(Item item, JsonItem jsonQuestItem)
        {
            if (jsonQuestItem == null) return false;
            var itemType = item.GetType().ToString().Split('.').Last();
            if (itemType != jsonQuestItem.Item) return false;
            if (item.Amount != jsonQuestItem.Amount) return false;
            var list = item.GetType().GetProperties();
            var matching = true;
            foreach (var prop in jsonQuestItem.Props)
            {
                foreach (var oprop in list)
                {
                    if (oprop.Name == prop.Key)
                    {
                        if (oprop.GetValue(item) != prop.Value) return false;
                    }
                }
            }
            return true;
        }

        public static Item ItemFromJsonItem(JsonItem jsonQuestItem)
        {
            Item item = (Item)NewItemByTypeString(jsonQuestItem.Item);
            var list = item.GetType().GetProperties();
            item.Amount = jsonQuestItem.Amount;
            foreach (var prop in jsonQuestItem.Props)
            {
                foreach (var oprop in list)
                {
                    if (oprop.Name == prop.Key)
                    {
                        oprop.SetValue(item, prop.Value);
                    }
                }
            }

            return item;
        }

        public static void CheckGiveItem(JsonQuestGiver giver, JsonQuestConvoNode node, Mobile @from)
        {
            if (!String.IsNullOrEmpty(node.Item.Item))
            {
                var built = ItemFromJsonItem(node.Item);

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

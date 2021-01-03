using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Server.Customs
{
    public static class JsonSystemHelper
    {
        public static object NewItemByTypeString(string typeString)
        {
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
            }
            catch (Exception e) { CustomUtility.ExceptionIgnore(e); }

            return built;
        }

        public static bool IsEqualJsonItem(Item item, JsonItem jsonQuestItem)
        {
            if (jsonQuestItem == null) return false;
            var itemType = item.GetType().ToString().Split('.').Last();
            if (itemType != jsonQuestItem.Item) return false;
            if (item.Amount != jsonQuestItem.Amount) return false;
            var list = item.GetType().GetProperties();
            //var matching = true;
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
                        try
                        {
                            oprop.SetValue(item, prop.Value);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            Console.WriteLine($"Couldn't parse {prop.Key}: {prop.Value} type of {prop.Value.GetType()} trying Int32 instead...");
                            //I'm not really sure why this comes in thinking it's an Int64, but... yeah, I'm dumb.
                            //CustomUtility.ExceptionIgnore(e);
                            try
                            {
                                oprop.SetValue(item, Int32.Parse(prop.Value.ToString()));
                            }
                            catch (Exception e2)
                            {
                                Console.WriteLine(e2);
                                Console.WriteLine("Failed to parse as Int64, ignoring prop.");
                                CustomUtility.ExceptionIgnore(e2);
                            }
                        }
                    }
                }
            }

            return item;
        }

        public static string[] GetWrapped(string original, int linecharlimit) {
            var count = 0;
            var lastspace = -1;
            var output = new List<string>();
            while (original.Length > 0)
            {
                if (original[count] == ' ') lastspace = count;
                count++;
                if (count > linecharlimit)
                {
                    count = 0;
                    var str = original.Substring(0, lastspace + 1);
                    output.Add(str);
                    original = original.Substring(lastspace+1);
                }
                if(count == original.Length - 1) {
                    output.Add(original);
                    original = String.Empty;
                }
            }
            return output.ToArray();
        }
    }
}

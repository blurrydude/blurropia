using System;
using System.Collections.Generic;
using System.Reflection;

namespace Server.Customs
{
    public class JsonItem
    {
        public string Item { get; set; }
        public int Amount { get; set; }
        public Dictionary<string, object> Props { get; set; }
        
        public Item GetItem()
        {
            object built = null;

            try
            {
                Type type = ScriptCompiler.FindTypeByName(Item);
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

                if (built == null) return null;

                Item item = (Item) built;

                var list = item.GetType().GetProperties();
                item.Amount = Amount;
                foreach (var prop in Props)
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
            catch (Exception e)
            {
                Console.Write(e);
                return null;
            }
        }
    }
}

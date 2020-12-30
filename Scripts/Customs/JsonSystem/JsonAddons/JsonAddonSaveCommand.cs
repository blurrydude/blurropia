using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Commands;
using Server.Commands.Generic;
using Server.Prompts;
using Server.Targeting;

namespace Server.Customs
{
    public class JsonAddonSaveCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("makeaddon", AccessLevel.GameMaster, new CommandEventHandler(MakeAddon_OnCommand));
        }

        [Usage("makeaddon")]
        [Description("Saves all items in a targeted bounding box as a json addon file.")]
        private static void MakeAddon_OnCommand(CommandEventArgs e)
        {
            BoundingBoxPicker.Begin(e.Mobile, new BoundingBoxCallback(AddonBox_Callback), null);
        }

        private class OriginTarget : Target
        {
            private Map _map;
            private Point3D _start;
            private Point3D _end;

            public OriginTarget(Map map, Point3D start, Point3D end)
                : base(-1, true, TargetFlags.None)
            {
                _map = map;
                _start = start;
                _end = end;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (!BaseCommand.IsAccessible(from, o))
                {
                    from.SendMessage("That is not accessible.");
                }
                else
                {
                    //from.SendGump(new PropertiesGump(from, o));
                    
                    var items = World.Items.Where(x => x.Value.Map == _map &&
                                                       x.Value.Location.X >= _start.X &&
                                                       x.Value.Location.X <= _end.X &&
                                                       x.Value.Location.Y >= _start.Y &&
                                                       x.Value.Location.Y <= _end.Y
                    );
                    
                    IPoint3D p = o as IPoint3D;
                    var components = new List<JsonAddonComponent>();
                    foreach (var item in items)
                    {
                        var type = item.Value.GetType().ToString().Split('.').Last();
                        if (type == "JsonAddon") continue;
                        var x = item.Value.Location.X - p.X;
                        var y = item.Value.Location.Y - p.Y;
                        var z  = item.Value.Location.Z - p.Z;
                        var jac = new JsonAddonComponent
                        {
                            I = item.Value.ItemID,
                            H = item.Value.Hue,
                            X = x,
                            Y = y,
                            Z = z,
                            T = type
                        };
                        components.Add(jac);
                    }

                    var json = JsonConvert.SerializeObject(components);
                    from.SendMessage("Enter a name for the addon (avoid spaces)");
                    from.Prompt = new NamePrompt(json);
                }
            }
        }

        private class NamePrompt : Prompt
        {
            private string _json;
            public NamePrompt(string json)
            {
                _json = json;
            }

            public override void OnResponse(Mobile from, string text)
            {
                File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}Scripts\Customs\JsonSystem\JsonAddons\Data\{text}.json", _json);
            }
        }

        private static void AddonBox_Callback(Mobile from, Map map, Point3D start, Point3D end, object state)
        {
            from.SendMessage("Select an origin point");
            from.Target = new OriginTarget(map,start,end);
        }
    }
}

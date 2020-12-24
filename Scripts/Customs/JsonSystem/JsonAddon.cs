using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;
using Server.Network;
using Server.Prompts;
using Server.Targeting;
using ServerUtilityExtensions;

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
                        var x = item.Value.Location.X - p.X;
                        var y = item.Value.Location.Y - p.Y;
                        var z  = item.Value.Location.Z - p.Z;
                        var jac = new JsonAddonComponent
                        {
                            I = item.Value.ItemID,
                            X = x,
                            Y = y,
                            Z = z
                        };
                        components.Add(jac);
                    }

                    var json = JsonUtility.Serialize(components);
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
                File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}Scripts\Customs\JsonSystem\JsonAddons\{text}.json", _json);
            }
        }

        private static void AddonBox_Callback(Mobile from, Map map, Point3D start, Point3D end, object state)
        {
            from.SendMessage("Select an origin point");
            from.Target = new OriginTarget(map,start,end);
        }
    }
    public class JsonAddon : BaseAddon
    {
        private int _owner;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Owner
        {
            get => _owner;
            set => _owner = value;
        }
        [Constructable]
        public JsonAddon()
        {
            Name = "default";
            Owner = 0;
            LoadFromJson();
        }

        public JsonAddon(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnSpeech => true;

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            Mobile m = e.Mobile;

            if(m.AccessLevel < AccessLevel.GameMaster || _owner > 0 && m.Serial.Value != _owner) {
                return;
            }

            if (!m.InRange(GetWorldLocation(), 1))
            {
                return;
            }

            if (_owner == 0 && e.Speech == "mine")
            {
                _owner = m.Serial.Value;
                PublicOverheadMessage(MessageType.Regular, 0x35, false, $"I now belong to {m.Name}");
            }

            if (e.Speech == "reset")
            {
                Name = "default";
                Reload();
            }

            if (e.Speech == "reload")
            {
                Reload();
            }

            if (e.Speech.Contains("set to"))
            {
                Name = e.Speech.Replace("set to ", string.Empty);
                Reload();
            }
        }

        public override void OnDoubleClick(Mobile @from) {
            if(Name == "default") {
                Reload();
            }
        }

        private void Reload()
        {
            List<AddonComponent> toRemove = new List<AddonComponent>(Components);
            LoadFromJson();

            foreach (var remove in toRemove)
            {
                Components.Remove(remove);
                remove.Delete(true);
            }
        }

        private void LoadFromJson()
        {
            if (!Directory.Exists("Scripts/Customs/JsonSystem/JsonAddons"))
            {
                Directory.CreateDirectory("Scripts/Customs/JsonSystem/JsonAddons");
            }

            var file = $"Scripts/Customs/JsonSystem/JsonAddons/{Name}.json";
            if (!File.Exists(file))
            {
                var newjson = "[{\"I\":41,\"X\":0,\"Y\":0,\"Z\":0}]";
                File.WriteAllText(file, newjson);
            }

            var json = File.ReadAllText(file);
            var addonComponents = (List<JsonAddonComponent>)JsonUtility.Deserialize<List<JsonAddonComponent>>(json);
            foreach (var a in addonComponents)
            {
                this.AddComponent(new AddonComponent(a.I), a.X, a.Y, a.Z);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            // probably gonna add some more nifty to this later, like the ability to add NPCs to the addon automatically and the like.
            base.Serialize(writer);

            writer.Write((int)1); // version
            writer.Write(_owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            _owner = version > 0 ? reader.ReadInt() : 0;
        }
    }
}

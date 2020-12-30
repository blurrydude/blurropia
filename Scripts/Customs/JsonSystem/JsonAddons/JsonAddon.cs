using System.Collections.Generic;
using System.IO;
using Server.Items;
using Server.Network;
using ServerUtilityExtensions;

namespace Server.Customs
{
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
            ClearAddon();
            LoadFromJson();
        }

        private void ClearAddon()
        {
            List<AddonComponent> toRemove = new List<AddonComponent>(Components);
            List<Item> itemsToRemove = new List<Item>(Items);

            foreach (var remove in toRemove)
            {
                Components.Remove(remove);
                remove.Delete();
            }
            foreach (var remove in itemsToRemove)
            {
                Items.Remove(remove);
                World.RemoveItem(remove);
                remove.Delete();
            }
        }

        private void LoadFromJson()
        {
            if (!Directory.Exists("Scripts/Customs/JsonSystem/JsonAddons/Data"))
            {
                Directory.CreateDirectory("Scripts/Customs/JsonSystem/JsonAddons/Data");
            }

            var file = $"Scripts/Customs/JsonSystem/JsonAddons/Data/{Name}.json";
            if (!File.Exists(file))
            {
                var newjson = "[{\"I\":41,\"X\":0,\"Y\":0,\"Z\":0}]";
                File.WriteAllText(file, newjson);
            }

            var json = File.ReadAllText(file);
            var addonComponents = (List<JsonAddonComponent>)JsonUtility.Deserialize<List<JsonAddonComponent>>(json);
            foreach (var a in addonComponents)
            {
                if(string.IsNullOrEmpty(a.T) || a.T.ToLower() == "static")
                {
                    AddComponent(new AddonComponent(a.I) { Hue = a.H??0 }, a.X, a.Y, a.Z);
                    continue;
                }
                var item = JsonSystemHelper.NewItemByTypeString(a.T);
                if (item == null) continue;
                ((Item)item).Hue = a.H??0;
                ((Item)item).Movable = false;
                AddItem((Item)item);
                ((Item)item).MoveToWorld(new Point3D(a.X + Location.X, a.Y + Location.Y, a.Z + Location.Z), Map);
            }
        }
        public override void OnDelete()
        {
            ClearAddon();
            base.OnDelete();
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

using System.Collections.Generic;
using System.IO;
using Server.Items;
using ServerUtilityExtensions;

namespace Server.Customs
{
    public class JsonAddon : BaseAddon
    {
        [Constructable]
        public JsonAddon()
        {
            Name = "default";
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

            if (!m.InRange(GetWorldLocation(), 12))
            {
                return;
            }

            if (e.Speech == "reload")
            {
                Reload();
            }

            if (e.Speech.Contains("setfile"))
            {
                Name = e.Speech.Replace("setfile ", string.Empty);
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
            if (!Directory.Exists("JsonAddons"))
            {
                Directory.CreateDirectory("JsonAddons");
            }

            var file = $"JsonAddons/{Name}.json";
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}

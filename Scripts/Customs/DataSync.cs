using Server.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.Commands;

namespace Server.Customs
{
    public class DataSyncCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("datasync", AccessLevel.GameMaster, new CommandEventHandler(DataSyncCommand_OnCommand));
        }

        [Usage("datasync")]
        [Description("Attempts data sync to Backpack folder")]
        public static void DataSyncCommand_OnCommand(CommandEventArgs arg)
        {
            DataSync.Sync();
        }
    }
    public static class DataSync
    {
        public static void Sync()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                //if (!Directory.Exists("Backpack"))
                //{
                //    Directory.CreateDirectory("Backpack");
                //}
                //if (!Directory.Exists("Backpack/Items"))
                //{
                //    Directory.CreateDirectory("Backpack/Items");
                //}
                //if (!Directory.Exists("Backpack/Mobiles"))
                //{
                //    Directory.CreateDirectory("Backpack/Mobiles");
                //}
                //if (!Directory.Exists("Backpack/Data"))
                //{
                //    Directory.CreateDirectory("Backpack/Data");
                //}

                var itemView = World.Items.Select(x => new
                {
                    x.Key.Value,
                    x.Value.Amount,
                    x.Value.Name,
                    Type = x.Value.GetType().ToString().Split('.').Last(),
                    Map = x.Value.Map.Name,
                    x.Value.Location
                });
                var itemJson = JsonConvert.SerializeObject(itemView);
                File.WriteAllText("ItemsView.json",itemJson);
                
                var mobileView = World.Mobiles.Select(x => new
                {
                    Serial = x.Key.Value,
                    Type = x.Value.GetType().ToString().Split('.').Last(),
                    Map = x.Value.Map?.Name,
                    Body = x.Value.Body.BodyID,
                    Location = x.Value.Location,
                    Account = x.Value.Account?.Username,
                    Name = x.Value.Name,
                    Alive = x.Value.Alive,
                    Hidden = x.Value.Hidden,
                    FireResistance = x.Value.FireResistance,
                    ColdResistance = x.Value.ColdResistance,
                    EnergyResistance = x.Value.EnergyResistance,
                    PhysicalResistance = x.Value.PhysicalResistance,
                    PoisonResistance = x.Value.PoisonResistance,
                    Backpack = x.Value.Backpack?.Items.Select(y => new DataSyncItemView(y.Name,y.Amount,y.Items)),
                    BaseColdResistance = x.Value.BaseColdResistance,
                    BaseEnergyResistance = x.Value.BaseEnergyResistance,
                    BaseFireResistance = x.Value.BaseFireResistance,
                    BasePhysicalResistance = x.Value.BasePhysicalResistance,
                    BasePoisonResistance = x.Value.BasePoisonResistance,
                    Dex = x.Value.Dex,
                    Fame = x.Value.Fame,
                    Hits = x.Value.Hits,
                    HitsMax = x.Value.HitsMax,
                    Int = x.Value.Int,
                    Karma = x.Value.Karma,
                    Luck = x.Value.Luck,
                    Mana = x.Value.Mana,
                    NameMod = x.Value.NameMod,
                    Race = x.Value.Race,
                    Skills = x.Value.Skills?.Select(s => new
                    {
                        s.Name,
                        s.Value
                    }),
                    Str = x.Value.Str,
                    TotalGold = x.Value.TotalGold,
                    Title = x.Value.Title,
                    TitleNam = x.Value.TitleName
                });
                var mobileJson = JsonConvert.SerializeObject(mobileView);
                File.WriteAllText("MobilesView.json",mobileJson);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            sw.Stop();
            Console.WriteLine($"Data synced in {sw.Elapsed}");
        }

        public class DataSyncItemView
        {
            public DataSyncItemView() {}

            public DataSyncItemView(string item, int amount, List<Item> children)
            {
                Item = item;
                Amount = amount;
                Items = new List<DataSyncItemView>();
                foreach (var child in children)
                {
                    Items.Add(new DataSyncItemView(child.Name,child.Amount,child.Items));
                }
            }

            public string Item { get; set; }
            public int Amount { get; set; }
            public List<DataSyncItemView> Items { get; set; }
        }
    }
}
//new
//                    {
//                        thing.Value.Name,
//                        thing.Value.Amount,
//                        //Items = thing.Value.Items.Select(x => x.Serial).ToList(),
//                        thing.Value.Serial,
//                        //BlessedFor = thing.Value.BlessedFor.Serial,
//                        thing.Value.BlocksFit,
//                        thing.Value.CanTarget,
//                        thing.Value.ColdResistance,
//                        thing.Value.DecayMultiplier,
//                        thing.Value.DecayTime,
//                        thing.Value.Decays,
//                        thing.Value.DefaultDecaySetting,
//                        thing.Value.DefaultName,
//                        thing.Value.Deleted,
//                        thing.Value.DefaultWeight,
//                        thing.Value.Direction,
//                        thing.Value.DisplayLootType,
//                        thing.Value.DisplayWeight,
//                        thing.Value.EnergyResistance,
//                        thing.Value.FireResistance,
//                        thing.Value.ForceShowProperties,
//                        thing.Value.GridLocation,
//                        thing.Value.Hue,
//                        thing.Value.HandlesOnMovement,
//                        thing.Value.HandlesOnSpeech,
//                        thing.Value.HeldBy, //
//                        thing.Value.HiddenQuestItemHue,
//                        thing.Value.HonestyItem,
//                        thing.Value.HuedItemID,
//                        thing.Value.InSecureTrade,
//                        thing.Value.Insured,
//                        thing.Value.IsArtifact,
//                        thing.Value.IsLockedDown,
//                        thing.Value.IsSecure,
//                        thing.Value.IsVirtualItem,
//                        //thing.Value.ItemData, //
//                        thing.Value.ItemID,
//                        thing.Value.LabelNumber,
//                        thing.Value.LastMoved,
//                        //thing.Value.Layer, //
//                        //thing.Value.Light, //
//                        thing.Value.Location,
//                        thing.Value.LootType,
//                        Map = thing.Value.Map.Name,
//                        //thing.Value.Modules, //
//                        thing.Value.Movable,
//                        thing.Value.NoMoveHS,
//                        thing.Value.Nontransferable,
//                        //thing.Value.OPLPacket, //
//                        //thing.Value.Parent, //
//                        //thing.Value.ParentEntity, //
//                        thing.Value.PayedInsurance,
//                        thing.Value.PhysicalResistance,
//                        thing.Value.PileWeight,
//                        thing.Value.PoisonResistance,
//                        //thing.Value.PropertyList, //
//                        thing.Value.QuestItem,
//                        thing.Value.QuestItemHue,
//                        //thing.Value.RemovePacket, //
//                        //thing.Value.RootParent, //
//                        //thing.Value.RootParentEntity, //
//                        thing.Value.SavedFlags,
//                        //thing.Value.Sockets, //
//                        //thing.Value.Spawner, //
//                        thing.Value.StackIgnoreHue,
//                        thing.Value.StackIgnoreItemID,
//                        thing.Value.StackIgnoreName,
//                        thing.Value.Stackable,
//                        thing.Value.TempFlags,
//                        thing.Value.TimeToDecay,
//                        thing.Value.TotalGold,
//                        thing.Value.TotalItems,
//                        thing.Value.TotalWeight,
//                        thing.Value.Visible,
//                        thing.Value.Weight,
//                        //thing.Value.WorldPacket, //
//                        //thing.Value.WorldPacketHS, //
//                        //thing.Value.WorldPacketSA, //
//                        thing.Value.X,
//                        thing.Value.Y,
//                        thing.Value.Z
//                    }

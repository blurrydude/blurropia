using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.Spells.Fourth;
using Server.Spells.Seventh;

namespace Server.Customs
{
    public class TravelBook : Item, ISecurable, ICraftable
	{
        public static readonly TimeSpan UseDelay = TimeSpan.FromSeconds(7.0);

        private BookQuality m_Quality;
		
        [CommandProperty(AccessLevel.GameMaster)]		
        public BookQuality Quality
        {
            get
            {
                return m_Quality;
            }
            set
            {
                m_Quality = value;
                InvalidateProperties();
            }
        }

        private List<RunebookEntry> m_Entries;
        private string m_Description;
        private int m_CurCharges, m_MaxCharges;
        private int m_DefaultIndex;
        private SecureLevel m_Level;
        private Mobile m_Crafter;
		
        private DateTime m_NextUse;
		
        private List<Mobile> m_Openers = new List<Mobile>();

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextUse
        {
            get
            {
                return m_NextUse;
            }
            set
            {
                m_NextUse = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return m_Crafter;
            }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return m_Level;
            }
            set
            {
                m_Level = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurCharges
        {
            get
            {
                return m_CurCharges;
            }
            set
            {
                m_CurCharges = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges
        {
            get
            {
                return m_MaxCharges;
            }
            set
            {
                m_MaxCharges = value;
            }
        }
		
        public List<Mobile> Openers
        {
            get
            {
                return m_Openers;
            }
            set
            {
                m_Openers = value;
            }
        }

        public virtual int MaxEntries { get { return 16; } }

        [Constructable]
        public TravelBook(int maxCharges, int id = 0x22C5)
            : base(Core.AOS ? id : 0xEFA)
        {
            Weight = (Core.SE ? 1.0 : 3.0);
            LootType = LootType.Blessed;
            Hue = 1151;
            Name = "Travel Book";

            Layer = (Core.AOS ? Layer.Invalid : Layer.OneHanded);

            m_Entries = new List<RunebookEntry>();

            m_MaxCharges = maxCharges;

            m_DefaultIndex = -1;

            m_Level = SecureLevel.CoOwners;
        }

        [Constructable]
        public TravelBook()
            : this(Core.SE ? 12 : 6)
        {
        }

        public List<RunebookEntry> Entries
        {
            get
            {
                return m_Entries;
            }
        }

        public int DefaultIndex
        {
            get { return m_DefaultIndex; }
            set { m_DefaultIndex = value; }
        }

        public RunebookEntry Default
        {
            get
            {
                if (m_DefaultIndex >= 0 && m_DefaultIndex < m_Entries.Count)
                    return m_Entries[m_DefaultIndex];

                return null;
            }
            set
            {
                if (value == null)
                    m_DefaultIndex = -1;
                else
                    m_DefaultIndex = m_Entries.IndexOf(value);
            }
        }

        public TravelBook(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            return true;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);

            writer.Write((byte)m_Quality);	

            writer.Write(m_Crafter);

            writer.Write((int)m_Level);

            writer.Write(m_Entries.Count);

            for (int i = 0; i < m_Entries.Count; ++i)
                m_Entries[i].Serialize(writer);

            writer.Write(m_Description);
            writer.Write(m_CurCharges);
            writer.Write(m_MaxCharges);
            writer.Write(m_DefaultIndex);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            LootType = LootType.Blessed;

            if (Core.SE && Weight == 3.0)
                Weight = 1.0;

            int version = reader.ReadInt();

            switch ( version )
            {
                case 3:
                    {
                        m_Quality = (BookQuality)reader.ReadByte();		
                        goto case 2;
                    }
                case 2:
                    {
                        m_Crafter = reader.ReadMobile();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        int count = reader.ReadInt();

                        m_Entries = new List<RunebookEntry>(count);

                        for (int i = 0; i < count; ++i)
                            m_Entries.Add(new RunebookEntry(reader));

                        m_Description = reader.ReadString();
                        m_CurCharges = reader.ReadInt();
                        m_MaxCharges = reader.ReadInt();
                        m_DefaultIndex = reader.ReadInt();

                        break;
                    }
            }
        }

        public void DropRune(Mobile from, RunebookEntry e, int index)
        {
            if (m_DefaultIndex > index)
                m_DefaultIndex -= 1;
            else if (m_DefaultIndex == index)
                m_DefaultIndex = -1;

            m_Entries.RemoveAt(index);

            if (e.Galleon != null)
            {
                if (e.Galleon.Deleted)
                {
                    from.SendMessage("You discard the rune as the galleon is no longer available.");
                    return;
                }
                else
                {
                    ShipRune rune = new ShipRune(e.Galleon);
                    from.AddToBackpack(rune);
                }
            }
            else
            {
                RecallRune rune = new RecallRune();

                rune.Target = e.Location;
                rune.TargetMap = e.Map;
                rune.Description = e.Description;
                rune.House = e.House;
                rune.Marked = true;
                rune.Hue = RecallRune.CalculateHue(e.Map, e.House, true);

                from.AddToBackpack(rune);
            }

            from.SendLocalizedMessage(502421); // You have removed the rune.
        }

        public bool IsOpen(Mobile toCheck)
        {
            NetState ns = toCheck.NetState;

            if (ns != null)
            {
                foreach (Gump gump in ns.Gumps)
                {
                    TravelBookGump bookGump = gump as TravelBookGump;

                    if (bookGump != null && bookGump.Book == this)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool DisplayLootType
        {
            get
            {
                return Core.AOS;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
		
            if (m_Quality == BookQuality.Exceptional)
                list.Add(1063341); // exceptional

            if (m_Crafter != null)
				list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (m_Description != null && m_Description.Length > 0)
                list.Add(m_Description);
        }
		
        public override bool OnDragLift(Mobile from)
        {
            if (from.HasGump(typeof(TravelBookGump)))
            {
                from.SendLocalizedMessage(500169); // You cannot pick that up.
                return false;
            }
			
            foreach (Mobile m in m_Openers)
                if (IsOpen(m))
                    m.CloseGump(typeof(TravelBookGump));
				
            m_Openers.Clear();
			
            return true;
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_Description != null && m_Description.Length > 0)
                LabelTo(from, m_Description);

            base.OnSingleClick(from);

            if (m_Crafter != null)
				LabelTo(from, 1050043, m_Crafter.TitleName);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), (Core.ML ? 3 : 1)) && CheckAccess(from))
            {
                if (RootParent is BaseCreature)
                {
                    from.SendLocalizedMessage(502402); // That is inaccessible.
                    return;
                }

                if (DateTime.UtcNow < m_NextUse)
                {
                    from.SendLocalizedMessage(502406); // This book needs time to recharge.
                    return;
                }

                from.CloseGump(typeof(TravelBookGump));
                from.SendGump(new TravelBookGump(this, 0));
				
                m_Openers.Add(from);
            }
        }

        public virtual void OnTravel()
        {
            if (!Core.SA)
                m_NextUse = DateTime.UtcNow + UseDelay;
        }

        public override void OnAfterDuped(Item newItem)
        {
            TravelBook book = newItem as TravelBook;

            if (book == null)
                return;

            book.m_Entries = new List<RunebookEntry>();

            for (int i = 0; i < m_Entries.Count; i++)
            {
                RunebookEntry entry = m_Entries[i];

                book.m_Entries.Add(new RunebookEntry(entry.Location, entry.Map, entry.Description, entry.House));
            }

            base.OnAfterDuped(newItem);
        }

        public bool CheckAccess(Mobile m)
        {
            if (!IsLockedDown || m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsAosRules && (house.Public ? house.IsBanned(m) : !house.HasAccess(m)))
                return false;

            return (house != null && house.HasSecureAccess(m, m_Level));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is RecallRune || dropped is ShipRune)
            {
                if (IsLockedDown && from.AccessLevel < AccessLevel.GameMaster)
                {
                    from.SendLocalizedMessage(502413, null, 0x35); // That cannot be done while the book is locked down.
                }
                else if (IsOpen(from))
                {
                    from.SendLocalizedMessage(1005571); // You cannot place objects in the book while viewing the contents.
                }
                else if (m_Entries.Count < MaxEntries)
                {
                    if (dropped is RecallRune)
                    {
                        RecallRune rune = (RecallRune)dropped;

                        if (rune.Marked && rune.TargetMap != null)
                        {
                            m_Entries.Add(new RunebookEntry(rune.Target, rune.TargetMap, rune.Description, rune.House));

                            dropped.Delete();

                            from.Send(new PlaySound(0x42, GetWorldLocation()));

                            string desc = rune.Description;

                            if (desc == null || (desc = desc.Trim()).Length == 0)
                                desc = "(indescript)";

                            from.SendMessage(desc);

                            return true;
                        }
                        else
                        {
                            from.SendLocalizedMessage(502409); // This rune does not have a marked location.
                        }
                    }
                    else if(dropped is ShipRune)
                    {
                        ShipRune rune = (ShipRune)dropped;

                        if (rune.Galleon != null && !rune.Galleon.Deleted)
                        {
                            m_Entries.Add(new RunebookEntry(Point3D.Zero, null, rune.Galleon.ShipName, null, rune.Galleon));

                            dropped.Delete();

                            from.Send(new PlaySound(0x42, GetWorldLocation()));

                            string desc = rune.Galleon.ShipName;

                            if (desc == null || (desc = desc.Trim()).Length == 0)
                                desc = "an unnamed ship";

                            from.SendMessage(desc);

                            return true;
                        }
                        else
                        {
                            if (rune.DockedBoat != null)
                                from.SendMessage("You cannot place a rune to a docked boat in the TravelBook.");
                            else
                                from.SendLocalizedMessage(502409); // This rune does not have a marked location.
                        }
                    }
                }
                else
                {
                    from.SendMessage("This Travel Book is full."); // 
                }
            }
            else if (dropped is RecallScroll)
            {
                if (m_CurCharges < m_MaxCharges)
                {
                    from.Send(new PlaySound(0x249, GetWorldLocation()));

                    int amount = dropped.Amount;

                    if (amount > (m_MaxCharges - m_CurCharges))
                    {
                        dropped.Consume(m_MaxCharges - m_CurCharges);
                        m_CurCharges = m_MaxCharges;
                    }
                    else
                    {
                        m_CurCharges += amount;
                        dropped.Delete();

                        return true;
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502410); // This book already has the maximum amount of charges.
                }
            }
            else if (dropped is Gold)
            {
                if (m_CurCharges < m_MaxCharges) {
                    int amount = dropped.Amount;
                    if (amount < 10)
                    {
                        from.SendMessage("You need at lesat 10 gold to charge this book.");
                    }
                    else
                    {
                        int charge = amount / 10;
                        int chargable = m_MaxCharges - m_CurCharges;
                        int consume = Math.Min(charge, chargable);
                        dropped.Consume(consume * 10);
                        m_CurCharges += charge;
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502410); // This book already has the maximum amount of charges.
                }
            }

            return false;
        }

        #region ICraftable Members

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            int charges = 5 + quality + (int)(from.Skills[SkillName.Inscribe].Value / 30);

            if (charges > 10)
                charges = 10;

            MaxCharges = (Core.SE ? charges * 2 : charges);

            if (makersMark)
                Crafter = from;

            m_Quality = (BookQuality)(quality - 1);

            return quality;
        }
        #endregion
    }

    public class TravelBookGump : Gump
    {
        public TravelBook Book;
        private int _page;
        private int _pageLimit;
        private int? _selection;
        public TravelBookGump(TravelBook book, int page, int? selection = null)
            : base(100,100)
        {
            Book = book;
            _page = page;
            _selection = selection;
            _pageLimit = (int)Math.Ceiling(Book.Entries.Count() / 15m) - 1;
            AddImage(0, 0, 39923);
            if (_page > 0 && _pageLimit > 0)
            {
                AddButton(24, 4, 2205, 2205, 1, GumpButtonType.Reply, 0);
            }

            if (_page < _pageLimit)
            {
                AddButton(374, 4, 2206, 2206, 2, GumpButtonType.Reply, 0);
            }

            AddButton(248, 305, 2284, 2284, 3, GumpButtonType.Reply, 0);
            if (_selection != null)
            {
                AddButton(294, 305, 2271, 2271, 4, GumpButtonType.Reply, 0);
                AddButton(340, 305, 2291, 2291, 5, GumpButtonType.Reply, 0);
                AddLabel(265, 96, 0, "Location selected");
                AddLabel(273, 116, 0, "Select a spell");
                AddImage(240,140,1590);
                AddTextEntry(250,140,100,25,0,1,Book.Entries[_selection??0].Description);
                AddButton(250, 170, 239, 240, 6, GumpButtonType.Reply, 0);
            }
            else
            {
                AddLabel(265, 96, 0, "Select a spell");
            }

            AddLabel(288, 48, 0, "Bagheera's");
            AddLabel(282, 64, 0, "Travel Book");
            AddLabel(248, 285, 0, $"Charges: {Book.CurCharges}");

            var i = 100;
            var y = 34;
            foreach (var entry in Book.Entries.Skip(_page*15).Take(15))
            {
                AddButton(38, y, 2224, 2224, i, GumpButtonType.Reply, 0);
                AddLabel(60, y, GetMapHue(entry.Map), entry.Description);
                AddButton(185, y, 11411, 11412, i+100, GumpButtonType.Reply, 0);
                i++;
                y += 16;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0) return;
            Mobile from = sender.Mobile;
            switch (info.ButtonID)
            {
                case 1: // previous page
                    if (_page > 0)
                    {
                        from.CloseGump(typeof(TravelBookGump));
                        from.SendGump(new TravelBookGump(Book, _page-1, _selection));
                    }
                    break;
                case 2: // next page
                    if (_page < _pageLimit)
                    {
                        from.CloseGump(typeof(TravelBookGump));
                        from.SendGump(new TravelBookGump(Book, _page+1, _selection));
                    }
                    break;
                case 3: // mark
                    //TODO: make sure to add rune in bag check and consume on mark
                    if (Book.CurCharges <= 0)
                    {
                        from.CloseGump(typeof(TravelBookGump));
                        from.SendGump(new TravelBookGump(Book, _page, _selection));

                        from.SendLocalizedMessage(502412); // There are no charges left on that item.
                    }
                    else
                    {
                        bool setDesc = false;
                        var loc = from.Location;
                        var map = from.Map;
                        var desc = string.Empty;
                        var house = Core.AOS ? BaseHouse.FindHouseAt(from) : null;
                        if (house != null)
                        {
                            HouseSign sign = house.Sign;

                            if (sign != null)
                                desc = sign.Name;

                            if (string.IsNullOrEmpty(desc) || (desc = desc.Trim()).Length == 0)
                                desc = "an unnamed house";

                            setDesc = true;

                            int x = house.BanLocation.X;
                            int y = house.BanLocation.Y + 2;
                            int z = house.BanLocation.Z;

                            Map hmap = house.Map;

                            if (hmap != null && !hmap.CanFit(x, y, z, 16, false, false))
                                z = hmap.GetAverageZ(x, y);

                            loc = new Point3D(x, y, z);
                            map = hmap;
                        }

                        if (!setDesc)
                            desc = BaseRegion.GetRuneNameFor(Region.Find(loc, map));

                        if (string.IsNullOrEmpty(desc)) desc = $"location #{Book.Entries.Count}";

                        var newEntry = new RunebookEntry(from.Location, from.Map, desc, null);
                        Book.Entries.Add(newEntry);
                        --Book.CurCharges;
                        from.CloseGump(typeof(TravelBookGump));
                        from.SendGump(new TravelBookGump(Book, _page, _selection));
                    }

                    break;
                case 4: // recall
                case 5: // gate travel
                    if (_selection == null)
                    {
                        from.CloseGump(typeof(TravelBookGump));
                        from.SendGump(new TravelBookGump(Book, _page, _selection));

                        from.SendMessage("You must select a location first.");
                    }
                    else if (Book.CurCharges <= 0)
                    {
                        from.CloseGump(typeof(TravelBookGump));
                        from.SendGump(new TravelBookGump(Book, _page, _selection));

                        from.SendLocalizedMessage(502412); // There are no charges left on that item.
                    }
                    else
                    {
                        var e = Book.Entries[_selection??0];
                        int xLong = 0, yLat = 0;
                        int xMins = 0, yMins = 0;
                        bool xEast = false, ySouth = false;

                        if (Sextant.Format(e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                        {
                            string location = String.Format("{0}o {1}'{2}, {3}o {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                            from.SendMessage(location);
                        }

                        Book.OnTravel();
                        if (info.ButtonID == 4)
                        {
                            new RecallSpell(from, null, e, null).Cast();
                        }
                        else
                        {
                            new GateTravelSpell(from, null, e).Cast();
                        }

                        --Book.CurCharges;
                        Book.Openers.Remove(from);
                    }
                    break;
                case 6:
                    Book.Entries[_selection??0].Description = info.TextEntries.First(x => x.EntryID == 1).Text;
                    from.CloseGump(typeof(TravelBookGump));
                    from.SendGump(new TravelBookGump(Book, _page, _selection));
                    break;
                default:
                    var entryId = info.ButtonID - 100;
                    if (entryId >= 100)
                    {
                        var idx = entryId - 100;
                        Book.DropRune(from, Book.Entries[idx], idx);
                        from.CloseGump(typeof(TravelBookGump));
                        from.SendGump(new TravelBookGump(Book, 0));
                        break;
                    }

                    from.CloseGump(typeof(TravelBookGump));
                    from.SendGump(new TravelBookGump(Book, _page, entryId));
                    break;
            }
        }

        public static int GetMapHue(Map map)
        {
            if (map == Map.Trammel)
                return 10;
            if (map == Map.Felucca)
                return 81;
            if (map == Map.Ilshenar)
                return 1102;
            if (map == Map.Malas)
                return 1102;
            if (map == Map.Tokuno)
                return 1154;
            if (map == Map.TerMur)
                return 1645;

            return 0;
        }
    }
}

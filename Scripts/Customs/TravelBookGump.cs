using System;
using System.Linq;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.Spells.Fourth;
using Server.Spells.Seventh;

namespace Server.Customs
{
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

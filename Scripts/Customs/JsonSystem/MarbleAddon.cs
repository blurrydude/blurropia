using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Items;
using ServerUtilityExtensions;

namespace Server.Customs
{
    public class MarbleAddon : BaseAddon
    {
        private int _width;
        private int _length;
        private int _stories;
        private bool _withBase;
        private int _wall_tlc;
        private int _wall_btw;
        private int _wall_bt;
        private int _wall_lr;
        private int _wall_lrw;
        private int _wall_brc;
        private int _roofedge_tlc;
        private int _roofedge_bt;
        private int _roofedge_lr;
        private int _roofedge_brc;
        private int _floor;
        private int _base_tl;
        private int _base_tr;
        private int _base_bl;
        private int _base_br;
        private int _base_t;
        private int _base_b;
        private int _base_l;
        private int _base_r;
        private int _base_f;
        private bool _firstLoad;
        private bool _doorEast;
        private bool _stairsEast;
        private TilePackType _tilePack;

        [CommandProperty(AccessLevel.Player)]
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Length
        {
            get => _length;
            set
            {
                _length = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Stories
        {
            get => _stories;
            set
            {
                _stories = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool WithBase
        {
            get => _withBase;
            set
            {
                _withBase = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DoorEast
        {
            get => _doorEast;
            set
            {
                _doorEast = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool StairsEast
        {
            get => _stairsEast;
            set
            {
                _stairsEast = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WallNorthwestCorner
        {
            get => _wall_tlc;
            set
            {
                _wall_tlc = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WallSoutheastCorner
        {
            get => _wall_brc;
            set
            {
                _wall_brc = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WallEast
        {
            get => _wall_lr;
            set
            {
                _wall_lr = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WallSouth
        {
            get => _wall_bt;
            set
            {
                _wall_bt = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WallEastWindowed
        {
            get => _wall_lrw;
            set
            {
                _wall_lrw = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WallSouthWindowed
        {
            get => _wall_btw;
            set
            {
                _wall_btw = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RoofEdgeNorthwestCorner
        {
            get => _roofedge_tlc;
            set
            {
                _roofedge_tlc = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RoofEdgeSoutheastCorner
        {
            get => _roofedge_brc;
            set
            {
                _roofedge_brc = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RoofEdgeEast
        {
            get => _roofedge_lr;
            set
            {
                _roofedge_lr = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RoofEdgeSouth
        {
            get => _roofedge_bt;
            set
            {
                _roofedge_bt = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Floor
        {
            get => _floor;
            set
            {
                _floor = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseNorthwest
        {
            get => _base_tl;
            set
            {
                _base_tl = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseNortheast
        {
            get => _base_tr;
            set
            {
                _base_tr = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseSouthwest
        {
            get => _base_bl;
            set
            {
                _base_bl = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseSoutheast
        {
            get => _base_br;
            set
            {
                _base_br = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseNorth
        {
            get => _base_t;
            set
            {
                _base_t = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseSouth
        {
            get => _base_b;
            set
            {
                _base_b = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseWest
        {
            get => _base_l;
            set
            {
                _base_l = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseEast
        {
            get => _base_r;
            set
            {
                _base_r = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseFill
        {
            get => _base_f;
            set
            {
                _base_f = value;
                Reload();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TilePackType TilePack
        {
            get => _tilePack;
            set
            {
                _tilePack = value;
                LoadTilePack();
                Reload();
            }
        }



        [Constructable]
        public MarbleAddon()
        {
            _width = 6;
            _length = 6;
            _stories = 1;
            _withBase = true;
            _wall_tlc = 257;
            _wall_btw = 258;
            _wall_bt = 255;
            _wall_lr = 256;
            _wall_lrw = 259;
            _wall_brc = 254;
            _roofedge_tlc = 273;
            _roofedge_bt = 271;
            _roofedge_lr = 272;
            _roofedge_brc = 270;
            _floor = 1294;
            _base_tl = 1810;
            _base_tr = 1813;
            _base_bl = 1812;
            _base_br = 1811;
            _base_t = 1804;
            _base_b = 1802;
            _base_l = 1805;
            _base_r = 1803;
            _base_f = 1801;
            _firstLoad = true;
            _doorEast = false;
            _stairsEast = false;
            AddComponent(new AddonComponent(1801), 0, 0, 0);
        }

        public MarbleAddon(Serial serial)
            : base(serial)
        {
        }

        public void LoadTilePack()
        {
            var set = new [] {   257,  258,  255,  256,  259,  254,  273,  271,  272,  270, 1294, 1810, 1813, 1812, 1811, 1804, 1802, 1805, 1803, 1801 };
            switch (_tilePack)
            {
                case TilePackType.GreyBrick: set = new [] {54,59,52,53,60,51,68,66,67,65,1301,1853,1855,1856,1854,1851,1849,1852,1850,1848}; break;
                case TilePackType.LightWood: set = new [] {169,186,168,167,185,166,192,191,190,189,1208,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                case TilePackType.DarkWood: set = new [] {9,14,7,8,15,6,23,22,21,20,1301,1960,1962,1963,1961,1958,1956,1959,1957,1955}; break;
                case TilePackType.Stone: set = new [] {29,34,28,27,35,26,48,47,46,45,1208,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                case TilePackType.StoneBlock: set = new [] {90,94,88,87,93,89,108,105,106,107,1301,1960,1962,1963,1961,1958,1956,1959,1957,1955}; break;
                case TilePackType.Log: set = new [] {147,152,146,145,153,144,157,156,155,154,1208,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                case TilePackType.LightWoodBanded: set = new [] {177,188,175,176,187,174,192,191,190,189,1301,1830,1832,1833,1831,1828,1826,1829,1827,1825}; break;
                case TilePackType.LightHeavyStone: set = new [] {204,202,200,201,203,199,223,222,221,220,1208,1960,1962,1963,1961,1958,1956,1959,1957,1955}; break;
                case TilePackType.OrnateWhiteMarble: set = new [] {251,252,249,250,253,248,273,271,272,270,1301,1806,1808,1809,1807,1804,1802,1805,1803,1801}; break;
                case TilePackType.TrimmedWhiteMarble: set = new [] {257,258,255,256,259,254,282,280,281,279,1208,1806,1808,1809,1807,1804,1802,1805,1803,1801}; break;
                case TilePackType.PlainWhiteMarble: set = new [] {263,264,261,262,265,260,282,280,281,279,1301,1806,1808,1809,1807,1804,1802,1805,1803,1801}; break;
                case TilePackType.TimberedPlainPlaster: set = new [] {298,314,310,311,315,306,324,322,323,321,1208,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                case TilePackType.PlainPlaster: set = new [] {298,314,310,311,315,309,324,322,323,321,1301,1853,1855,1856,1854,1851,1849,1852,1850,1848}; break;
                case TilePackType.OrnatePlaster: set = new [] {298,342,338,339,343,332,324,322,323,321,1208,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                case TilePackType.OrnateSandBlock: set = new [] {347,348,345,346,349,344,359,357,358,356,1301,1905,1907,1908,1906,1903,1901,1904,1902,1900}; break;
                case TilePackType.PlainSandBlock: set = new [] {353,355,352,351,354,350,363,362,361,360,1208,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                case TilePackType.HeavyStone: set = new [] {466,467,464,465,468,463,491,489,490,488,1301,1933,1935,1936,1934,1931,1929,1932,1930,1928}; break;
                case TilePackType.FormedSand: set = new [] {603,595,589,590,596,588,192,191,190,189,1208,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                case TilePackType.PlainBlueMarble: set = new [] {660,661,658,659,662,657,696,694,695,693,1301,1853,1855,1856,1854,1851,1849,1852,1850,1848}; break;
                case TilePackType.TrimmedBlueMarble: set = new [] {666,667,664,665,668,663,696,694,695,693,1208,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                case TilePackType.OrnateBlueMarble: set = new [] {672,686,670,671,685,669,700,698,699,697,1301,1853,1855,1856,1854,1851,1849,1852,1850,1848}; break;
                case TilePackType.WornPlaster: set = new [] {898,314,910,911,315,909,927,925,926,924,1208,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                case TilePackType.HeavilyWornPlaster: set = new [] {898,314,915,916,315,914,927,925,926,924,1301,1853,1855,1856,1854,1851,1849,1852,1850,1848}; break;
                case TilePackType.SmoothSandstone: set = new [] {988,990,986,987,991,985,956,954,953,952,1183,1011,1013,1014,1012,1009,1007,1010,1008,1006}; break;
                case TilePackType.HalfSmoothSandstone: set = new [] {982,990,980,981,991,979,962,960,961,958,1181,1011,1013,1014,1012,1009,1007,1010,1008,1006}; break;
                case TilePackType.Sandstone: set = new [] {970,990,968,969,991,967,978,976,977,975,1182,1015,1018,1017,1016,1009,1007,1010,1008,1006}; break;
                case TilePackType.TokunoWooden: set = new [] {263,10341,10338,10342,10345,10335,10401,10340,10344,10337,10350,1853,1855,1856,1854,1851,1849,1852,1850,1848}; break;
                case TilePackType.DarkClothFrame: set = new [] {10381,10388,10387,10384,10389,10378,10379,10385,10382,10376,10362,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                case TilePackType.LightClothFrame: set = new [] {10403,10399,10395,10398,10400,10392,10401,10393,10396,10390,10362,1853,1855,1856,1854,1851,1849,1852,1850,1848}; break;
                case TilePackType.DarkClay: set = new [] {10563,10560,10560,10557,10557,10554,10561,10558,10555,10552,10616,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                case TilePackType.RoughClay: set = new [] {10572,10575,10575,10569,10569,10566,10570,10573,10567,10564,10599,1853,1855,1856,1854,1851,1849,1852,1850,1848}; break;
                case TilePackType.LightClay: set = new [] {10587,10581,10581,10578,10578,10584,10585,10579,10576,10582,10592,1877,1879,1880,1878,1875,1873,1876,1874,1872}; break;
                default: set = new [] {   257,  258,  255,  256,  259,  254,  273,  271,  272,  270, 1294, 1810, 1813, 1812, 1811, 1804, 1802, 1805, 1803, 1801 }; break;
            }

            _wall_tlc =     set[0];
            _wall_btw =     set[1];
            _wall_bt =      set[2];
            _wall_lr =      set[3];
            _wall_lrw =     set[4];
            _wall_brc =     set[5];
            _roofedge_tlc = set[6];
            _roofedge_bt =  set[7];
            _roofedge_lr =  set[8];
            _roofedge_brc = set[9];
            _floor =        set[10];
            _base_tl =      set[11];
            _base_tr =      set[12];
            _base_bl =      set[13];
            _base_br =      set[14];
            _base_t =       set[15];
            _base_b =       set[16];
            _base_l =       set[17];
            _base_r =       set[18];
            _base_f =       set[19];
        }

        public override void OnDoubleClick(Mobile @from) {
            if(_firstLoad) {
                Reload();
                _firstLoad = false;
            }
        }

        private void Reload()
        {
            List<AddonComponent> toRemove = new List<AddonComponent>(Components);

            foreach (var remove in toRemove)
            {
                Components.Remove(remove);
                remove.Delete(true);
            }

            foreach (var a in GetAddon(_width,_length,_stories,_withBase))
            {
                AddComponent(new AddonComponent(a.I), a.X, a.Y, a.Z);
            }
        }

        public List<JsonAddonComponent> GetAddon(int width, int length, int stories, bool withBase)
        {
            var z_mod = withBase ? 5 : 0;

            var addon = new List<JsonAddonComponent>();
            var doubledoor = width % 2 == 1;
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < length; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        var z = 0;
                        for (var f = 0; f < stories; f++)
                        {
                            addon.Add(new JsonAddonComponent(_wall_tlc, x, y, z + z_mod));
                            z += 20;
                        }

                        addon.Add(new JsonAddonComponent(_roofedge_tlc, x, y, z + z_mod));
                    }
                    else if (x == width - 1 && y == length - 1)
                    {
                        var z = 0;
                        for (var f = 0; f < stories; f++)
                        {
                            addon.Add(new JsonAddonComponent(_wall_brc, x, y, z + z_mod));
                            z += 20;
                        }

                        addon.Add(new JsonAddonComponent(_roofedge_brc, x, y, z + z_mod));
                    }
                    else if (x == 0 && y == length - 1)
                    {
                        var z = 0;
                        for (var f = 0; f < stories; f++)
                        {
                            addon.Add(new JsonAddonComponent(_wall_lr, x, y, z + z_mod));
                            z += 20;
                        }

                        addon.Add(new JsonAddonComponent(_roofedge_lr, x, y, z + z_mod));
                    }
                    else if (!_doorEast && y == length - 1 && (x == width / 2 || (doubledoor && x == (width / 2) + 1)))
                    {
                        var z = 20;
                        for (var f = 1; f < stories; f++)
                        {
                            addon.Add(new JsonAddonComponent(_wall_bt, x, y, z + z_mod));
                            z += 20;
                        }

                        addon.Add(new JsonAddonComponent(_roofedge_bt, x, y, z + z_mod));
                    }
                    else if (_doorEast && x == width - 1 && (y == length / 2 || (doubledoor && y == (length / 2) + 1)))
                    {
                        var z = 20;
                        for (var f = 1; f < stories; f++)
                        {
                            addon.Add(new JsonAddonComponent(_wall_lr, x, y, z + z_mod));
                            z += 20;
                        }

                        addon.Add(new JsonAddonComponent(_roofedge_lr, x, y, z + z_mod));
                    }
                    else if (y == 0 || y == length - 1)
                    {
                        var tile = (x == 2 || x == width - 2) ? _wall_btw : _wall_bt;
                        var z = 0;
                        for (var f = 0; f < stories; f++)
                        {
                            addon.Add(new JsonAddonComponent(tile, x, y, z + z_mod));
                            z += 20;
                        }

                        addon.Add(new JsonAddonComponent(_roofedge_bt, x, y, z + z_mod));
                    }
                    else if (x == 0 || x == width - 1)
                    {
                        var tile = (y == 2 || y == length - 2) ? _wall_lrw : _wall_lr;
                        var z = 0;
                        for (var f = 0; f < stories; f++)
                        {
                            addon.Add(new JsonAddonComponent(tile, x, y, z + z_mod));
                            z += 20;
                        }

                        addon.Add(new JsonAddonComponent(_roofedge_lr, x, y, z + z_mod));
                    }
                }
            }

            for (var x = 1; x < width; x++)
            {
                for (var y = 1; y < length; y++)
                {
                    var z = (withBase ? 20 : 0);
                    for (var f = (withBase ? 1 : 0); f <= stories; f++)
                    {
                        addon.Add(new JsonAddonComponent(_floor, x, y, z + z_mod));
                        z += 20;
                    }
                }
            }

            if (withBase)
            {
                for (var x = 0; x <= width; x++)
                {
                    for (var y = 0; y <= length; y++)
                    {
                        if (x == 0 && y == 0) addon.Add(new JsonAddonComponent(_base_tl, x, y, 0));
                        else if (x == 0 && y == length)
                            addon.Add(new JsonAddonComponent(_base_bl, x, y, 0));
                        else if (x == width && y == length)
                            addon.Add(new JsonAddonComponent(_base_br, x, y, 0));
                        else if (x == width && y == 0)
                            addon.Add(new JsonAddonComponent(_base_tr, x, y, 0));
                        else if (x == 0)
                            addon.Add(new JsonAddonComponent(_base_l, x, y, 0));
                        else if (x == width)
                            addon.Add(new JsonAddonComponent(_base_r, x, y, 0));
                        else if (y == 0)
                            addon.Add(new JsonAddonComponent(_base_t, x, y, 0));
                        else if (y == length)
                            addon.Add(new JsonAddonComponent(_base_b, x, y, 0));
                        else
                            addon.Add(new JsonAddonComponent(_base_f, x, y, 0));
                    }
                }
            }

            if (stories > 1)
            {
                var northfootprint = new List<Point2D>
                {
                    new Point2D(width/2,3),
                    new Point2D(width/2,4),
                    new Point2D(width/2,5),
                    new Point2D(width/2,6)
                };
                if (doubledoor)
                {
                    northfootprint.Add(new Point2D(width/2+1,3));
                    northfootprint.Add(new Point2D(width/2+1,4));
                    northfootprint.Add(new Point2D(width/2+1,5));
                    northfootprint.Add(new Point2D(width/2+1,6));
                }
                var southfootprint = new List<Point2D>
                {
                    new Point2D(width/2,length-3),
                    new Point2D(width/2,length-4),
                    new Point2D(width/2,length-5),
                    new Point2D(width/2,length-6)
                };
                if (doubledoor)
                {
                    southfootprint.Add(new Point2D(width/2+1,length-3));
                    southfootprint.Add(new Point2D(width/2+1,length-4));
                    southfootprint.Add(new Point2D(width/2+1,length-5));
                    southfootprint.Add(new Point2D(width/2+1,length-6));
                }
                var eastfootprint = new List<Point2D>
                {
                    new Point2D(width-3,length/2),
                    new Point2D(width-4,length/2),
                    new Point2D(width-5,length/2),
                    new Point2D(width-6,length/2)
                };
                if (doubledoor)
                {
                    eastfootprint.Add(new Point2D(width-3,length/2+1));
                    eastfootprint.Add(new Point2D(width-4,length/2+1));
                    eastfootprint.Add(new Point2D(width-5,length/2+1));
                    eastfootprint.Add(new Point2D(width-6,length/2+1));
                }
                var westfootprint = new List<Point2D>
                {
                    new Point2D(3,length/2),
                    new Point2D(4,length/2),
                    new Point2D(5,length/2),
                    new Point2D(6,length/2)
                };
                if (doubledoor)
                {
                    westfootprint.Add(new Point2D(3,length/2+1));
                    westfootprint.Add(new Point2D(4,length/2+1));
                    westfootprint.Add(new Point2D(5,length/2+1));
                    westfootprint.Add(new Point2D(6,length/2+1));
                }

                if (!_stairsEast)
                {
                    for (var f = 1; f < stories; f++)
                    {
                        if (f % 2 == 0)
                            addon.RemoveAll(x =>
                                southfootprint.Contains(new Point2D(x.X, x.Y)) && x.Z == f * 20 + (_withBase ? 5 : 0));
                        else
                            addon.RemoveAll(x =>
                                northfootprint.Contains(new Point2D(x.X, x.Y)) && x.Z == f * 20 + (_withBase ? 5 : 0));
                    }

                    for (var f = 0; f < stories - 1; f++)
                    {
                        if (f % 2 == 0)
                            addon.AddRange(BuildStairs(northfootprint, f * 20 + (_withBase ? 5 : 0), _base_b));
                        else addon.AddRange(BuildStairs(southfootprint, f * 20 + (_withBase ? 5 : 0), _base_t));
                    }
                }
                else
                {
                    for (var f = 1; f < stories; f++)
                    {
                        if(f % 2 == 0) addon.RemoveAll(x => eastfootprint.Contains(new Point2D(x.X, x.Y)) && x.Z == f*20+(_withBase?5:0));
                        else addon.RemoveAll(x => westfootprint.Contains(new Point2D(x.X, x.Y)) && x.Z == f*20+(_withBase?5:0));
                    }
                    for (var f = 0; f < stories-1; f++)
                    {
                        if (f % 2 == 0) addon.AddRange(BuildStairs(westfootprint,f*20+(_withBase?5:0),_base_r));
                        else addon.AddRange(BuildStairs(eastfootprint,f*20+(_withBase?5:0),_base_l));
                    }
                }
            }

            foreach (var c in addon)
            {
                c.Y -= length;
            }

            return addon;
        }

        private List<JsonAddonComponent> BuildStairs(List<Point2D> footprint, int z, int stairid)
        {
            var addon = new List<JsonAddonComponent>();
            var c = 0;
            footprint.Reverse();
            foreach (var p in footprint)
            {
                var q = c % 4;
                for (var t = 0; t < q; t++)
                {
                    addon.Add(new JsonAddonComponent(_base_f, p.X, p.Y, z+t*5));
                }
                addon.Add(new JsonAddonComponent(stairid, p.X, p.Y, z+q*5));

                c++;
            }
            footprint.Reverse();

            return addon;
        }

        public override void Serialize(GenericWriter writer)
        {
            // probably gonna add some more nifty to this later, like the ability to add NPCs to the addon automatically and the like.
            base.Serialize(writer);

            writer.Write((int)3); // version
            writer.Write(_width);
            writer.Write(_length);
            writer.Write(_stories);
            writer.Write(_withBase);
            writer.Write(_wall_tlc);
            writer.Write(_wall_btw);
            writer.Write(_wall_bt);
            writer.Write(_wall_lr);
            writer.Write(_wall_lrw);
            writer.Write(_wall_brc);
            writer.Write(_roofedge_tlc);
            writer.Write(_roofedge_bt);
            writer.Write(_roofedge_lr);
            writer.Write(_roofedge_brc);
            writer.Write(_floor);
            writer.Write(_base_tl);
            writer.Write(_base_tr);
            writer.Write(_base_bl);
            writer.Write(_base_br);
            writer.Write(_base_t);
            writer.Write(_base_b);
            writer.Write(_base_l);
            writer.Write(_base_r);
            writer.Write(_base_f);
            writer.Write(_firstLoad);
            writer.Write(_doorEast);
            writer.Write(_stairsEast);
            writer.Write((int)_tilePack);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            _width = reader.ReadInt();
            _length = reader.ReadInt();
            _stories = reader.ReadInt();
            _withBase = reader.ReadBool();
            _wall_tlc = reader.ReadInt();
            _wall_btw = reader.ReadInt();
            _wall_bt = reader.ReadInt();
            _wall_lr = reader.ReadInt();
            _wall_lrw = reader.ReadInt();
            _wall_brc = reader.ReadInt();
            _roofedge_tlc = reader.ReadInt();
            _roofedge_bt = reader.ReadInt();
            _roofedge_lr = reader.ReadInt();
            _roofedge_brc = reader.ReadInt();
            _floor = reader.ReadInt();
            _base_tl = reader.ReadInt();
            _base_tr = reader.ReadInt();
            _base_bl = reader.ReadInt();
            _base_br = reader.ReadInt();
            _base_t = reader.ReadInt();
            _base_b = reader.ReadInt();
            _base_l = reader.ReadInt();
            _base_r = reader.ReadInt();
            _base_f = reader.ReadInt();
            _firstLoad = reader.ReadBool();
            if (version > 1)
            {
                _doorEast = reader.ReadBool();
                _stairsEast = reader.ReadBool();
            }

            if (version > 2)
            {
                _tilePack = (TilePackType) reader.ReadInt();
            }
        }
    }

    public class TilePack
    {
        public TilePack()
        {
        }

        public TilePack(int wnc, int wsw, int ws, int we, int wew, int wsc, int rnc, int rs, int re, int rsc, int f,
            int bnwc, int bnec, int bswc, int bsec, int bn, int bs, int bw, int be, int bf)
        {
            WallNorthwestCorner = wnc;
            WallSouthWindowed = wsw;
            WallSouth = ws;
            WallEast = we;
            WallEastWindowed = wew;
            WallSoutheastCorner = wsc;
            RoofedgeNorthwestCorner = rnc;
            RoofedgeSouth = rs;
            RoofedgeEast = re;
            RoofedgeSoutheastCorner = rsc;
            Floor = f;
            BaseNorthwestCorner = bnwc;
            BaseNortheastCorner = bnec;
            BaseSouthwestCorner = bswc;
            BaseSoutheastCorner = bsec;
            BaseNorth = bn;
            BaseSouth = bs;
            BaseWest = bw;
            BaseEast = be;
            BaseFill = bf;
        }

        public int WallNorthwestCorner { get; set; }
        public int WallSouthWindowed { get; set; }
        public int WallSouth { get; set; }
        public int WallEast { get; set; }
        public int WallEastWindowed { get; set; }
        public int WallSoutheastCorner { get; set; }
        public int RoofedgeNorthwestCorner { get; set; }
        public int RoofedgeSouth { get; set; }
        public int RoofedgeEast { get; set; }
        public int RoofedgeSoutheastCorner { get; set; }
        public int Floor { get; set; }
        public int BaseNorthwestCorner { get; set; }
        public int BaseNortheastCorner { get; set; }
        public int BaseSouthwestCorner { get; set; }
        public int BaseSoutheastCorner { get; set; }
        public int BaseNorth { get; set; }
        public int BaseSouth { get; set; }
        public int BaseWest { get; set; }
        public int BaseEast { get; set; }
        public int BaseFill { get; set; }
    }

    public enum TilePackType
    {
        GreyBrick,
        LightWood,
        DarkWood,
        Stone,
        StoneBlock,
        Log,
        LightWoodBanded,
        LightHeavyStone,
        OrnateWhiteMarble,
        TrimmedWhiteMarble,
        PlainWhiteMarble,
        TimberedPlainPlaster,
        PlainPlaster,
        OrnatePlaster,
        OrnateSandBlock,
        PlainSandBlock,
        HeavyStone,
        FormedSand,
        PlainBlueMarble,
        TrimmedBlueMarble,
        OrnateBlueMarble,
        WornPlaster,
        HeavilyWornPlaster,
        SmoothSandstone,
        HalfSmoothSandstone,
        Sandstone,
        TokunoWooden,
        DarkClothFrame,
        LightClothFrame,
        DarkClay,
        RoughClay,
        LightClay
    }
}

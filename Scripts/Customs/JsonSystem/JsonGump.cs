using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Server.Gumps;

namespace Server.Customs
{
    public class JsonGump : Gump
    {
        public List<JsonGumpData> GumpData;

        public JsonGump(string configFilePath)
            : base(42, 42)
        {
            try
            {
                AddPage(0);

                var json = File.ReadAllText(configFilePath);
                GumpData = JsonConvert.DeserializeObject<List<JsonGumpData>>(json);

                foreach (var gd in GumpData)
                {
                    switch (gd.T)
                    {
                        case JsonGumpDataType.Image:
                            if (gd.U > 0)
                            {
                                AddImage(gd.X, gd.Y, gd.G, gd.U);
                                continue;
                            }

                            AddImage(gd.X, gd.Y, gd.G);
                            break;
                        case JsonGumpDataType.Background:
                            AddBackground(gd.X, gd.Y, gd.W, gd.H, gd.G);
                            break;
                        case JsonGumpDataType.Tiled:
                            AddImageTiled(gd.X, gd.Y, gd.W, gd.H, gd.G);
                            break;
                        case JsonGumpDataType.Button:
                            AddButton(gd.X, gd.Y, gd.G, gd.P, Convert.ToInt32(gd.V), GumpButtonType.Reply, 0);
                            break;
                        case JsonGumpDataType.Html:
                            AddHtml(gd.X, gd.Y, gd.W, gd.H, gd.V, false, true);
                            break;
                        case JsonGumpDataType.Item:
                            if (gd.U > 0)
                            {
                                AddItem(gd.X, gd.Y, gd.G, gd.U);
                                continue;
                            }

                            AddItem(gd.X, gd.Y, gd.G);
                            break;
                        case JsonGumpDataType.Label:
                            /*if (gd.U > 0)
                            {
                                AddLabel(gd.X, gd.Y, gd.U, gd.V);
                            }

                            AddLabel(gd.X, gd.Y, 256, gd.V);*/
                            AddLabel(gd.X, gd.Y, gd.U, gd.V);
                            break;
                        case JsonGumpDataType.PageBreak:
                            AddPage(Convert.ToInt32(gd.V));
                            break;
                        case JsonGumpDataType.PageButton:
                            AddButton(gd.X, gd.Y, gd.G, gd.P, Convert.ToInt32(gd.V), GumpButtonType.Page,
                                Convert.ToInt32(gd.V));
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
    }

    public class JsonGumpData
    {
        public int G { get; set; }
        public int P { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public int U { get; set; }
        public JsonGumpDataType T { get; set; }
        public string V { get; set; }
        public string C { get; set; }
    }

    public enum JsonGumpDataType
    {
        Background,
        Tiled,
        Button,
        Label,
        Image,
        Html,
        Item,
        PageBreak,
        PageButton
    }
}

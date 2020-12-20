using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Customs
{
    public class ItemsViewerCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("ItemViewer", AccessLevel.GameMaster, new CommandEventHandler(ItemsViewerCommand_OnCommand));
        }

        [Usage("ItemViewer")]
        [Description("Opens the item viewer.")]
        public static void ItemsViewerCommand_OnCommand(CommandEventArgs arg)
        {
            arg.Mobile.SendGump(new ItemsViewer(arg.Length > 0 ? (int)arg.GetDouble(0) : 0));
        }
    }

    public class ItemsViewer : Gump
    {
        private static int _page;
        public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
        public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;
        public static readonly int TextHue = PropsConfig.TextHue;
        public static readonly int TextOffsetX = PropsConfig.TextOffsetX;
        public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
        public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
        public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
        public static readonly int BackGumpID = PropsConfig.BackGumpID;
        public static readonly int SetGumpID = PropsConfig.SetGumpID;
        public static readonly int SetWidth = 600;
        public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
        public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
        public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;
        public static readonly int PrevWidth = PropsConfig.PrevWidth;
        public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
        public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
        public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;
        public static readonly int NextWidth = PropsConfig.NextWidth;
        public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
        public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
        public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;
        public static readonly int OffsetSize = PropsConfig.OffsetSize;
        public static readonly int EntryHeight = PropsConfig.EntryHeight;
        public static readonly int BorderSize = PropsConfig.BorderSize;
        public static bool OldStyle = PropsConfig.OldStyle;
        /*
        private static bool PrevLabel = OldStyle, NextLabel = OldStyle;

        private static readonly int PrevLabelOffsetX = PrevWidth + 1;
		
        private static readonly int PrevLabelOffsetY = 0;

        private static readonly int NextLabelOffsetX = -29;
        private static readonly int NextLabelOffsetY = 0;
        * */
        private static readonly int NameWidth = 107;
        private static readonly int ValueWidth = 128;
        private static readonly int EntryCount = 15;
        private static readonly int TypeWidth = NameWidth + OffsetSize + ValueWidth;
        private static readonly int TotalWidth = OffsetSize + NameWidth + OffsetSize + ValueWidth + OffsetSize + SetWidth + OffsetSize;
        private static readonly int TotalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (EntryCount + 1));
        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;
        private static readonly int IndentWidth = 12;

        public ItemsViewer(int itemPage)
            : base(GumpOffsetX, GumpOffsetY)
        {
            _page = itemPage;
            int totalHeight = 800;

            this.AddPage(0);

            this.AddBackground(0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID);
            this.AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);

            int x = BorderSize + OffsetSize;
            int y = BorderSize + OffsetSize;

            int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);
            
            this.AddButton(20,16,PrevButtonID1, PrevButtonID2,1,GumpButtonType.Reply,0);
            this.AddButton(820,16,NextButtonID1, NextButtonID2,2,GumpButtonType.Reply,0);

            var offset = _page * 56;
            for (var r = 0; r < 4; r++)
            {
                for (int i = 1; i < 15; i++)
                {
                    this.AddItem((i * 50) + 25, (r * 200) + 32, i + offset);
                    this.AddButton((i * 50) + 40, (r * 200) + 12, 0x15E2, 0x15E6, i + offset + 10, GumpButtonType.Reply, 0);
                    this.AddButton((i * 50) + 52, (r * 200) + 12, 0x15E2, 0x15E6, i + offset + 1000000, GumpButtonType.Reply, 0);
                }

                offset += 14;
            }

            this.AddLabel(20, 784, 256, $"Page {_page}   (use [itemviewer {{page}} to come directly to this page)");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID > 1000000)
            {
                var itemId = info.ButtonID - 1000000;
                sender.Mobile.SendMessage("What do you wish to convert into this object? <ESC> to cancel.");
                //BoundingBoxPicker.Begin(e.Mobile, new BoundingBoxCallback(AddonBox_Callback), null);
                sender.Mobile.Target = new SetTarget(_page, itemId);
                return;
            }
            if (info.ButtonID > 10)
            {
                var itemId = info.ButtonID - 10;
                sender.Mobile.SendMessage("Where do you wish to place this object? <ESC> to cancel.");
                sender.Mobile.Target = new InternalTarget(_page, itemId);
                return;
            }

            sender.Mobile.CloseGump(typeof(ItemsViewer));

            switch (info.ButtonID)
            {
                case 1:
                    sender.Mobile.SendGump(new ItemsViewer(_page < 1 ? 0 : _page - 1));
                    break;
                case 2:
                    sender.Mobile.SendGump(new ItemsViewer(_page + 1));
                    break;
            }
        }

        public class InternalTarget : Target
        {
            private readonly int m_Page;
            private readonly int m_ItemId;
            public InternalTarget(int page, int itemId)
                : base(-1, true, TargetFlags.None)
            {
                this.m_Page = page;
                m_ItemId = itemId;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                {
                    if (p is Item)
                        p = ((Item)p).GetWorldTop();
                    else if (p is Mobile)
                        p = ((Mobile)p).Location;

                    //Server.Commands.Add.Invoke(from, new Point3D(p), new Point3D(p), new string[] { this.m_Type.Name });
                    var newItem = new Static();
                    newItem.Movable = false;
                    newItem.ItemID = m_ItemId;
                    newItem.MoveToWorld(new Point3D(p), from.Map);

                    from.Target = new ItemsViewer.InternalTarget(m_Page, m_ItemId);
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (cancelType == TargetCancelType.Canceled)
                    from.SendGump(new ItemsViewer(m_Page));
            }
        }

        public class SetTarget : Target
        {
            private readonly int m_Page;
            private readonly int m_ItemId;
            public SetTarget(int page, int itemId)
                : base(-1, true, TargetFlags.None)
            {
                this.m_Page = page;
                m_ItemId = itemId;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                try
                {
                    ((Item) o).ItemID = m_ItemId;
                } catch(Exception) {}

                from.SendGump(new ItemsViewer(m_Page));
                //from.Target = new ItemsViewer.SetTarget(m_Page, m_ItemId);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (cancelType == TargetCancelType.Canceled)
                    from.SendGump(new ItemsViewer(m_Page));
            }
        }
    }
}

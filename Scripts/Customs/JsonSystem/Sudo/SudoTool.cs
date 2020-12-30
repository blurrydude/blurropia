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

    public class SudoTool : JsonGump
    {
        private string _menu;
        public SudoTool(string menu)
            : base($@"{AppDomain.CurrentDomain.BaseDirectory}Scripts\Customs\JsonSystem\Sudo\Data\{menu}.json")
        {
            Console.WriteLine($"sudo gump {menu} fired");
            _menu = menu;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0) return;
            Mobile from = sender.Mobile;
            var gd = GumpData.FirstOrDefault(x => x.V == info.ButtonID.ToString());
            if (gd == null) return;
            var commands = gd.C.Split(',');
            foreach (var command in commands)
            {
                CommandSystem.Handle(from, command);
            }

            from.SendGump(new SudoTool(_menu));
        }

        public class InternalTarget : Target
        {
            //private readonly int m_Page;
            private readonly int m_ItemId;
            private string _menu;
            public InternalTarget(string menu)
                : base(-1, true, TargetFlags.None)
            {
                _menu = menu;
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

                    from.Target = new SudoTool.InternalTarget(_menu);
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (cancelType == TargetCancelType.Canceled)
                    from.SendGump(new SudoTool(_menu));
            }
        }
    }
}

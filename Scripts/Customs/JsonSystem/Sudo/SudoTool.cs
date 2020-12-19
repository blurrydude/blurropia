using System;
using System.Collections.Generic;
using System.IO;
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
    public class SudoCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("sudo", AccessLevel.GameMaster, SudoToolCommand_OnCommand);
            Console.WriteLine("sudo command registered");
            var menus = Directory.GetFiles($@"{AppDomain.CurrentDomain.BaseDirectory}Scripts\Customs\JsonSystem\Sudo\Data\", "*.json");
            foreach (var menu in menus)
            {
                Console.WriteLine(menu);
            }
        }

        [Usage("sudo")]
        [Description("Opens the sudo tool.")]
        public static void SudoToolCommand_OnCommand(CommandEventArgs arg)
        {
            Console.WriteLine("sudo command fired");
            arg.Mobile.SendGump(new SudoTool(arg.Length > 0 ? arg.GetString(0) : "sudo"));
        }
    }

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
            private readonly int m_Page;
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
    public class SudoStone : Item
    {
        private string _menu;
        [CommandProperty(AccessLevel.GameMaster)]
        public string Menu
        {
            get => _menu;
            set
            {
                _menu = value;
                Name = $"Sudo Stone - {_menu}";
            }
        }
        [Constructable]
        public SudoStone() : base(3631)
        {
            Name = $"Sudo Stone - {_menu}";
            _menu = "sudo";
        }
        public SudoStone( Serial serial ) : base( serial )
        {

        }

        public override void OnDoubleClick(Mobile @from)
        {
            from.SendGump(new SudoTool(_menu));
        }

        public override void Serialize(GenericWriter writer)
        {
            writer.Write(_menu);
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            _menu = reader.ReadString();
            base.Deserialize(reader);
        }
    }
}

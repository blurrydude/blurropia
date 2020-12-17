using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Server.Customs
{
    public class ScheduledTeleporter : Item
    {
        private bool m_Active, m_Creatures, m_CombatCheck, m_CriminalCheck;
        private Point3D m_PointDest;
        private List<Point3D> m_PointDestinations;
        private Map m_MapDest;
        private bool m_SourceEffect;
        private bool m_DestEffect;
        private int m_SoundID;
        private int m_CurrentDestination;
        private TimeSpan m_Delay;
        private Timer m_Timer;

        [Constructable]
        public ScheduledTeleporter()
            : this(new List<Point3D>(), null, false)
        { }

        [Constructable]
        public ScheduledTeleporter(List<Point3D> pointDestinations, Map mapDest)
            : this(pointDestinations, mapDest, false)
        { }

        [Constructable]
        public ScheduledTeleporter(List<Point3D> pointDestinations, Map mapDest, bool creatures)
            : base(0x1BC3)
        {
            Movable = false;
            Visible = false;

            m_Active = true;
            m_PointDestinations = pointDestinations;
            m_PointDest = new Point3D();
            m_MapDest = mapDest;
            m_Creatures = creatures;
            m_CurrentDestination = 0;

            m_CombatCheck = false;
            m_CriminalCheck = false;
            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), new TimerCallback(OnTick));
            m_Delay = TimeSpan.FromSeconds(10);
        }

        public ScheduledTeleporter(Serial serial)
            : base(serial)
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SourceEffect
        {
            get { return m_SourceEffect; }
            set
            {
                m_SourceEffect = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DestEffect
        {
            get { return m_DestEffect; }
            set
            {
                m_DestEffect = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SoundID
        {
            get { return m_SoundID; }
            set
            {
                m_SoundID = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Delay
        {
            get { return m_Delay; }
            set
            {
                m_Delay = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                m_Active = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D PointDest
        {
            get { return m_PointDest; }
            set
            {
                m_PointDestinations.Add(value);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentDestination
        {
            get { return m_CurrentDestination; }
            set
            {
                m_CurrentDestination = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PointDestinations
        {
            get { return m_PointDestinations?.Count??0; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map MapDest
        {
            get { return m_MapDest; }
            set
            {
                m_MapDest = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Creatures
        {
            get { return m_Creatures; }
            set
            {
                m_Creatures = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CombatCheck
        {
            get { return m_CombatCheck; }
            set
            {
                m_CombatCheck = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CriminalCheck
        {
            get { return m_CriminalCheck; }
            set
            {
                m_CriminalCheck = value;
                InvalidateProperties();
            }
        }

        public override int LabelNumber { get { return 1026095; } } // teleporter
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Active)
            {
                list.Add(1060742); // active
            }
            else
            {
                list.Add(1060743); // inactive
            }

            if (m_MapDest != null)
            {
                list.Add(1060658, "Map\t{0}", m_MapDest);
            }

            if (m_PointDest != Point3D.Zero)
            {
                list.Add(1060659, "Coords\t{0}", m_PointDest);
            }

            list.Add(1060660, "Creatures\t{0}", m_Creatures ? "Yes" : "No");
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Active)
            {
                if (m_MapDest != null && m_PointDest != Point3D.Zero)
                {
                    LabelTo(from, "{0} [{1}]", m_PointDest, m_MapDest);
                }
                else if (m_MapDest != null)
                {
                    LabelTo(from, "[{0}]", m_MapDest);
                }
                else if (m_PointDest != Point3D.Zero)
                {
                    LabelTo(from, m_PointDest.ToString());
                }
            }
            else
            {
                LabelTo(from, "(inactive)");
            }
        }

        public virtual bool CanTeleport(Mobile m)
        {
            if (!m_Active)
            {
                return false;
            }

            if (!m_Creatures && !m.Player)
            {
                return false;
            }

            if (m.Holding != null)
            {
                m.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
                return false;
            }
            
            if (m_CriminalCheck && m.Criminal)
            {
                m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            
            if (m_CombatCheck && SpellHelper.CheckCombat(m))
            {
                m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }
            
            if (!CheckDestination(m) || (Siege.SiegeShard && m_MapDest == Map.Trammel))
            {
                return false;
            }

            return true;
        }

        private bool CheckDestination(Mobile m)
        {
            Map map = m_MapDest;

            if (map == null || map == Map.Internal)
            {
                map = Map;
            }

            Region myRegion = Region.Find(m.Location, m.Map);
            Region toRegion = Region.Find(m_PointDest, map);

            if (myRegion != toRegion)
            {
                return toRegion.OnMoveInto(m, m.Direction, m_PointDest, m.Location);
            }

            return true;
        }

        public virtual void StartTeleport(Mobile m)
        {
            if (!m.CanBeginAction(typeof(Teleporter)))
            {
                m.SendMessage("Teleport in progress...");
                return;
            }

            if (!m_Active || !CanTeleport(m))
            {
                return;
            }
            // Allow OnMoveOver to return before processing the map/location changes
            Timer.DelayCall(DoTeleport, m); 
        }
        
        public virtual void DoTeleport(Mobile m)
        {
            if (m_PointDestinations == null || !m_PointDestinations.Any())
            {
                m.SendMessage("Nope.");
                return;
            }

            Map map = m_MapDest;

            if (map == null || map == Map.Internal)
            {
                map = Map;
            }

            Point3D p = m_PointDestinations[m_CurrentDestination];

            if (p == Point3D.Zero)
            {
                p = m.Location;
            }

            bool sendEffect = (!m.Hidden || m.IsPlayer());

            if (m_SourceEffect && sendEffect)
            {
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
            }

            BaseCreature.TeleportPets(m, p, map);

            m.MoveToWorld(p, map);

            if (m_DestEffect && sendEffect)
            {
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
            }

            if (m_SoundID > 0 && sendEffect)
            {
                Effects.PlaySound(m.Location, m.Map, m_SoundID);
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            StartTeleport(m);
            
            return true;
        }

        private void OnTick()
        {
            if (m_PointDestinations == null || !m_PointDestinations.Any())
            {
                return;
            }

            var limit = m_PointDestinations.Count - 1;
            if (m_CurrentDestination >= limit)
            {
                m_CurrentDestination = 0;
                return;
            }

            m_CurrentDestination++;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_CriminalCheck);
            writer.Write(m_CombatCheck);

            writer.Write(m_SourceEffect);
            writer.Write(m_DestEffect);
            writer.Write(m_Delay);
            writer.WriteEncodedInt(m_SoundID);

            writer.Write(m_Creatures);

            writer.Write(m_Active);
            writer.Write(m_MapDest);

            writer.Write(m_CurrentDestination);
            writer.Write(m_PointDestinations.Count);
            foreach (var point in m_PointDestinations)
            {
                writer.Write(point);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_CriminalCheck = reader.ReadBool();
                        m_CombatCheck = reader.ReadBool();
                        m_SourceEffect = reader.ReadBool();
                        m_DestEffect = reader.ReadBool();
                        m_Delay = reader.ReadTimeSpan();
                        m_SoundID = reader.ReadEncodedInt();
                        m_Creatures = reader.ReadBool();
                        m_Active = reader.ReadBool();
                        m_MapDest = reader.ReadMap();
                        m_CurrentDestination = reader.ReadInt();
                        var pdCount = reader.ReadInt();
                        m_PointDestinations = new List<Point3D>();
                        for (int i = 0; i < pdCount; i++)
                        {
                            m_PointDestinations.Add(reader.ReadPoint3D());
                        }
                        break;
                    }
            }
        }
    }
}

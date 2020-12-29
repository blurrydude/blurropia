using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Server.Items;
using Server.Network;
using ServerUtilityExtensions;

namespace Server.Customs.JsonSystem.Theater
{
    public class TheaterStone : Item
    {
        public List<List<WayPoint>> Grid;
        public List<JsonActor> Actors;
        public List<Item> SetProps;
        public DateTime? PlayStart;
        public JsonTheaterScript Script;
        public bool Active;
        public int CurrentScene;
        private List<int> _waypoints;
        private List<int> _roster;
        private List<int> _setProps;
        private int _width;
        private int _length;
        private Timer _timer;
        private string _scriptName;

        [CommandProperty(AccessLevel.Counselor)]
        public int Width
        {
            get => _width;
            set { _width = value; BuildGrid(); }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public int Length
        {
            get => _length;
            set { _length = value; BuildGrid(); }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public string ScriptName
        {
            get => _scriptName;
            set { _scriptName = value; BuildGrid(); }
        }

        [Constructable]
        public TheaterStone(): base(0xEDC)
        {
            Visible = false;
            Movable = false;
            Active = false;
            CurrentScene = 0;
            _timer = new TheaterStoneTimer(this);
            _timer.Start();
        }

        [Constructable]
        public TheaterStone(Serial serial) : base(serial)
        {
            Active = false;
            CurrentScene = 0;
            _timer = new TheaterStoneTimer(this);
            _timer.Start();
        }

        public override bool HandlesOnSpeech { get=>true; }
        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!Active && e.Mobile.InRange(Location, 20) && e.Speech == "begin")
            {
                CurrentScene = 0;
                Active = false;
                LoadScript();
                LoadScene();
                PublicOverheadMessage(MessageType.Regular,0x35,false,"Play starting");
            }
        }

        public override void OnDoubleClick(Mobile @from)
        {
            base.OnDoubleClick(@from);
            if (Active)
            {
                Active = false;
                ClearActors();
                ClearSet();
                PublicOverheadMessage(MessageType.Regular,0x35,false,"Cleared set");
                PlayStart = null;
                return;
            }
            CurrentScene = 0;
            Active = false;
            LoadScript();
            LoadScene();
            PublicOverheadMessage(MessageType.Regular,0x35,false,"Play starting");
        }

        public void LoadScript()
        {
            if (String.IsNullOrEmpty(_scriptName)) return;
            var json = File.ReadAllText($"Scripts/Customs/JsonSystem/Theater/Data/{_scriptName}.json");
            Script = (JsonTheaterScript) JsonUtility.Deserialize<JsonTheaterScript>(json);
        }

        public void LoadScene()
        {
            if (Script == null) return;
            Active = false;
            if (CurrentScene >= Script.Scenes.Count)
            {
                CurrentScene = 0;
                Active = false;
                ClearSet();
                ClearActors();
                PlayStart = null;
                return;
            }
            if(PlayStart == null) PlayStart = DateTime.Now;
            
            SetStage();
            Places();
            Active = true;

        }

        public void SetStage()
        {
            ClearSet();

            _setProps = new List<int>();
            SetProps = new List<Item>();
            var scene = Script.Scenes[CurrentScene];
            foreach (var prop in scene.SetProps)
            {
                var item = new Static(prop.ItemId);
                item.Hue = prop.Hue ?? 0;
                var loc = Grid[prop.Position.X][prop.Position.Y].Location;
                item.MoveToWorld(new Point3D(loc.X,loc.Y,prop.Position.Z),Map);
                _setProps.Add(item.Serial);
            }
        }

        public void ClearSet()
        {
            if (_setProps != null)
            {
                foreach (var serial in _setProps)
                {
                    var item = World.FindItem(serial);
                    if (item == null) continue;
                    item.Delete();
                }
            }
            BuildGrid();
        }

        public void Places()
        {
            ClearActors();

            _roster = new List<int>();
            Actors = new List<JsonActor>();
            var scene = Script.Scenes[CurrentScene];
            foreach (var config in scene.Cast)
            {
                var actor = new JsonActor(config);
                actor.CantWalk = true;
                var loc = Grid[config.StartingPlace.X][config.StartingPlace.Y].Location;
                actor.MoveToWorld(new Point3D(loc.X,loc.Y,loc.Z), Map);
                Actors.Add(actor);
                _roster.Add(actor.Serial);
            }
        }

        public void ClearActors()
        {
            if (_roster != null)
            {
                foreach (var serial in _roster)
                {
                    var mobile = World.FindMobile(serial);
                    if(mobile != null) mobile.Delete();
                }
            }
        }

        public void BuildGrid()
        {
            if (_waypoints != null)
            {
                foreach (var serial in _waypoints)
                {
                    var waypoint = World.FindItem(serial);
                    if (waypoint == null) continue;
                    waypoint.Delete();
                }
            }
            _waypoints = new List<int>();
            Grid = new List<List<WayPoint>>();
            for (var x = 0; x < _width; x++)
            {
                Grid.Add(new List<WayPoint>());
                for (var y = 0; y < _length; y++)
                {
                    var waypoint = new WayPoint();
                    waypoint.MoveToWorld(new Point3D(
                        Location.X - (_width/2) + x,
                        Location.Y + _length - y,
                        Location.Z
                    ),Map);
                    Grid[x].Add(waypoint);
                    _waypoints.Add(waypoint.Serial);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
            writer.Write(_width);
            writer.Write(_length);
            writer.Write(_scriptName);
            if (_waypoints == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(_waypoints.Count);
                foreach (var serial in _waypoints)
                {
                    writer.Write(serial);
                }
            }
            
            if (_roster == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(_roster.Count);
                foreach (var serial in _roster)
                {
                    writer.Write(serial);
                }
            }
            
            if (_setProps == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(_setProps.Count);
                foreach (var serial in _setProps)
                {
                    writer.Write(serial);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            _width = version < 1 ? 19 : reader.ReadInt();
            _length = version < 1 ? 4 : reader.ReadInt();
            _scriptName = reader.ReadString();
            int waypointCount = reader.ReadInt();
            _waypoints = new List<int>();
            for (var i = 0; i < waypointCount; i++)
            {
                _waypoints.Add(reader.ReadInt());
            }

            Grid = new List<List<WayPoint>>();
            var x = 0;
            var y = 0;
            foreach (var serial in _waypoints)
            {
                if(Grid.Count == x) Grid.Add(new List<WayPoint>());
                var waypoint = World.FindItem(serial);
                if (waypoint == null) continue;
                Grid[x].Add((WayPoint)waypoint);
                if (x == _width)
                {
                    x = 0;
                    y++;
                }
            }
            int rosterCount = reader.ReadInt();
            _roster = new List<int>();
            for (var i = 0; i < rosterCount; i++)
            {
                _roster.Add(reader.ReadInt());
            }

            Actors = new List<JsonActor>();
            foreach (var serial in _roster)
            {
                var mobile = World.FindMobile(serial);
                if (mobile == null) continue;
                Actors.Add((JsonActor)mobile);
            }

            var propCount = reader.ReadInt();
            _setProps = new List<int>();
            for (var i = 0; i < propCount; i++)
            {
                _setProps.Add(reader.ReadInt());
            }

            SetProps = new List<Item>();
            foreach (var serial in _setProps)
            {
                var item = World.FindItem(serial);
                if (item == null) continue;
                SetProps.Add(item);
            }
        }
    }

    public class JsonTheaterProp
    {
        public int ItemId { get; set; }
        public Point3D Position { get; set; }
        public int? Hue { get; set; }
    }
}

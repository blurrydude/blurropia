using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Customs.JsonSystem
{
    public class ArenaBattleStone : Item
    {
        private int _Version;
        private int _ArenaXRange;
        private int _ArenaYRange;
        private bool _InWar;
        private bool _Automatic;
        private string _LastScore = "";
        private int _AutoStep;
        private int _OffsetX;
        private int _OffsetY;
        private bool _BettingOpen;
        private Timer _AutoTimer;
        public List<ArenaFighter> Grid { get; set; }
        public List<ArenaBattleWager> Wagers { get; set; }

        [CommandProperty( AccessLevel.Counselor )]
        public int ArenaXRange
        {
            get { return _ArenaXRange; }
            set { _ArenaXRange = value; }
        }
        
        [CommandProperty( AccessLevel.Counselor )]
        public int ArenaYRange
        {
            get { return _ArenaYRange; }
            set { _ArenaYRange = value; }
        }
        
        [CommandProperty( AccessLevel.Counselor )]
        public int OffsetY
        {
            get { return _OffsetY; }
            set { _OffsetY = value; }
        }
        
        [CommandProperty( AccessLevel.Counselor )]
        public int OffsetX
        {
            get { return _OffsetX; }
            set { _OffsetX = value; }
        }

        public bool Automatic
        {
            get { return _Automatic; }
            set { _Automatic = value; }
        }

        public bool BettingOpen
        {
            get { return _BettingOpen; }
            set { _BettingOpen = value; }
        }

        [Constructable] 
        public ArenaBattleStone() : base( 0xEDC ) 
        { 
            Movable = false; 
            Hue = 89; 
            Name = "Arena Battle Stone";
            Grid = new List<ArenaFighter>();
            Wagers = new List<ArenaBattleWager>();
            _ArenaXRange = 5;
            _ArenaYRange = 3;
            _InWar = false;
            _Automatic = false;
            _AutoStep = 0;
            _OffsetX = -5;
            _OffsetY = 2;
            _BettingOpen = false;

            _Version = 3;
            
            _AutoTimer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), AutoTimer);
        }

        public override void OnDelete()
        {
            RemoveAll();
            _AutoTimer.Stop();
            base.OnDelete();
        }

        public override void OnDoubleClick( Mobile from )
        {
            if (from.HasGump(typeof(ArenaBattleBettingGump))) from.CloseGump(typeof(ArenaBattleBettingGump));
            if (from.HasGump(typeof(ArenaBattleGump))) from.CloseGump(typeof(ArenaBattleGump));
            if (_BettingOpen)
            {
                if (Wagers.Any(x => x.Serial == from.Serial))
                {
                    from.SendMessage(136,"You have already placed a wager on this fight.");
                    return;
                }
                from.SendGump(new ArenaBattleBettingGump(from, this));
                return;
            }
            if (from.AccessLevel >= AccessLevel.Counselor)
            {
                from.SendGump(new ArenaBattleGump(from, this));
                return;
            }
            from.SendMessage(136,"Betting is closed.");
        }

        public ArenaBattleStone( Serial serial ) : base( serial ) 
        {
        }

        public void PlaceWager(int team, int amount, Mobile mobile)
        {
            if (Wagers.Any(x => x.Serial == mobile.Serial))
            {
                mobile.SendMessage(136,"You have already placed a wager on this fight.");
                return;
            }

            var gold = FindGold(mobile.Backpack, true);
            if (gold.Sum(x => x.Amount) < amount)
            {
                mobile.SendMessage(136,"You cannot afford this wager.");
                return;
            }

            if (!ConsumeGold(mobile.Backpack, amount, true))
            {
                mobile.SendMessage(136,"Seriously, you cannot afford this wager.");
                return;
            }
            var wager = new ArenaBattleWager
            {
                Team = team,
                Amount = amount,
                Serial = mobile.Serial
            };
            Wagers.Add(wager);
            mobile.SendMessage(166,$"You have placed a {amount} gold wager on the {(team==1?"green":"blue")} team to win.");
        }
        
        public void Auto()
        {
            _AutoStep = 0;
            _InWar = false;
            _Automatic = true;
            RemoveAll();
        }

        public void Manual()
        {
            _AutoStep = 0;
            _InWar = false;
            _Automatic = false;
            RemoveAll();
        }

        public void AutoGenTeams()
        {
            var w = _ArenaXRange * 2 + 1;
            var h = _ArenaYRange * 2 + 1;
            var qX = (int)Math.Floor((double)_ArenaXRange / 2);
            var qY = _ArenaYRange;
            var team1Positions = new []
            {
                new Point2D(qX,qY),
                new Point2D(qX,qY-2),
                new Point2D(qX,qY+2),
                
                new Point2D(qX+2,qY-1),
                new Point2D(qX+2,qY+1),
                new Point2D(qX+2,qY-3),
                new Point2D(qX+2,qY+3),
                
                new Point2D(qX-2,qY-1),
                new Point2D(qX-2,qY+1),
                new Point2D(qX-2,qY-3),
                new Point2D(qX-2,qY+3)
            };
            var team2Positions = new []
            {
                new Point2D(qX,qY) + new Point2D(_ArenaXRange+1,0),
                new Point2D(qX,qY-2) + new Point2D(_ArenaXRange+1,0),
                new Point2D(qX,qY+2) + new Point2D(_ArenaXRange+1,0),
                
                new Point2D(qX-2,qY-1) + new Point2D(_ArenaXRange+1,0),
                new Point2D(qX-2,qY+1) + new Point2D(_ArenaXRange+1,0),
                new Point2D(qX-2,qY-3) + new Point2D(_ArenaXRange+1,0),
                new Point2D(qX-2,qY+3) + new Point2D(_ArenaXRange+1,0),
                
                new Point2D(qX+2,qY-1) + new Point2D(_ArenaXRange+1,0),
                new Point2D(qX+2,qY+1) + new Point2D(_ArenaXRange+1,0),
                new Point2D(qX+2,qY-3) + new Point2D(_ArenaXRange+1,0),
                new Point2D(qX+2,qY+3) + new Point2D(_ArenaXRange+1,0)
            };
            var teamSize = Utility.Random(1, Math.Max(_ArenaXRange, 11));
            for (var i = 0; i < teamSize; i++)
            {
                var typeRoll = Utility.Random(1, 100);
                var typeRoll2 = Utility.Random(1, 100);
                ArenaFighterType type1 = 
                    i == 0 && teamSize > 3 ? ArenaFighterType.Healer :
                    typeRoll < 50 ? ArenaFighterType.Swordsman :
                    typeRoll < 65 ? ArenaFighterType.Fencer :
                    typeRoll < 85 ? ArenaFighterType.Macer :
                    //typeRoll < 95 ? ArenaFighterType.Healer :
                    typeRoll < 95 ? ArenaFighterType.Assassin :
                    ArenaFighterType.Supermage;
                ArenaFighterType type2 = 
                    i == 0 && teamSize > 3 ? ArenaFighterType.Healer :
                    typeRoll2 < 50 ? ArenaFighterType.Swordsman :
                    typeRoll2 < 65 ? ArenaFighterType.Fencer :
                    typeRoll2 < 85 ? ArenaFighterType.Macer :
                    typeRoll2 < 95 ? ArenaFighterType.Assassin :
                    ArenaFighterType.Supermage;
                var t1index = MathHelper.GetIndex(team1Positions[i], w);
                var t2index = MathHelper.GetIndex(team2Positions[i], w);
                Spawn(t1index, type1);
                Spawn(t2index, type2);
            }
        }

        public void AutoTimer()
        {
            if (_Automatic)
            {
                switch (_AutoStep)
                {
                    case 3:
                        RemoveAll();
                        break;
                    case 5:
                        PublicOverheadMessage(MessageType.Regular, 166, false,
                            "Place your wagers, battle will commence in two minutes!");
                        _BettingOpen = true;
                        AutoGenTeams();
                        break;
                    case 10:
                    case 20:
                    case 30:
                    case 40:
                    case 50:
                    case 60:
                    case 70:
                    case 80:
                    case 90:
                        PublicOverheadMessage(MessageType.Regular, 166, false,
                            "Double click stone to place your wagers!");
                        break;
                    case 95:
                        PublicOverheadMessage(MessageType.Regular, 166, false,
                            "Thirty seconds until the fight begins! Betting closes in twenty!");
                        break;
                    case 115:
                        PublicOverheadMessage(MessageType.Regular, 166, false,
                            "Ten seconds until the fight begins! All betting closed.");
                        _BettingOpen = false;
                        break;
                    case 125:
                        War(); _AutoStep++;
                        break;
                    case 130:
                        PublicOverheadMessage(MessageType.Regular, 166, false,
                            "We will start another round in two minutes.");
                        break;
                    case 160:
                        PublicOverheadMessage(MessageType.Regular, 166, false,
                            "We will start another round in 90 seconds.");
                        break;
                    case 190:
                        PublicOverheadMessage(MessageType.Regular, 166, false,
                            "We will start another round in one minute.");
                        break;
                    case 210:
                        PublicOverheadMessage(MessageType.Regular, 166, false,
                            "We will start another round in 30 seconds.");
                        break;
                    case 240:
                        _AutoStep = 0;
                        break;

                }

                if (!_InWar)
                {
                    _AutoStep++;
                }
            }

            if (_InWar)
            {
                WarTimer();
            }
        }

        public void WarTimer()
        {
            var serials = Grid.Select(x => x.Serial).ToList();
            foreach (var afs in serials)
            {
                if (!World.Mobiles.ContainsKey(afs))
                {
                    Grid.Remove(Grid.FirstOrDefault(x => x.Serial == afs));
                }
            }
            ClearCorpses();

            var team1 = Grid.Count(x => x.Team == 1);
            var team2 = Grid.Count(x => x.Team == 2);
            var score = $"Green: {team1} | Blue: {team2}";
            if (_LastScore != score)
            {
                _LastScore = score;
                PublicOverheadMessage(MessageType.Regular,166,false,score);
            }

            if (team1 == 0 || team2 == 0)
            {
                Peace();
                PublicOverheadMessage(MessageType.Regular,166,false,$"{(team1==0?"Blue":"Green")} wins!!!");
                Payout(team1!=0);
            }
        }

        public void Payout(bool team1wins)
        {
            var pot = Wagers.Sum(x => x.Amount) * 1.5;
            var winners = Wagers.Where(x => x.Team == (team1wins?1:2));
            var winnersIn = winners.Sum(x => x.Amount);
            foreach (var wager in Wagers)
            {
                var value = (int)((double)wager.Amount / winnersIn * pot);
                if (World.Mobiles.All(x => x.Key != wager.Serial)) continue;
                var mobile = World.Mobiles.First(x => x.Key == wager.Serial);
                if (wager.Team == 1 && team1wins || wager.Team == 2 && !team1wins)
                {
                    var check = new BankCheck(value);
                    mobile.Value.AddToBackpack(check);
                    mobile.Value.SendMessage(166,$"You won {value} gold on the {(team1wins?"green":"blue")} team");
                }
                else
                {
                    mobile.Value.SendMessage(166,$"You lost {wager.Amount} gold on the {(wager.Team==1?"green":"blue")} team");
                }
            }

            Wagers.Clear();
        }

        public void KillAll()
        {
            PublicOverheadMessage(MessageType.Regular,166,false,"Exterminate!");
            foreach (var serial in Grid.Select(x => x.Serial))
            {

                if (World.Mobiles.ContainsKey(serial))
                {
                    var fighter = World.Mobiles[serial];
                    fighter.Blessed = false;
                    fighter.CantWalk = false;
                    fighter.Kill();
                }
            }

            Grid.Clear();
        }

        public void RemoveAll()
        {
            PublicOverheadMessage(MessageType.Regular,166,false,"Clearing the field.");
            foreach (var serial in Grid.Select(x => x.Serial))
            {

                if (World.Mobiles.ContainsKey(serial))
                {
                    var fighter = World.Mobiles[serial];
                    fighter.Blessed = false;
                    fighter.CantWalk = false;
                    fighter.Delete();
                }
            }

            Grid.Clear();
            ClearCorpses();
            DeleteBlockers();
        }

        public void ClearCorpses()
        {
            var items = this.GetItemsInRange(_ArenaXRange + _ArenaYRange + 10);
            var corpses = items?.Where(x => x.GetType() == typeof(Corpse)).ToList()??new List<Item>();
            foreach (var corpse in corpses)
            {
                foreach (var item in corpse.Items.ToList())
                {
                    item.Delete();
                }
                corpse.Delete();
            }
        }

        public void War()
        {
            RebuildFence();
            PublicOverheadMessage(MessageType.Regular,166,false,"Fight!");
            foreach (var serial in Grid.Select(x => x.Serial))
            {
                if (World.Mobiles.ContainsKey(serial))
                {
                    var fighter = World.Mobiles[serial];
                    fighter.Blessed = false;
                    fighter.CantWalk = false;
                }
                else
                {
                    Grid.Remove(Grid.FirstOrDefault(x => x.Serial == serial));
                }
            }

            _InWar = true;
        }

        public void DeleteBlockers()
        {
            var items = this.GetItemsInRange(_ArenaXRange + _ArenaYRange + 10);
            var w = _ArenaXRange * 2 + 1;
            var h = _ArenaYRange * 2 + 1;
            var fieldStartX = _OffsetX - 1 + Location.X;
            var fieldStartY = _OffsetY - 1 + Location.Y;
            var fieldEndX = w + _OffsetX + 1 + Location.X;
            var fieldEndY = h + _OffsetY + 1 + Location.Y;
            var blockers = items?.Where(x => x.GetType() == typeof(Blocker) &&
                                             x.Location.X >= fieldStartX && x.Location.X <= fieldEndX &&
                                             x.Location.Y >= fieldStartY && x.Location.Y <= fieldEndY).ToList()??new List<Item>();
            foreach (var blocker in blockers)
            {
                blocker.Delete();
            }
        }

        public void RebuildFence()
        {
            DeleteBlockers();
            
            var w = _ArenaXRange * 2 + 1;
            var h = _ArenaYRange * 2 + 1;
            var start = new Point2D(_OffsetX - 1, _OffsetY - 1);
            for (var x = _OffsetX - 1; x <= w + _OffsetX; x++)
            {
                var pointA = Location + new Point3D(x,_OffsetY-1,0);
                var pointB = Location + new Point3D(x,h + _OffsetY,0);
                var blockerA = new Items.Blocker();
                blockerA.Map = Map;
                blockerA.MoveToWorld(pointA);
                var blockerB = new Items.Blocker();
                blockerB.Map = Map;
                blockerB.MoveToWorld(pointB);
            }
            for (var y = _OffsetY; y <= h + _OffsetY; y++)
            {
                var pointA = Location + new Point3D(_OffsetX - 1,y,0);
                var pointB = Location + new Point3D(w + _OffsetX,y,0);
                var blockerA = new Items.Blocker();
                blockerA.Map = Map;
                blockerA.MoveToWorld(pointA);
                var blockerB = new Items.Blocker();
                blockerB.Map = Map;
                blockerB.MoveToWorld(pointB);
            }
        }

        public void Peace()
        {
            PublicOverheadMessage(MessageType.Regular,166,false,"Peace has been called.");
            foreach (var serial in Grid.Select(x => x.Serial))
            {
                if (World.Mobiles.ContainsKey(serial))
                {
                    var fighter = World.Mobiles[serial];
                    fighter.Blessed = true;
                    fighter.CantWalk = true;
                }
                else
                {
                    Grid.Remove(Grid.FirstOrDefault(x => x.Serial == serial));
                }
            }
            _LastScore = "";
            _InWar = false;
        }
        
        public void Kill(int index)
        {
            var gfighter = Grid.First(x => x.Index == index);
            if (World.Mobiles.ContainsKey(gfighter.Serial))
            {
                var fighter = World.Mobiles[gfighter.Serial];
                fighter.Blessed = false;
                fighter.CantWalk = false;
                fighter.Kill();
            }

            Grid.Remove(gfighter);
        }
        
        public void Remove(int index)
        {
            var gfighter = Grid.First(x => x.Index == index);
            if (World.Mobiles.ContainsKey(gfighter.Serial))
            {
                var fighter = World.Mobiles[gfighter.Serial];
                fighter.Blessed = false;
                fighter.CantWalk = false;
                fighter.Delete();
            }

            Grid.Remove(gfighter);
        }
        
        public void Swap(int index)
        {
            var gfighter = Grid.First(x => x.Index == index);
            var fighter = World.Mobiles[gfighter.Serial];
            Dummy newfighter = null;
            var fightertype = ArenaFighterType.Healer;
            if (fighter.GetType() == typeof(DummyHealer)) { newfighter = new DummyFence(); fightertype = ArenaFighterType.Fencer; }
            else if (fighter.GetType() == typeof(DummyFence)) { newfighter = new DummySword(); fightertype = ArenaFighterType.Swordsman; }
            else if (fighter.GetType() == typeof(DummySword)) { newfighter = new DummyMace(); fightertype = ArenaFighterType.Macer; }
            else if (fighter.GetType() == typeof(DummyMace)) { newfighter = new DummySuper(); fightertype = ArenaFighterType.Supermage; }
            else if (fighter.GetType() == typeof(DummySuper)) { newfighter = new DummyAssassin(); fightertype = ArenaFighterType.Assassin; }
            fighter.Blessed = false;
            fighter.CantWalk = false;
            fighter.Delete();
            Grid.Remove(gfighter);
            if (newfighter != null)
            {
                newfighter.Blessed = true;
                newfighter.CantWalk = true;
                var rp = MathHelper.GetCartesian3D(index, _ArenaXRange * 2 + 1);
                var team = rp.X > _ArenaXRange || (rp.X > _ArenaXRange - 1 && rp.Y > _ArenaYRange) ? 2 : 1;
                newfighter.Team = team;
                var point = rp + new Point3D(_OffsetX,_OffsetY,0) + Location;
                newfighter.MoveToWorld(point, Map);
                Grid.Add(new ArenaFighter
                {
                    Index = index,
                    FighterType = fightertype,
                    Serial = newfighter.Serial,
                    Team = team
                });
            }
        }

        public void Spawn(int index, ArenaFighterType type = ArenaFighterType.Healer)
        {
            var fightertype = ArenaFighterType.Healer;
            Dummy dummy = null;
            switch (type)
            {
                case ArenaFighterType.Assassin: dummy = new DummyAssassin(); fightertype = ArenaFighterType.Healer; break;
                case ArenaFighterType.Fencer: dummy = new DummyFence(); fightertype = ArenaFighterType.Fencer; break;
                case ArenaFighterType.Macer: dummy = new DummyMace(); fightertype = ArenaFighterType.Macer; break;
                case ArenaFighterType.Supermage: dummy = new DummySuper(); fightertype = ArenaFighterType.Supermage; break;
                case ArenaFighterType.Swordsman: dummy = new DummySword(); fightertype = ArenaFighterType.Swordsman; break;
                default: case ArenaFighterType.Healer: dummy = new DummyHealer(); fightertype = ArenaFighterType.Healer; break;
            }
            dummy.Blessed = true;
            dummy.CantWalk = true;
            var rp = MathHelper.GetCartesian3D(index, _ArenaXRange * 2 + 1);
            var team = rp.X > _ArenaXRange || (rp.X > _ArenaXRange - 1 && rp.Y > _ArenaYRange) ? 2 : 1;
            dummy.Team = team;
            var point = rp + new Point3D(_OffsetX,_OffsetY,0) + Location;
            dummy.MoveToWorld(point, Map);
            Grid.Add(new ArenaFighter
            {
                Index = index,
                FighterType = fightertype,
                Serial = dummy.Serial,
                Team = team
            });
        }

        public override bool HandlesOnSpeech => true;

        public override void OnSpeech(SpeechEventArgs e)
        {
            if(e.Mobile.AccessLevel < AccessLevel.Counselor) return;
            switch (e.Speech)
            {
                case "kill": KillAll();
                    break;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)_Version); // version
            writer.Write(_ArenaXRange);
            writer.Write(_ArenaYRange);
            writer.Write(_Automatic);
            writer.Write(_InWar);
            writer.Write(_AutoStep);
            writer.Write(_OffsetX);
            writer.Write(_OffsetY);

            writer.Write(Grid.Count);
            foreach (var arenaFighter in Grid)
            {
                writer.Write(arenaFighter.Index);
                writer.Write((int)arenaFighter.FighterType);
                writer.Write(arenaFighter.Serial);
                writer.Write(arenaFighter.Team);
            }
            writer.Write(Wagers.Count);
            foreach (var wager in Wagers)
            {
                writer.Write(wager.Team);
                writer.Write(wager.Amount);
                writer.Write(wager.Serial);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            Grid = new List<ArenaFighter>();
            Wagers = new List<ArenaBattleWager>();
            base.Deserialize(reader);
            _Version = reader.ReadInt();
            _ArenaXRange = reader.ReadInt();
            _ArenaYRange = reader.ReadInt();
            _Automatic = reader.ReadBool();
            if(_Version < 3) reader.ReadBool();
            _InWar = reader.ReadBool();
            _AutoStep = reader.ReadInt();
            if (_Version > 0) _OffsetX = reader.ReadInt();
            if (_Version > 0) _OffsetY = reader.ReadInt();
            var count = reader.ReadInt();
            for (var i = 0; i < count; i++)
            {
                var id = reader.ReadInt();
                var fType = reader.ReadInt();
                var serial = reader.ReadInt();
                var team = reader.ReadInt();
                var fighter = new ArenaFighter
                {
                    Index = id,
                    FighterType = (ArenaFighterType)fType,
                    Serial = serial,
                    Team = team
                };
                Grid.Add(fighter);
            }

            if (_Version > 1)
            {
                var wagerCount = reader.ReadInt();
                for (var i = 0; i < wagerCount; i++)
                {
                    var team = reader.ReadInt();
                    var amount = reader.ReadInt();
                    var serial = reader.ReadInt();
                    var wager = new ArenaBattleWager
                    {
                        Team = team,
                        Amount = amount,
                        Serial = serial
                    };
                    Wagers.Add(wager);
                }
            }

            _BettingOpen = false;
            _AutoTimer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1), AutoTimer);
        }

        public static bool ConsumeGold(Container cont, double amount, bool recurse)
        {
            var gold = new Queue<Gold>(FindGold(cont, recurse));
            var total = gold.Aggregate(0.0, (c, g) => c + g.Amount);

            if (total < amount)
            {
                gold.Clear();

                return false;
            }

            var consume = amount;

            while (consume > 0)
            {
                var g = gold.Dequeue();

                if (g.Amount > consume)
                {
                    g.Consume((int)consume);

                    consume = 0;
                }
                else
                {
                    consume -= g.Amount;

                    g.Delete();
                }
            }

            gold.Clear();

            return true;
        }

        private static IEnumerable<Gold> FindGold(Container cont, bool recurse)
        {
            if (cont == null || cont.Items.Count == 0)
            {
                yield break;
            }

            if (cont is ILockable && ((ILockable)cont).Locked)
            {
                yield break;
            }

            if (cont is TrapableContainer && ((TrapableContainer)cont).TrapType != TrapType.None)
            {
                yield break;
            }

            var count = cont.Items.Count;

            while(--count >= 0)
            {
                if (count >= cont.Items.Count)
                {
                    continue;
                }

                var item = cont.Items[count];

                if (item is Container)
                {
                    if (!recurse)
                    {
                        continue;
                    }

                    foreach (var gold in FindGold((Container)item, true))
                    {
                        yield return gold;
                    }
                }
                else if (item is Gold)
                {
                    yield return (Gold)item;
                }
            }
        }
    }
}

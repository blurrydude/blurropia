using System;
using System.Linq;

namespace Server.Customs.JsonSystem.Theater
{
    public class TheaterStoneTimer : Timer
    {
        private TheaterStone _stone;
        public TheaterStoneTimer(TheaterStone stone) : base(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100))
        {
            Priority = TimerPriority.FiftyMS;
            _stone = stone;
        }

        protected override void OnTick()
        {
            if (!_stone.Active || _stone.PlayStart == null) return;
            var target = DateTime.Now - _stone.PlayStart;
            var scene = _stone.Script.Scenes[_stone.CurrentScene];
            var actions = scene.Actions.Where(x => x.TriggerTime <= target);
            /*if (_stone.Actors != null)
            {
                foreach (var actor in _stone.Actors)
                {
                    if (actor.CurrentWayPoint == null || actor.Location == actor.CurrentWayPoint.Location)
                    {
                        actor.CantWalk = true;
                        actor.CurrentWayPoint = null;
                        actor.Animate(0,8,0,true,false,0);
                    }
                }
            }*/
            foreach (var action in actions)
            {
                DoAction(action);
            }

            scene.Actions.RemoveAll(x => x.TriggerTime <= target);
            if (scene.Actions.Count == 0)
            {
                _stone.CurrentScene++;
                _stone.LoadScene();
            }

            foreach (var a in _stone.Actors)
            {
                if (a.CurrentWayPoint == null ||
                    a.Location == a.CurrentWayPoint.Location)
                {
                    a.Frozen = true;
                    a.CantWalk = true;
                }
            }
        }

        private void DoAction(JsonTheaterAction action)
        {
            var actor = _stone.Actors[action.Actor];
            switch (action.Action)
            {
                case "walk":
                    var target = action.Data;
                    var waypoint = _stone.Grid[target[0]][target[1]];
                    actor.CurrentWayPoint = waypoint;
                    actor.CantWalk = false;
                    actor.Frozen = false;
                    break;
                case "say":
                    var line = action.Text;
                    actor.Say(false,line);
                    break;
                case "act":
                    var anim = action.Data;
                    actor.Animate(anim[0],anim[1],anim[2],anim[3]>0,anim[4]>0,anim[5]);
                    break;
                case "effect":
                    var effect = action.Data;
                    actor.FixedEffect(effect[0],effect[1],effect[2],effect[3],effect[4]);
                    break;
                case "body":
                    actor.BodyValue = action.Data[0];
                    break;
                case "die":
                    actor.Kill();
                    break;
                case "face":
                    var success = Enum.TryParse<Direction>(action.Text, out var d);
                    if(success) actor.Direction = d;
                    actor.Frozen = true;
                    break;
                case "remove":
                    var toRemove = actor.Items.Where(x => x.GetType().ToString().Contains(action.Text)).ToList();
                    foreach (var item in toRemove)
                    {
                        item.Delete();
                    }
                    break;
                case "add":
                    actor.AddItem((Item)JsonSystemHelper.NewItemByTypeString(action.Text));
                    break;
                case "rename":
                    actor.Name = action.Text;
                    break;
                case "retitle":
                    actor.Title = action.Text;
                    break;
            }
        }
    }
}

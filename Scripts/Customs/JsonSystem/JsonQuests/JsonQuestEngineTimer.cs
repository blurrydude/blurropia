using System;

namespace Server.Customs
{
    public class JsonQuestEngineTimer : Timer
    {
        public JsonQuestEngineTimer()
            : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(15.0))
        {
            Priority = TimerPriority.FiveSeconds;
        }

        public static void Initialize()
        {
            if (!JsonQuestEngine.Enabled) return;
            new JsonQuestEngineTimer().Start();
            Console.WriteLine("Json Quest Engine Timer Started");
        }

        protected override void OnTick()
        {
            if (!JsonQuestEngine.Ready)
            {
                JsonQuestEngine.LastUpdate = DateTime.Now;
                JsonQuestEngine.LoadEngineData();
                JsonQuestEngine.LoadQuests();
                JsonQuestEngine.Ready = true;
                return;
            }

            var now = DateTime.Now;

            if (JsonQuestEngine.LastUpdate < now.AddMinutes(-1))
            {
                JsonQuestEngine.LastUpdate = DateTime.Now;
                JsonQuestEngine.LoadEngineData();
                if (JsonQuestEngine.Config.UpdatePending)
                {
                    JsonQuestEngine.LoadQuests();
                }
            }
        }
    }
}

using System;
using Server.Services;

namespace Server.Customs
{
    public class FirestoreChatEngine : Timer
    {
        public FirestoreChatEngine() : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(15.0))
        {
            Priority = TimerPriority.FiveSeconds;
        }

        public static void Initialize()
        {
            new FirestoreChatEngine().Start();
            Console.WriteLine("Firestore Chat Engine Started");
        }

        protected override void OnTick()
        {
            //FirestoreChatHandler.HandleIncoming();
        }
    }
}

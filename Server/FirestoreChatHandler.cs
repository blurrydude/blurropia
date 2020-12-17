using Server.Firestore;

namespace Server.Services
{
    public static class FirestoreChatHandler
    {
        public static void MessageSent(Mobile from, string channel, string message)
        {
            ChatHandler.MessageSent(from,channel,message);
        }

        public static void HandleIncoming()
        {
            ChatHandler.HandleIncoming();
        }
    }
}

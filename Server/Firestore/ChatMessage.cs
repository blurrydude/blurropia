using Google.Cloud.Firestore;

namespace Server.Firestore
{
    [FirestoreData]
    public class ChatMessage
    {
        [FirestoreProperty]
        public int TimeStamp { get; set; }
        
        [FirestoreProperty]
        public string Name { get; set; }
        
        [FirestoreProperty]
        public string AccountName { get; set; }
        
        [FirestoreProperty]
        public string Message { get; set; }
        
        [FirestoreProperty]
        public bool FromSite { get; set; }
        
        [FirestoreProperty]
        public bool Consumed { get; set; }
    }
}
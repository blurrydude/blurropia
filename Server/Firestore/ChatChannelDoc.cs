using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace Server.Firestore
{
    [FirestoreData]
    public class ChatChannelDoc
    {
        [FirestoreProperty]
        public string Channel { get; set; }
        
        [FirestoreProperty]
        public int LastUpdate { get; set; }
        
        [FirestoreProperty]
        public List<ChatMessage> Messages { get; set; }

        public ChatChannelDoc() {}

        public ChatChannelDoc(Dictionary<string, object> obj)
        {
            Channel = (string)obj["Channel"]??"";
            //LastUpdate = (int)(double)obj["LastUpdate"];
            Messages = new List<ChatMessage>();
            foreach (var mobj in (List<object>)obj["Messages"])
            {
                var message = (Dictionary<string,object>) mobj;
                Messages.Add(new ChatMessage
                {
                    AccountName = (string)message["AccountName"],
                    Consumed = (bool)message["Consumed"],
                    FromSite = (bool)message["FromSite"],
                    Message = (string)message["Message"],
                    Name = (string)message["Name"],
                    //TimeStamp = (int)(double)message["TimeStamp"]
                });
            }
        }
    }
}

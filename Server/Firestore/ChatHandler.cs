using System;
using System.Collections.Generic;
using System.Linq;
using Server.Network;

namespace Server.Firestore
{
    public static class ChatHandler
    {
        public static DateTime LastUpdate = DateTime.Now;
        public static void MessageSent(Mobile from, string channel, string message)
        {
            Int32 unixTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var docPath = "Chat/" + channel;
            var doc = new ChatChannelDoc((Dictionary<string,object>)DB.GetDocument(docPath));
            doc.Channel = channel;
            doc.LastUpdate = unixTimestamp;
            if(doc.Messages == null) doc.Messages = new List<ChatMessage>();
            doc.Messages.Add(new ChatMessage
            {
                Name = from.Name,
                AccountName = from.Account.Username,
                Consumed = true,
                FromSite = false,
                Message = message,
                TimeStamp = unixTimestamp
            });
            if(doc.Messages.Count > 50) doc.Messages.RemoveAt(0);
            DB.UpdateDocument(docPath, doc);
        }

        public static void HandleIncoming()
        {
            HandleIncomingChat();
            HandleIncomingAuthRequests();
            HandleIncomingCommands();
        }

        private static void HandleIncomingChat()
        {
            Int32 unixTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var docPath = "Chat";
            var docs = DB.GetDocuments(docPath);
            foreach (var d in docs)
            {
                var doc = new ChatChannelDoc((Dictionary<string,object>)d);
                if (doc.Messages.Any(x => !x.Consumed))
                {
                    var toProcess = doc.Messages.Where(x => !x.Consumed);
                    foreach (var message in toProcess)
                    {
                        foreach (var player in NetState.Instances)
                        {
                            player.Mobile.SendMessage(0x35, "{1} chat from site [{0}]: {2}", doc.Channel, message.Name, message.Message);
                        }
                        message.Consumed = true;
                    }

                    doc.LastUpdate = unixTimestamp;
                    DB.UpdateDocument("Chat/"+doc.Channel,doc);
                }
            }
        }

        private static void HandleIncomingAuthRequests()
        {
            var docPath = "AuthRequests";
            var docs = DB.GetDocuments(docPath);
            foreach (var doc in docs)
            {
                var authRequest = new SiteUserDoc((Dictionary<string,object>)doc);
                if (!authRequest.Pending) continue;
                var instance = NetState.Instances.FirstOrDefault(x => x.Account.Username == authRequest.Username);
                if (instance == null)
                {
                    authRequest.Confirmed = false;
                    authRequest.Pending = false;
                    authRequest.Response =
                        "You must be logged into the shard when you log into the site for the first time.";
                    DB.UpdateDocument(docPath+"/"+authRequest.Username, authRequest);
                    continue;
                }

                if (!instance.Account.CheckPassword(authRequest.Password))
                {
                    authRequest.Confirmed = false;
                    authRequest.Pending = false;
                    authRequest.Response =
                        "You have entered invalid login credentials.";
                    DB.UpdateDocument(docPath+"/"+authRequest.Username, authRequest);
                    continue;
                }

                authRequest.Confirmed = true;
                authRequest.Pending = false;
                authRequest.Response = "OK";
                authRequest.Password = "removed";
                if(!authRequest.Characters.Contains(instance.Mobile.Name)) authRequest.Characters.Add(instance.Mobile.Name);
                DB.UpdateDocument(docPath+"/"+authRequest.Username, authRequest);
            }
        }

        private static void HandleIncomingCommands()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Server.Firestore
{
    [FirestoreData]
    public class ActivityLogEntry
    {
        public ActivityLogEntry()
        {

        }

        public ActivityLogEntry(string accountUsername, string name, string action)
        {
            var utc = DateTime.UtcNow;
            Int32 unixTimestamp = (Int32)(utc.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Timestamp = utc;
            UnixTimestamp = unixTimestamp;
            AccountUsername = accountUsername;
            Name = name;
            Action = action;
        }

        [FirestoreProperty]
        public DateTime Timestamp { get; set; }

        [FirestoreProperty]
        public int UnixTimestamp { get; set; }

        [FirestoreProperty]
        public string AccountUsername { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Action { get; set; }
    }
}

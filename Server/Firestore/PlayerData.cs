using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Firestore
{
    [FirestoreData]
    public class PlayerData
    {
        public PlayerData()
        {
        }

        public PlayerData(Mobile m)
        {
            Name = m.Name;
            Alive = m.Alive;
            Dex = m.Dex;
            Fame = m.Fame;
            HitsMax = m.HitsMax;
            Hits = m.Hits;
            Int = m.Int;
            Karma = m.Karma;
            X = m.Location.X;
            Y = m.Location.Y;
            Z = m.Location.Z;
            Race = m.Race.ToString();
            Str = m.Str;
            Title = m.Title;
            Map = m.Map.ToString();
            Serial = m.Serial.ToString();
            AccountUsername = m.Account.Username;
            Skills = m.Skills.Select(x => $"{{{x.Name}:{x.Value}}}").ToArray();
        }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public bool Alive { get; set; }

        [FirestoreProperty]
        public bool LoggedIn { get; set; }

        [FirestoreProperty]
        public int Dex { get; set; }

        [FirestoreProperty]
        public int Fame { get; set; }

        [FirestoreProperty]
        public int HitsMax { get; set; }

        [FirestoreProperty]
        public int Hits { get; set; }

        [FirestoreProperty]
        public int Int { get; set; }

        [FirestoreProperty]
        public int Karma { get; set; }

        [FirestoreProperty]
        public int X { get; set; }

        [FirestoreProperty]
        public int Y { get; set; }

        [FirestoreProperty]
        public int Z { get; set; }

        [FirestoreProperty]
        public string Race { get; set; }

        [FirestoreProperty]
        public int Str { get; set; }

        [FirestoreProperty]
        public string Title { get; set; }

        [FirestoreProperty]
        public string Map { get; set; }

        [FirestoreProperty]
        public string Serial { get; set; }

        [FirestoreProperty]
        public string AccountUsername { get; set; }

        [FirestoreProperty]
        public string[] Skills { get; set; }

    }
}

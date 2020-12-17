using System;
using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace Server.Firestore
{
    [FirestoreData]
    public class SiteUserDoc
    {
        [FirestoreProperty]
        public string Username { get; set; }
        
        [FirestoreProperty]
        public string Password { get; set; }
        
        [FirestoreProperty]
        public string Token { get; set; }
        
        [FirestoreProperty]
        public bool Confirmed { get; set; }
        
        [FirestoreProperty]
        public bool Pending { get; set; }
        
        [FirestoreProperty]
        public string Response { get; set; }
        
        [FirestoreProperty]
        public List<string> Characters { get; set; }
        public SiteUserDoc() {}

        public SiteUserDoc(Dictionary<string, object> obj)
        {
            Username = (string)obj["Username"]??"";
            Password = (string)obj["Password"]??"";
            Token = (string)obj["Token"]??"";
            Confirmed = (bool)(obj["Confirmed"]??false);
            Pending = (bool)(obj["Pending"]??false);
            Response = (string) obj["Response"]??"";
            try
            {
                var characters = new List<string>();
                foreach (var ch in (List<object>) obj["Characters"])
                {
                    characters.Add((string) ch);
                }

                Characters = characters;
            }
            catch (Exception e)
            {
                Characters = new List<string>();
            }
        }
    }
}

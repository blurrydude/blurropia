using System;
using System.Collections.Generic;
using System.Linq;
using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Server.Firestore
{
    public static class DB
    {
        private static bool _enabled = false;
        public static FirestoreDb Root = FirestoreDb.Create("blurropia-uo");

        public static void UpdateDocument(string path, object obj)
        {
            if (!_enabled) return;
            try
            {
                var docref = Root.Document(path);
                docref.SetAsync(obj).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("Update Document "+path+": "+e.Message);
                Console.Write(e.StackTrace);
                Console.Write(JsonConvert.SerializeObject(e, Formatting.Indented));
            }
        }

        public static object GetDocument(string path)
        {
            if (!_enabled) return new object();
            try
            {
                var docref = Root.Document(path);
                return docref.GetSnapshotAsync().Result.ConvertTo<object>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Get Document "+path+": "+e.Message);
                return new object();
            }
        }

        public static List<object> GetDocuments(string path)
        {
            if (!_enabled) return new List<object>();
            try
            {
                var docref = Root.Collection(path);
                var returnList = new List<object>();
                foreach (var snapshot in docref.GetSnapshotAsync().Result.Documents)
                {
                    returnList.Add(snapshot.ConvertTo<Dictionary<string,object>>());
                }

                return returnList;
            }
            catch (Exception e)
            {
                Console.WriteLine("Get Documents "+path+": "+e.Message);
                return new List<object>();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Server;

namespace ServerUtilityExtensions
{

    public static class JsonUtility
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static object Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public static class RestUtility
    {
        public static string ExecuteRestCall(string endpoint, bool post, string token, string json)
        {
            try
            {
                var client = new RestClient(endpoint) {Timeout = 5000};
                var request = new RestRequest(post ? Method.POST : Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                if (!string.IsNullOrEmpty(token))
                {
                    request.AddHeader("Authorization", token);
                }
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = client.Execute(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                ConsoleUtility.OutputLine($"ExecuteRestCall failed.\n{ex.Message}\n{ex.InnerException?.Message}\n");
                Utility.PopColor();
                return string.Empty;
            }
        }
    }

    //public static class DatabaseUtility
    //{
    //    public static UoData GetUoData(string serial)
    //    {
    //        using (var db = new ApplicationDbContext())
    //        {
    //            return db.UoDatas.FirstOrDefault(x => x.Serial == serial);
    //        }
    //    }

    //    public static void SaveUoData(UoData data)
    //    {
    //        using (var db = new ApplicationDbContext())
    //        {
    //            if (string.IsNullOrEmpty(data.Serial) || !db.UoDatas.Any(x => x.Serial == data.Serial))
    //            {
    //                db.UoDatas.Add(data);
    //                db.SaveChanges();
    //                return;
    //            }
    //            db.Entry(data).State = EntityState.Modified;
    //            db.SaveChanges();
    //        }
    //    }

    //    public static void SaveMobileData(MobileData mobileData)
    //    {
    //        if (ConfigurationManager.AppSettings["UseDatabase"] == "true")
    //        {
    //            using (var db = new BlurropiaDbContext())
    //            {
    //                if (db.MobileDatas.All(x => x.Serial != mobileData.Serial))
    //                {
    //                    db.MobileDatas.Add(mobileData);
    //                    db.SaveChanges();
    //                    return;
    //                }

    //                db.Entry(mobileData).State = EntityState.Modified;
    //                db.SaveChanges();
    //            }
    //        }
    //    }

    //    public static void SaveMarketData(MarketData marketData)
    //    {
    //        if (ConfigurationManager.AppSettings["UseDatabase"] == "true")
    //        {
    //            using (var db = new BlurropiaDbContext())
    //            {
    //                if (db.MarketDatas.All(x => x.Item != marketData.Item))
    //                {
    //                    db.MarketDatas.Add(marketData);
    //                    db.SaveChanges();
    //                    return;
    //                }

    //                db.Entry(marketData).State = EntityState.Modified;
    //                db.SaveChanges();
    //            }
    //        }
    //    }

    //    public static MarketData GetMarketData(string item, string itemType, int defaultPrice, int defaultAvailability)
    //    {

    //        if (ConfigurationManager.AppSettings["UseDatabase"] == "true")
    //        {
    //            using (var db = new BlurropiaDbContext())
    //            {
    //                var result = db.MarketDatas.FirstOrDefault(x => x.Item == item);
    //                if (result != null && result.ItemType != itemType)
    //                {
    //                    result.ItemType = itemType;
    //                    db.SaveChanges();
    //                }

    //                if (result == null && !String.IsNullOrEmpty(item))
    //                {
    //                    SaveMarketData(new MarketData
    //                    {
    //                        Item = item,
    //                        ItemName = Regex.Replace(item, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0"),
    //                        Availability = defaultAvailability,
    //                        AveragePrice = defaultPrice,
    //                        BestPrice = defaultPrice,
    //                        HighestPrice = defaultPrice,
    //                        CurrentPrice = defaultPrice,
    //                        ItemType = itemType
    //                    });
    //                }

    //                return result;
    //            }
    //        }

    //        return null;
    //    }

    //    public static List<MarketData> GetAllMarketData()
    //    {
    //        if (ConfigurationManager.AppSettings["UseDatabase"] == "true")
    //        {
    //            using (var db = new BlurropiaDbContext())
    //            {
    //                return db.MarketDatas.ToList();
    //            }
    //        }

    //        return new List<MarketData>();
    //    }

    //    public static void SaveChatMessage(ChatLog entry)
    //    {
    //        if (ConfigurationManager.AppSettings["UseDatabase"] == "true")
    //        {
    //            using (var db = new BlurropiaDbContext())
    //            {
    //                db.ChatLogs.Add(entry);
    //                db.SaveChanges();
    //            }
    //        }
    //    }

    //    public static List<ChatLog> ReadPendingChatMessages()
    //    {

    //        if (ConfigurationManager.AppSettings["UseDatabase"] == "true")
    //        {
    //            using (var db = new BlurropiaDbContext())
    //            {
    //                return db.ChatLogs.Where(x => x.Pending).ToList();
    //            }
    //        }

    //        return new List<ChatLog>();
    //    }

    //    public static void UpdateChatMessages(IEnumerable<ChatLog> entries)
    //    {

    //        if (ConfigurationManager.AppSettings["UseDatabase"] == "true")
    //        {
    //            using (var db = new BlurropiaDbContext())
    //            {
    //                foreach (var entry in entries)
    //                {
    //                    db.Entry(entry).State = EntityState.Modified;
    //                }

    //                db.SaveChanges();
    //            }
    //        }
    //    }

    //    public static List<RemoteCommand> ReadPendingCommands()
    //    {

    //        if (ConfigurationManager.AppSettings["UseDatabase"] == "true")
    //        {
    //            using (var db = new BlurropiaDbContext())
    //            {
    //                return db.RemoteCommands.Where(x => x.Executed == null).ToList();
    //            }
    //        }

    //        return new List<RemoteCommand>();
    //    }

    //    public static void MarkCommandExecuted(RemoteCommand command)
    //    {
    //        if (ConfigurationManager.AppSettings["UseDatabase"] == "true")
    //        {
    //            using (var db = new BlurropiaDbContext())
    //            {
    //                var com = db.RemoteCommands.FirstOrDefault(x => x.Id == command.Id);
    //                if (com == null) return;
    //                com.Executed = DateTime.UtcNow;

    //                db.SaveChanges();
    //            }
    //        }
    //    }

    //    public static void DeleteChatMessages(IEnumerable<ChatLog> entries)
    //    {
    //        if (ConfigurationManager.AppSettings["UseDatabase"] == "true")
    //        {
    //            using (var db = new BlurropiaDbContext())
    //            {
    //                foreach (var entry in entries)
    //                {
    //                    db.Entry(entry).State = EntityState.Deleted;
    //                }

    //                db.SaveChanges();
    //            }
    //        }
    //    }

    //    public static void AddConsoleOutput(string entry)
    //    {

    //        if (ConfigurationManager.AppSettings["UseDatabase"] == "true")
    //        {
    //            using (var db = new BlurropiaDbContext())
    //            {
    //                var newOutput = new ConsoleOutput
    //                {
    //                    Message = entry,
    //                    Sent = DateTime.UtcNow
    //                };
    //                db.ConsoleOutputs.Add(newOutput);
    //                db.SaveChanges();
    //            }
    //        }
    //    }

    //    public static List<MarketHistory> GetMarketHistory(string item)
    //    {
    //        using (var db = new BlurropiaDbContext())
    //        {
    //            return db.MarketHistories.Where(x => x.Item == item).ToList();
    //        }
    //    }
    //}

    public static class ConsoleUtility
    {

        public static void Output(string format, params object[] args)
        {
            Console.Write(format, args);
            try
            {
                var entry = string.Format(format, args);
                OutputLine(entry);
            }
            catch (Exception)
            {
            }
        }

        public static void Output(string output)
        {
            Console.Write(output);
            try
            {
                OutputLine(output);
            }
            catch (Exception)
            {
            }
        }

        public static void OutputLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            try
            {
                var entry = string.Format(format, args);
                OutputLine(entry);
            }
            catch (Exception)
            {
            }
        }

        public static void OutputLine(string output)
        {
            Console.WriteLine(output);
            
        }

            public static void OutputLine(Config.Entry e)
            {
                Console.WriteLine(e);
                try
                {
                    OutputLine($"{e.Key}: {e.Value}");
                }
                catch (Exception)
                {
                }
            }

            public static void OutputLine()
        {
            Console.WriteLine();
            try
            {
                OutputLine(string.Empty);
            }
            catch (Exception)
            {
            }
        }

        public static void OutputLine(StackTrace stackTrace)
        {
            Console.WriteLine(stackTrace);
            try
            {
                OutputLine(stackTrace.ToString());
            }
            catch (Exception)
            {
            }
        }

        public static void OutputLine(Exception ex)
        {
            Console.WriteLine(ex);
            try
            {
                OutputLine(ex.Message);
                if (ex.InnerException != null)
                {
                    OutputLine(ex.InnerException.Message);
                }
                OutputLine(ex.StackTrace);
            }
            catch (Exception)
            {
            }
        }

        public static void OutputLine(object exceptionObject)
        {
            Console.WriteLine(exceptionObject);
            try
            {
                OutputLine(exceptionObject.ToString());
                if (exceptionObject.GetType() == typeof(Exception))
                {
                    OutputLine((Exception)exceptionObject);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

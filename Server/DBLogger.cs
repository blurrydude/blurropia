using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Firestore;

namespace Server.Services
{
    public static class DBLogger
    {
        public static void LogEvent(Mobile m, string action)
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var accountName = "SERVER";
            var name = "SYSTEM";
            if (m != null)
            {
                if (m.Account != null && m.Account.Username != null)
                {
                    accountName = m.Account.Username;
                }

                if (!String.IsNullOrEmpty(m.Name))
                {
                    name = m.Name;
                }
                else
                {
                    name = m.GetType().ToString().Split('.').Last();
                }
            }
            var entry = new ActivityLogEntry(accountName,name,action);
            var path = "ActivityLog/" + unixTimestamp;
            DB.UpdateDocument(path,entry);
        }
    }
}

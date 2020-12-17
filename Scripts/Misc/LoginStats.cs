using System;
using Server.Network;
using Server.Engines.Quests;
using Server.Mobiles;
using System.Collections.Generic;
using System.Linq;
using Server.Accounting;

namespace Server.Misc
{
    public class LoginStats
    {
        public static void Initialize()
        {
            // Register our event handler
            EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

        private static void EventSink_Login(LoginEventArgs args)
        {
            int userCount = NetState.Instances.Count;
            int itemCount = World.Items.Count;
            int mobileCount = World.Mobiles.Count;

            Mobile m = args.Mobile;

            m.SendMessage("Welcome, {0}! There {1} currently {2} user{3} online, with {4} item{5} and {6} mobile{7} in the world.",
                args.Mobile.Name,
                userCount == 1 ? "is" : "are",
                userCount, userCount == 1 ? "" : "s",
                itemCount, itemCount == 1 ? "" : "s",
                mobileCount, mobileCount == 1 ? "" : "s");

            if (m.IsStaff())
            {
                Server.Engines.Help.PageQueue.Pages_OnCalled(m);
            }
            
            var account = (Account)((PlayerMobile) m).Account;
            var lastStipend = account.Tags.FirstOrDefault(x => x.Name == "lastStipend");
            var loginDays = account.Tags.FirstOrDefault(x => x.Name == "loginDays");
            var doy = DateTime.Now.DayOfYear;
            if (lastStipend == null)
            {
                lastStipend = new AccountTag("lastStipend","0");
                
                account.Tags.Add(lastStipend);
            }

            if (loginDays == null)
            {
                if ((new[] {"Morrigan", "benfir", "benfir2", "Bagheera", "BlurryDude"}).Contains(account.Username))
                {
                    loginDays = new AccountTag("loginDays","6");
                }
                else
                {
                    loginDays = new AccountTag("loginDays","1");
                }
                
                account.Tags.Add(loginDays);
            }

            if (lastStipend.Value != doy.ToString())
            {
                var rollDay = doy - Int32.Parse(lastStipend.Value) == 1;
                lastStipend.Value = doy.ToString();
                var days = Int32.Parse(loginDays.Value);
                if (rollDay)
                {
                    days++;
                    loginDays.Value = days.ToString();
                }
                else
                {
                    days = 1;
                    loginDays.Value = "1";
                }

                if (days < 7)
                {
                    m.SendMessage(0x35,"You have earned a 500 gold stipend for logging in today.");
                    account.DepositGold(500);
                }
                else if (days == 7)
                {
                    m.SendMessage(0x35,"You've logged in seven days in a row, so we're doubling your stipend to 1,000 gold until you miss a day.");
                    account.DepositGold(1000);
                }
                else if (days > 7 && days < 14)
                {
                    m.SendMessage(0x35,"You've logged in seven or more days in a row, you've earned a 1,000 gold stipend.");
                    account.DepositGold(1000);
                }
                else if (days == 14)
                {
                    m.SendMessage(0x35,"You've logged in fourteen days in a row, so we're doubling your stipend to 2,000 gold until you miss a day.");
                    account.DepositGold(2000);
                }
                else if (days > 14)
                {
                    m.SendMessage(0x35,"You've logged in fourteen or more days in a row, you've earned a 2,000 gold stipend.");
                    account.DepositGold(2000);
                }
                else if (days > 0 && days % 28 == 0)
                {
                    var months = (int) (days / 28);
                    m.SendMessage(0x35,$"You've logged in every single day for {months} month{(months>1?"s":"")}. You earned a {months},000 gold bonus!");
                    account.DepositGold(months*1000);
                }
            }
        }
    }
}

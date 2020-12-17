using System;
using Server.Accounting;
using ServerUtilityExtensions;

namespace Server.Misc
{
    public class AccountPrompt
    {
        public static void Initialize()
        {
            if (Accounts.Count == 0 && !Core.Service)
            {
                ConsoleUtility.OutputLine("This server has no accounts.");
                ConsoleUtility.Output("Do you want to create the owner account now? (y/n)");

                string key = Console.ReadLine();
 
                if (key.ToUpper() == "Y")
                {
                    ConsoleUtility.OutputLine();

                    ConsoleUtility.Output("Username: ");
                    string username = Console.ReadLine();

                    ConsoleUtility.Output("Password: ");
                    string password = Console.ReadLine();

                    Account a = new Account(username, password);
                    a.AccessLevel = AccessLevel.Owner;

                    ConsoleUtility.OutputLine("Account created.");
                }
                else
                {
                    ConsoleUtility.OutputLine();

                    ConsoleUtility.OutputLine("Account not created.");
                }
            }
        }
    }
}
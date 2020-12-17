#region References
using System;
using System.Linq;
using System.Threading;

using Server.Accounting;
using Server.Engines.Help;
using Server.Network;
using ServerUtilityExtensions;

#endregion

namespace Server.Misc
{
	internal static class ServerConsole
	{
		private static readonly Func<string> _Listen = Console.ReadLine;

		private static string _Command;

		private static Timer _PollTimer;

		private static bool _HearConsole;

		public static void Initialize()
		{
			EventSink.ServerStarted += () =>
			{
				PollCommands();

				if (_HearConsole)
				{
					ConsoleUtility.OutputLine("Now listening to the whole shard.");
				}
			};

			EventSink.Speech += args =>
			{
				if (args.Mobile == null || !_HearConsole)
				{
					return;
				}

				try
				{
					if (args.Mobile.Region.Name.Length > 0)
					{
						ConsoleUtility.OutputLine(args.Mobile.Name + " (" + args.Mobile.Region.Name + "): " + args.Speech);
					}
					else
					{
						ConsoleUtility.OutputLine("" + args.Mobile.Name + ": " + args.Speech + "");
					}
				}
				catch
				{ }
			};
		}

		private static void PollCommands()
		{
			_PollTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMilliseconds(100), ProcessCommand);

			_Listen.BeginInvoke(r => ProcessInput(_Listen.EndInvoke(r)), null);
		}

		private static void ProcessInput(string input)
		{
			if (!Core.Crashed && !Core.Closing)
			{
				Interlocked.Exchange(ref _Command, input);
			}
		}

		private static void ProcessCommand()
		{
			if (Core.Crashed || Core.Closing || World.Loading || World.Saving)
			{
				return;
			}

			if (String.IsNullOrEmpty(_Command))
			{
				return;
			}

			ProcessCommand(_Command);

			Interlocked.Exchange(ref _Command, String.Empty);

			_Listen.BeginInvoke(r => ProcessInput(_Listen.EndInvoke(r)), null);
		}

		private static PageEntry[] _Pages;

		private static void ProcessCommand(string input)
		{
			input = input.Trim();

			if (_Pages != null)
			{
				HandlePaging(input);
				return;
			}

			if (input.StartsWith("pages", StringComparison.OrdinalIgnoreCase))
			{
				HandlePaging(input.Substring(5).Trim());
				return;
			}

			if (input.StartsWith("bc", StringComparison.OrdinalIgnoreCase))
			{
				var sub = input.Substring(2).Trim();

				BroadcastMessage(AccessLevel.Player, 0x35, String.Format("[Admin] {0}", sub));

				ConsoleUtility.OutputLine("[World]: {0}", sub);
				return;
			}

			if (input.StartsWith("sc", StringComparison.OrdinalIgnoreCase))
			{
				var sub = input.Substring(2).Trim();

				BroadcastMessage(AccessLevel.Counselor, 0x32, String.Format("[Admin] {0}", sub));

				ConsoleUtility.OutputLine("[Staff]: {0}", sub);
				return;
			}

			if (input.StartsWith("ban", StringComparison.OrdinalIgnoreCase))
			{
				var sub = input.Substring(3).Trim();

				var states = NetState.Instances;

				if (states.Count == 0)
				{
					ConsoleUtility.OutputLine("There are no players online.");
					return;
				}

				var ns = states.Find(o => o.Account != null && o.Mobile != null && Insensitive.StartsWith(sub, o.Mobile.RawName));

				if (ns != null)
				{
					ConsoleUtility.OutputLine("[Ban]: {0}: Mobile: '{1}' Account: '{2}'", ns, ns.Mobile.RawName, ns.Account.Username);

					ns.Dispose();
				}

				return;
			}

			if (input.StartsWith("kick", StringComparison.OrdinalIgnoreCase))
			{
				var sub = input.Substring(4).Trim();

				var states = NetState.Instances;

				if (states.Count == 0)
				{
					ConsoleUtility.OutputLine("There are no players online.");
					return;
				}

				var ns = states.Find(o => o.Account != null && o.Mobile != null && Insensitive.StartsWith(sub, o.Mobile.RawName));

				if (ns != null)
				{
					ConsoleUtility.OutputLine("[Kick]: {0}: Mobile: '{1}' Account: '{2}'", ns, ns.Mobile.RawName, ns.Account.Username);

					ns.Dispose();
				}

				return;
			}

			switch (input.Trim())
			{
				case "crash":
					{
						Timer.DelayCall(() => { throw new Exception("Forced Crash"); });
					}
					break;
				case "shutdown":
					{
						AutoSave.Save();
						Core.Kill(false);
					}
					break;
				case "shutdown nosave":
					{
						Core.Kill(false);
					}
					break;
				case "restart":
					{
						AutoSave.Save();
						Core.Kill(true);
					}
					break;
				case "restart nosave":
					{
						Core.Kill(true);
					}
					break;
				case "online":
					{
						var states = NetState.Instances;

						if (states.Count == 0)
						{
							ConsoleUtility.OutputLine("There are no users online at this time.");
						}

						foreach (var t in states)
						{
							var a = t.Account as Account;

							if (a == null)
							{
								continue;
							}

							var m = t.Mobile;

							if (m != null)
							{
								ConsoleUtility.OutputLine("- Account: {0}, Name: {1}, IP: {2}", a.Username, m.Name, t);
							}
						}
					}
					break;
				case "save":
					AutoSave.Save();
					break;
				case "hear": // Credit to Zippy for the HearAll script!
					{
						_HearConsole = !_HearConsole;

						ConsoleUtility.OutputLine("{0} sending speech to the console.", _HearConsole ? "Now" : "No longer");
					}
					break;
				default:
					DisplayHelp();
					break;
			}
		}

		private static void DisplayHelp()
		{
			ConsoleUtility.OutputLine(" ");
			ConsoleUtility.OutputLine("Commands:");
			ConsoleUtility.OutputLine("crash           - Forces an exception to be thrown.");
			ConsoleUtility.OutputLine("save            - Performs a forced save.");
			ConsoleUtility.OutputLine("shutdown        - Performs a forced save then shuts down the server.");
			ConsoleUtility.OutputLine("shutdown nosave - Shuts down the server without saving.");
			ConsoleUtility.OutputLine("restart         - Sends a message to players informing them that the server is");
			ConsoleUtility.OutputLine("                  restarting, performs a forced save, then shuts down and");
			ConsoleUtility.OutputLine("                  restarts the server.");
			ConsoleUtility.OutputLine("restart nosave  - Restarts the server without saving.");
			ConsoleUtility.OutputLine("online          - Shows a list of every person online:");
			ConsoleUtility.OutputLine("                  Account, Char Name, IP.");
			ConsoleUtility.OutputLine("bc <message>    - Type this command and your message after it.");
			ConsoleUtility.OutputLine("                  It will then be sent to all players.");
			ConsoleUtility.OutputLine("sc <message>    - Type this command and your message after it.");
			ConsoleUtility.OutputLine("                  It will then be sent to all staff.");
			ConsoleUtility.OutputLine("hear            - Copies all local speech to this console:");
			ConsoleUtility.OutputLine("                  Char Name (Region name): Speech.");
			ConsoleUtility.OutputLine("ban <name>      - Kicks and bans the users account.");
			ConsoleUtility.OutputLine("kick <name>     - Kicks the user.");
			ConsoleUtility.OutputLine("pages           - Enter page mode to handle help requests.");
			ConsoleUtility.OutputLine("help|?          - Shows this list.");
			ConsoleUtility.OutputLine(" ");
		}

		private static void DisplayPagingHelp()
		{
			ConsoleUtility.OutputLine(" ");
			ConsoleUtility.OutputLine("Paging Commands:");
			ConsoleUtility.OutputLine("view <id>              - View sender message.");
			ConsoleUtility.OutputLine("remove <id>            - Remove without message.");
			ConsoleUtility.OutputLine("handle <id> <message>  - Remove with message.");
			ConsoleUtility.OutputLine("clear                  - Clears the page queue.");
			ConsoleUtility.OutputLine("exit                   - Exit page mode.");
			ConsoleUtility.OutputLine("help|?                 - Shows this list.");
			ConsoleUtility.OutputLine(" ");
		}

		private static void HandlePaging(string sub)
		{
			if (sub.StartsWith("help", StringComparison.OrdinalIgnoreCase) ||
				sub.StartsWith("?", StringComparison.OrdinalIgnoreCase))
			{
				DisplayPagingHelp();

				HandlePaging(String.Empty);
				return;
			}

			if (PageQueue.List.Count == 0)
			{
				ConsoleUtility.OutputLine("There are no pages in the queue.");

				if (_Pages != null)
				{
					_Pages = null;

					ConsoleUtility.OutputLine("[Pages]: Disabled page mode.");
				}

				return;
			}

			if (String.IsNullOrWhiteSpace(sub))
			{
				if (_Pages == null)
				{
					ConsoleUtility.OutputLine("[Pages]: Enabled page mode.");

					DisplayPagingHelp();
				}

				_Pages = PageQueue.List.Cast<PageEntry>().ToArray();

				const string format = "{0:D3}:\t{1}\t{2}";

				for (var i = 0; i < _Pages.Length; i++)
				{
					ConsoleUtility.OutputLine(format, i + 1, _Pages[i].Type, _Pages[i].Sender);
				}

				return;
			}

			if (sub.StartsWith("exit", StringComparison.OrdinalIgnoreCase))
			{
				if (_Pages != null)
				{
					_Pages = null;

					ConsoleUtility.OutputLine("[Pages]: Disabled page mode.");
				}

				return;
			}

			if (sub.StartsWith("clear", StringComparison.OrdinalIgnoreCase))
			{
				if (_Pages != null)
				{
					foreach (var page in _Pages)
					{
						PageQueue.Remove(page);
					}

					ConsoleUtility.OutputLine("[Pages]: Queue cleared.");

					Array.Clear(_Pages, 0, _Pages.Length);

					_Pages = null;

					ConsoleUtility.OutputLine("[Pages]: Disabled page mode.");
				}

				return;
			}

			if (sub.StartsWith("remove", StringComparison.OrdinalIgnoreCase))
			{
				string[] args;

				var page = FindPage(sub, out args);

				if (page == null)
				{
					ConsoleUtility.OutputLine("[Pages]: Invalid page entry.");
				}
				else
				{
					PageQueue.Remove(page);

					ConsoleUtility.OutputLine("[Pages]: Removed from queue.");
				}

				HandlePaging(String.Empty);
				return;
			}

			if (sub.StartsWith("handle", StringComparison.OrdinalIgnoreCase))
			{
				string[] args;

				var page = FindPage(sub, out args);

				if (page == null)
				{
					ConsoleUtility.OutputLine("[Pages]: Invalid page entry.");

					HandlePaging(String.Empty);
					return;
				}

				if (args.Length <= 0)
				{
					ConsoleUtility.OutputLine("[Pages]: Message required.");

					HandlePaging(String.Empty);
					return;
				}

				page.Sender.SendGump(new MessageSentGump(page.Sender, ServerList.ServerName, String.Join(" ", args)));

				ConsoleUtility.OutputLine("[Pages]: Message sent.");

				PageQueue.Remove(page);

				ConsoleUtility.OutputLine("[Pages]: Removed from queue.");

				HandlePaging(String.Empty);
				return;
			}

			if (sub.StartsWith("view", StringComparison.OrdinalIgnoreCase))
			{
				string[] args;

				var page = FindPage(sub, out args);

				if (page == null)
				{
					ConsoleUtility.OutputLine("[Pages]: Invalid page entry.");

					HandlePaging(String.Empty);
					return;
				}

				var idx = Array.IndexOf(_Pages, page) + 1;

				ConsoleUtility.OutputLine("[Pages]: {0:D3}:\t{1}\t{2}", idx, page.Type, page.Sender);

				if (!String.IsNullOrWhiteSpace(page.Message))
				{
					ConsoleUtility.OutputLine("[Pages]: {0}", page.Message);
				}
				else
				{
					ConsoleUtility.OutputLine("[Pages]: No message supplied.");
				}

				HandlePaging(String.Empty);
				return;
			}

			if (_Pages != null)
			{
				string[] args;

				var page = FindPage(sub, out args);

				if (page != null)
				{
					var idx = Array.IndexOf(_Pages, page) + 1;

					ConsoleUtility.OutputLine("[Pages]: {0:D3}:\t{1}\t{2}", idx, page.Type, page.Sender);

					if (!String.IsNullOrWhiteSpace(page.Message))
					{
						ConsoleUtility.OutputLine("[Pages]: {0}", page.Message);
					}
					else
					{
						ConsoleUtility.OutputLine("[Pages]: No message supplied.");
					}

					HandlePaging(String.Empty);
					return;
				}

				Array.Clear(_Pages, 0, _Pages.Length);

				_Pages = null;

				ConsoleUtility.OutputLine("[Pages]: Disabled page mode.");
			}
		}

		private static PageEntry FindPage(string sub, out string[] args)
		{
			args = sub.Split(' ');

			if (args.Length > 1)
			{
				sub = args[1];

				if (args.Length > 2)
				{
					args = args.Skip(2).ToArray();
				}
				else
				{
					args = args.Skip(1).ToArray();
				}
			}

			int id;

			if (Int32.TryParse(sub, out id) && --id >= 0 && id < _Pages.Length)
			{
				var page = _Pages[id];

				if (PageQueue.List.Contains(page))
				{
					return page;
				}
			}

			return null;
		}

		public static void BroadcastMessage(AccessLevel ac, int hue, string message)
		{
			World.Broadcast(hue, false, ac, message);
		}
	}
}
#region References
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Server.Commands;
using Server.Guilds;
using Server.Network;
using Server.Engines.Chat;
using Server.Items;
using ServerUtilityExtensions;
using Server.Firestore;
#endregion

namespace Server.Misc
{
	public class StatusPage : Timer
	{
		public static readonly bool Enabled = true;

		private static HttpListener _Listener;

		private static string _StatusPage = String.Empty;
		private static byte[] _StatusBuffer = new byte[0];

		private static readonly object _StatusLock = new object();

		public static void Initialize()
		{
			if (!Enabled)
			{
				return;
			}

			new StatusPage().Start();
		    //EventSink.Disconnected += new DisconnectedEventHandler(EventSink_Disconnected);

			Listen();
		}

	    private static void EventSink_Disconnected(DisconnectedEventArgs e)
        {
        }

	    private static void Listen()
		{
			/*if (!HttpListener.IsSupported)
			{
				return;
			}

			if (_Listener == null)
			{
				_Listener = new HttpListener();
				_Listener.Prefixes.Add("http://*:80/status/");
				_Listener.Start();
			}
			else if (!_Listener.IsListening)
			{
				_Listener.Start();
			}

			if (_Listener.IsListening)
			{
				_Listener.BeginGetContext(ListenerCallback, null);
			}*/
		}

		private static void ListenerCallback(IAsyncResult result)
		{
			try
			{
				var context = _Listener.EndGetContext(result);

				byte[] buffer;

				lock (_StatusLock)
				{
					buffer = _StatusBuffer;
				}

				context.Response.ContentLength64 = buffer.Length;
				context.Response.OutputStream.Write(buffer, 0, buffer.Length);
				context.Response.OutputStream.Close();
			}
			catch
			{ }

			Listen();
		}

		private static string Encode(string input)
		{
			var sb = new StringBuilder(input);

			sb.Replace("&", "&amp;");
			sb.Replace("<", "&lt;");
			sb.Replace(">", "&gt;");
			sb.Replace("\"", "&quot;");
			sb.Replace("'", "&apos;");

			return sb.ToString();
		}

		public StatusPage()
			: base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
		{
			Priority = TimerPriority.FiveSeconds;
		}

		protected override void OnTick()
		{
			if (!Directory.Exists("web"))
			{
				Directory.CreateDirectory("web");
			}

			foreach (Mobile m in NetState.Instances.Where(state => state.Mobile != null).Select(state => state.Mobile))
			{
			    var pdata = new PlayerData(m);
			    pdata.LoggedIn = true;
			    DB.UpdateDocument("PlayerData/"+m.Account.Username + "~" + m.Name, pdata);
			}
            /*
            var incomingChat = DatabaseUtility.ReadPendingChatMessages();
            foreach (var entry in incomingChat)
            {
                var user = ChatUser.GetChatUsers(entry.CharacterName).FirstOrDefault();
                
                if (user != null)
                {
                    var channel = Channel.FindChannelByName(entry.Channel) ?? Channel.Default;
                    ChatActionHandlers.ChannelMessage(user, channel, entry.Message, entry.Pending);
                    entry.CharacterName = user.Username;
                }
                entry.Pending = false;
            }
            DatabaseUtility.UpdateChatMessages(incomingChat);

            var pendingRemoteCommands = DatabaseUtility.ReadPendingCommands();
		    var command = pendingRemoteCommands.FirstOrDefault(x => x.DelayTill == null || x.DelayTill < DateTime.UtcNow);
            if (command != null)
            {
                try
                {
                    if (command.Command.Substring(0,9) == "sysbcast:")
                    {
                        var remainingCommand = command.Command.Substring(9, command.Command.Length - 9);
                        var split = remainingCommand.Split(':');
                        var hue = Convert.ToInt32(split[0]);
                        World.Broadcast(hue, true, "SYSTEM: "+split[1]);
                    }
                    else if (command.Command == "syncscripts")
                    {
                        ConsoleUtility.OutputLine("Alerting players about pending update and wait one minute...");
                        World.Broadcast(0x22, true, "The server will restart in one minute for script updates. We should be back up in just a couple minutes.");
                        Thread.Sleep(60000);
                        ConsoleUtility.OutputLine("Pulling scripts and executables from dev...");
                        var process = Process.Start(@"RELEASE_UO.bat");
                        while (!process.HasExited)
                        {
                            ConsoleUtility.OutputLine("waiting for pull to finish...");
                            Thread.Sleep(1000);
                        }
                        ConsoleUtility.OutputLine("Issuing restart command");
                        CommandSystem.Handle(World.Mobiles.First(x => x.Key.ToString() == command.Serial).Value,
                            "[restart");
                    }
                    else
                    {
                        CommandSystem.Handle(World.Mobiles.First(x => x.Key.ToString() == command.Serial).Value,
                            command.Command);
                    }
                } catch(Exception ex) { ConsoleUtility.OutputLine(ex); }
                DatabaseUtility.MarkCommandExecuted(command);
            }*/

            /*using (var op = new StreamWriter("web/status.html"))
            {
                op.WriteLine("<!DOCTYPE html>");
                op.WriteLine("<html>");
                op.WriteLine("   <head>");
                op.WriteLine("      <title>" + ServerList.ServerName + " Server Status</title>");
                op.WriteLine("   </head>");
                op.WriteLine("   <style type=\"text/css\">");
                op.WriteLine("   body { background: #999; }");
                op.WriteLine("   table { width: 100%; }");
                op.WriteLine("   tr.ruo-header td { background: #000; color: #FFF; }");
                op.WriteLine("   tr.odd td { background: #222; color: #DDD; }");
                op.WriteLine("   tr.even td { background: #DDD; color: #222; }");
                op.WriteLine("   </style>");
                op.WriteLine("   <body>");
                op.WriteLine("      <h1>" + ServerList.ServerName + "Server Status</h1>");
                op.WriteLine("      <h3>Online clients</h3>");
                op.WriteLine("      <table cellpadding=\"0\" cellspacing=\"0\">");
                op.WriteLine("         <tr class=\"ruo-header\"><td>Name</td><td>Location</td><td>Kills</td><td>Karma/Fame</td></tr>");

                var index = 0;

                foreach (var m in NetState.Instances.Where(state => state.Mobile != null).Select(state => state.Mobile))
                {
                    ++index;

                    var g = m.Guild as Guild;

                    op.Write("         <tr class=\"ruo-result " + (index % 2 == 0 ? "even" : "odd") + "\"><td>");

                    if (g != null)
                    {
                        op.Write(Encode(m.Name));
                        op.Write(" [");

                        var title = m.GuildTitle;

                        title = title != null ? title.Trim() : String.Empty;

                        if (title.Length > 0)
                        {
                            op.Write(Encode(title));
                            op.Write(", ");
                        }

                        op.Write(Encode(g.Abbreviation));

                        op.Write(']');
                    }
                    else
                    {
                        op.Write(Encode(m.Name));
                    }

                    op.Write("</td><td>");
                    op.Write(m.X);
                    op.Write(", ");
                    op.Write(m.Y);
                    op.Write(", ");
                    op.Write(m.Z);
                    op.Write(" (");
                    op.Write(m.Map);
                    op.Write(")</td><td>");
                    op.Write(m.Kills);
                    op.Write("</td><td>");
                    op.Write(m.Karma);
                    op.Write(" / ");
                    op.Write(m.Fame);
                    op.WriteLine("</td></tr>");
                }

                op.WriteLine("         <tr>");
                op.WriteLine("      </table>");
                op.WriteLine("   </body>");
                op.WriteLine("</html>");
            }*/

            /*lock (_StatusLock)
            {
                _StatusPage = File.ReadAllText("web/status.html");
                _StatusBuffer = Encoding.UTF8.GetBytes(_StatusPage);
            }*/
        }
	}
}

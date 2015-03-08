using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using gameServer.Game;
using gameServer.Tools;
using gameServer.Game.Misc;
using gameServer.Game.World;
using gameServer.Game.Caches;
using gameServer.Game.Objects;

namespace gameServer.Packets.Handlers.Commands
{
	public class CommandProcessor
	{
		public delegate void CommandHandler(MartialClient c, InCommand cmd);

		public static string Label { get; private set; }
		private static Hashtable c_handlers;
		private int c_count;
		public int Count { get { return c_count; } }
		private static CommandProcessor c_processor;

		public static Hashtable getCommandHandlers()
		{
			return c_handlers;
		}

		public CommandProcessor(string label)
		{
			Label = label;
			c_handlers = new Hashtable();
		}

		private void AppendHandler(string command, CommandHandler handler)
		{
			c_handlers[command] = handler;
			c_count++;
		}

		public static void InitCommandHandlers()
		{
			c_processor = new CommandProcessor("MainProcessor");

			c_processor.AppendHandler("/commands",	Packets.Handlers.Commands.CHandlers.Help.ListCommands);
			c_processor.AppendHandler("/announce",	Packets.Handlers.Commands.CHandlers.MessagingRelated.Announce);
			c_processor.AppendHandler("/message",	Packets.Handlers.Commands.CHandlers.MessagingRelated.Message);
			c_processor.AppendHandler("/item",		Packets.Handlers.Commands.CHandlers.ItemsRelated.ItemCreate);
			c_processor.AppendHandler("/goto",		Packets.Handlers.Commands.CHandlers.MovementRelated.Warp);
			c_processor.AppendHandler("/setfame",	Packets.Handlers.Commands.CHandlers.PlayerRelated.SetFame);
			c_processor.AppendHandler("/setlevel",	Packets.Handlers.Commands.CHandlers.PlayerRelated.SetLevel);
			c_processor.AppendHandler("/setmhp",	Packets.Handlers.Commands.CHandlers.PlayerRelated.SetMHP);
		}

		public static void ParseCommand(MartialClient c, string[] cmd)
		{
			InCommand p = new InCommand(cmd[0].ToLower(), cmd.Length == 1 ? null : cmd.Skip(1).ToArray());

			CommandHandler handler = null;
			handler = c_processor[p.commandName];

			if(handler != null)
			{
				handler(c, p);
			}
			else
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Command '" + cmd[0] + "' wasn't found.");
				return;
			}
		}

		private CommandHandler this[string command]
		{
			get
			{
				Logger.WriteLog(Logger.LogTypes.Info, "Command received: " + command + ".");
				try
				{
					return (CommandHandler)c_handlers[command];
				}
				catch
				{
					return null;
				}
			}
		}
	}
}
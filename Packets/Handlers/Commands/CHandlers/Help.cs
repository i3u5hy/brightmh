using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gameServer.Packets.Handlers.Commands;

namespace gameServer.Packets.Handlers.Commands.CHandlers
{
	class Help
	{
		public static void ListCommands(MartialClient c, InCommand cmd)
		{
			StaticPackets.sendSystemMessageToClient(c, 1, "Available commands: ");
			foreach(string cmdName in CommandProcessor.getCommandHandlers().Keys)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, cmdName);
			}
		}
	}
}

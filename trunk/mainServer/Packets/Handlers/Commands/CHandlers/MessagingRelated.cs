using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gameServer.Packets.Handlers.Commands.CHandlers
{
	public class MessagingRelated
	{
		public static void Message(MartialClient c, InCommand cmd)
		{

		}

		public static void Announce(MartialClient c, InCommand cmd)
		{
			if(cmd.commandArgs == null)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "/announce [text]");
				return;
			}
			string announcing = string.Join(" ", cmd.commandArgs);
			StaticPackets.sendWorldAnnounce(announcing);
			return;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gameServer.Game;
using gameServer.Tools;
using gameServer.Game.Misc;
using gameServer.Game.World;
using gameServer.Game.Caches;
using gameServer.Game.Objects;

namespace gameServer.Packets.Handlers.Commands.CHandlers
{
	class PlayerRelated
	{
		public static void SetMHP(MartialClient c, InCommand cmd)
		{
			if(cmd.commandArgs.Length != 2)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "/setmhp [name] [amount]");
				return;
			}

			Character player = WMap.Instance.findPlayerByName(cmd.commandArgs[0]);
			if(player == null)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Player wasn't found.");
				return;
			}

			int amount = -1;
			if(!Int32.TryParse(cmd.commandArgs[1], out amount))
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Server wasn't able to parse amount of MHPoints.");
				return;
			}

			player.getAccount().MHPoints = amount;
			StaticPackets.setMHPoints(player.getAccount().mClient, amount);
			return;
		}

		public static void SetLevel(MartialClient c, InCommand cmd)
		{
			if(cmd.commandArgs.Length < 2)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "/setlevel [name] [amount]");
				return;
			}

			Character player = WMap.Instance.findPlayerByName(cmd.commandArgs[0]);
			if(player == null)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Wrong player name has been given.");
				return;
			}

			byte level;
			if(!Byte.TryParse(cmd.commandArgs[1], out level))
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Parameters must be values!");
				return;
			}

			if(level < 0 || level > 255)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Setlevel range goes from 0 - 255.");
				return;
			}

			StaticPackets.setCharacterLevel(player, level);
			StaticPackets.sendSystemMessageToClient(c, 1, player.getName() + "'s level has been set up to: " + level + "!");
			return;
		}

		public static void SetFame(MartialClient c, InCommand cmd)
		{
			if(cmd.commandArgs == null)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "/setfame [name] [amount]");
				return;
			}

			Character player = WMap.Instance.findPlayerByName(cmd.commandArgs[0]);
			if(player == null)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Wrong player name has been given.");
				return;
			}

			int fameAmount;
			if(!Int32.TryParse(cmd.commandArgs[1], out fameAmount))
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Parameters must be values!");
				return;
			}

			if(fameAmount < 0 || fameAmount > 2147483647)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Setfame range goes from 0 - 2147483647.");
				return;
			}

			StaticPackets.setCharacterFame(player, fameAmount);
			StaticPackets.sendSystemMessageToClient(c, 1, player.getName() + "'s fame has been set up to: " + fameAmount + "!");
			return;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gameServer.Tools;
using gameServer.Game;
using gameServer.Game.Misc;
using gameServer.Game.World;
using gameServer.Game.Caches;

namespace gameServer.Packets.Handlers.Commands.CHandlers
{
	class MovementRelated
	{
		public static void Warp(MartialClient c, InCommand cmd)
		{
			if(cmd.commandArgs.Length < 2)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "/goto [x] [y] [map] | [Mob|NPC|Player] [uID/name] | [map] true");
				return;
			}

			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Null activity in command handler");
				c.Close();
				return;
			}
			Character chr = c.getAccount().activeCharacter;

			short goMap = -1;
			float goX = -1;
			float goY = -1;

			if(cmd.commandArgs.Length == 2)
			{
				switch(cmd.commandArgs[0].ToLower())
				{
					case "npc":
					{
						int npcID = -1;
						if(!Int32.TryParse(cmd.commandArgs[1], out npcID))
						{
							StaticPackets.sendSystemMessageToClient(c, 1, "Server wasn't able to parse NPC's ID.");
							return;
						}

						if(WMap.Instance.getNpcs().ElementAtOrDefault(npcID) == null)
						{
							StaticPackets.sendSystemMessageToClient(c, 1, "Server wasn't able to find NPC of ID " + npcID + "!");
							return;
						}

						NPC npc = WMap.Instance.getNpcs()[npcID];

						goMap = npc.getMap();
						goX = npc.getPosition()[0];
						goY = npc.getPosition()[1];
						break;
					}
					/*case "mob":
					{
						int mobID = -1;
						if(!Int32.TryParse(cmd.commandArgs[1], out mobID))
						{
							StaticPackets.sendSystemMessageToClient(c, 1, "Server wasn't able to parse Mob's ID.");
							return;
						}

						Mob mob = WMap.Instance.getGrid(chr.getMap()).findMobByuID(mobID);
						if(mob == null)
						{
							StaticPackets.sendSystemMessageToClient(c, 1, "Server wasn't able to find Mob of ID " + mobID + "!");
							return;
						}

						goMap = mob.getMap();
						goX = mob.getPosition()[0];
						goY = mob.getPosition()[1];
						break;
					}*/
					case "player":
					{
						Character player = WMap.Instance.findPlayerByName(cmd.commandArgs[1]);
						if(player == null)
						{
							StaticPackets.sendSystemMessageToClient(c, 1, "Player wasn't found.");
							return;
						}

						goMap = player.getMap();
						goX = player.getPosition()[0];
						goY = player.getPosition()[1];
						break;
					}
					default:
					{
						if(!MiscFunctions.IsNumeric(cmd.commandArgs[0]) || cmd.commandArgs[1] != "true" && !MiscFunctions.IsNumeric(cmd.commandArgs[1]))
						{
							StaticPackets.sendSystemMessageToClient(c, 1, "/goto [x] [y] [map] | [Mob|NPC|Player] [uID/name] | [map] true");
							return;
						}

						Waypoint closestTown = null;
						short _desiredMap = -1;
						if(!Int16.TryParse(cmd.commandArgs[0], out _desiredMap))
						{
							StaticPackets.sendSystemMessageToClient(c, 1, "Server wasn't able to parse your desired map's ID!");
							return;
						}
						
						if(cmd.commandArgs[1] == "true")
						{
							closestTown = TownCoordsCache.Instance.getClosestWaypointForMap(_desiredMap, new Waypoint(0, 0));
							if(closestTown == null)
							{
								StaticPackets.sendSystemMessageToClient(c, 1, "There's not any town on map " + _desiredMap + "!");
								return;
							}
						}
						else if(MiscFunctions.IsNumeric(cmd.commandArgs[1]))
						{
							int _desiredTown = -1;
							if(!Int32.TryParse(cmd.commandArgs[1], out _desiredTown))
							{
								StaticPackets.sendSystemMessageToClient(c, 1, "Server wasn't able to parse your desired town's index!");
								return;
							}

							closestTown = TownCoordsCache.Instance.getWaypointAtIndexForMap(_desiredMap, _desiredTown);
							if(closestTown == null)
							{
								StaticPackets.sendSystemMessageToClient(c, 1, "There's not any town on map " + _desiredMap + " with index " + _desiredTown + "!");
								return;
							}
						}

						goMap = Convert.ToInt16(cmd.commandArgs[0]);
						goX = closestTown.getX();
						goY = closestTown.getY();
						break;
					}
				}
			}
			else if(cmd.commandArgs.Length == 3)
			{
				foreach(string parser in cmd.commandArgs)
					if(!MiscFunctions.IsNumeric(parser))
					{
						StaticPackets.sendSystemMessageToClient(c, 1, "Parameters must be values!");
						return;
					}

				goMap = Convert.ToInt16(cmd.commandArgs[2]);
				goX = Convert.ToSingle(cmd.commandArgs[0]);
				goY = Convert.ToSingle(cmd.commandArgs[1]);
			} else return;

			CharacterFunctions.setPlayerPosition(c.getAccount().activeCharacter, goX, goY, goMap);
			return;
		}
	}
}

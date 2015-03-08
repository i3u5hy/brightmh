using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gameServer.Core.IO;
using gameServer.Tools;
using gameServer.Game;
using gameServer.Game.Objects;
using gameServer.Servers;

namespace gameServer.Packets.Handlers
{
	public class CommunityManagement
	{
		public static void RefreshFriends(MartialClient c, InPacket p)
		{

		}

		public static void SendMessage(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook SendMessage handling while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			int messageType = p.ReadInt(); // identifier of message type maybe?
			string receiver = p.ReadString(20); // receiver name
			MiscFunctions.obscureString(receiver);
			if(!MySQLTool.NameTaken(receiver))
			{
				
				return;
			}
			int messageLength = p.ReadInt(); // message length
			string message = p.ReadString(messageLength);
			MiscFunctions.obscureString(message);

			OutPacket op = new OutPacket(20);
			op.WriteInt(20);
			op.WriteShort(4);
			op.WriteShort(0x53);
			op.WriteInt(718349825);
			op.WriteInt(chr.getuID());
			op.WriteInt(-1089732352);
			c.WriteRawPacket(op.ToArray());
		}

		public static void HandleFriends(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook HandleFriends while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte managementType = p.ReadByte();
			byte communityIndex = p.ReadByte();
			string personName = MiscFunctions.obscureString(p.ReadString(16));

			Community com = chr.getCommunity();

			switch(managementType)
			{
				case 0:
					case 1:
					{
						if(!com.addPersona(managementType, communityIndex, personName))
						{
							StaticPackets.sendSystemMessageToClient(c, 1, "Sorry. Something went wrong!");
							return;
						}
						break;
					}
				case 2:
					case 3:
					{
						if(!com.removePersona((byte)(managementType - 2), communityIndex))
						{
							StaticPackets.sendSystemMessageToClient(c, 1, "Sorry. Something went wrong!");
							return;
						}
						break;
					}
				default:
				{
					//tuffnucks you!
					return;
				}
			}
			
			OutPacket op = new OutPacket(40);
			op.WriteInt(40);
			op.WriteShort(0x04);
			op.WriteShort(0x31);
			op.WriteInt(134652417);
			op.WriteInt(chr.getuID());
			op.WriteShort(1);
			op.WriteByte(managementType);
			op.WriteByte(communityIndex);
			op.WritePaddedString(personName, 16);
			op.WriteInt(-1089495552);
			c.WriteRawPacket(op.ToArray());
		}
	}
}

using System.Linq;
using System.Text.RegularExpressions;
using gameServer.Core.IO;
using gameServer.Game;
using gameServer.Game.World;
using gameServer.Tools;
using gameServer.Packets.Handlers.Commands;

namespace gameServer.Packets.Handlers
{
	static class PlayerState
	{
		public static void Chat(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Hooked chat with null of activeCharacter");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte messageType = (byte)p.ReadShort();
			string receiver = MiscFunctions.obscureString(p.ReadString(17));
			byte messageLength = (byte)p.ReadInt();
			if(messageLength > 65)
			{
				Logger.LogCheat(Logger.HackTypes.Chat, c, "Tried to send a message of size {0}", messageLength);
				c.Close();
				return;
			}
			string message = p.ReadString(messageLength);

			switch(messageType)
			{
				case 0: WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), StaticPackets.chatRelay(chr, messageType, message)); break;
				case 1:
				{
					if(receiver == null)
						return;

					Character player = WMap.Instance.findPlayerByName(receiver);
					if(player == null)
					{
						chr.getAccount().mClient.WriteRawPacket(StaticPackets.playerIsntConnected(chr));
						break;
					}

					player.getAccount().mClient.WriteRawPacket(StaticPackets.chatRelay(chr, messageType, message));
					break;
				}
				case 6: // karma notice
				{
					if(chr.getKarmaMessagingTimes() == 0)
					{
						
					}

					WMap.Instance.sendToAllCharactersExcept(chr, StaticPackets.chatRelay(chr, messageType, message));
					break;
				}
				case 7: // "GM Shout"
				{
					if(chr.getAccount().gmLvl == 0 && chr.getGMShoutMessagingCounts() == 0)
					{

					}

					WMap.Instance.sendToAllCharactersExcept(chr, StaticPackets.chatRelay(chr, messageType, message));
					break;
				}
				case 9: // admin commands
				{
					string[] cmd = Regex.Split(message, " ");
					if(chr.getAccount().gmLvl == 0)
					{
						Logger.LogCheat(Logger.HackTypes.Chat, c, "Tried to parse GM Command {0}", cmd[0]);
						c.Close();
						break;
					}
					
					if(cmd.Length == 0)
					{
						Logger.LogCheat(Logger.HackTypes.Chat, c, "Tried to parse null GM Command");
						c.Close();
						break;
					}

					if(cmd[0][0] != '/')
					{
						Logger.LogCheat(Logger.HackTypes.Chat, c, "Tried to parse command without '/' slash");
						c.Close();
						break;
					}

					CommandProcessor.ParseCommand(c, cmd);
					break;
				}
				default:
				{
					break;
				}
			}
		}

		public static void Movement(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Hooked Movement with null of activeCharacter");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			p.Skip(4);
			byte[] locationToX = p.ReadBytes(4);
			byte[] locationToY = p.ReadBytes(4);
			p.Skip(1);
			byte movingMode = p.ReadByte();
			MoveCharacterPacket.HandleMovement(chr, locationToX, locationToY, movingMode);
		}

		public static void AttackDefendState(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Hooked playerState with null of activeCharacter");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			OutPacket op = new OutPacket(24);
			op.WriteInt(24);
			op.WriteShort(0x05);
			op.WriteShort(0x06);
			op.WriteByte(0x01);
			op.WriteByte(0x33);
			op.WriteByte(0x15);
			op.WriteByte(0x08);
			op.WriteInt(chr.getuID());
			op.WriteShort(p.ReadByte());
			op.WriteByte(0x10);
			op.WriteByte(0x29);

			WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), op.ToArray());
		}

		public static void Fury(MartialClient c, InPacket p)
		{
			return;
		}

		public static void ToggleMutationEffect(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Hooked toggleMutationEffect with null of activeCharacter");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte toggleType = p.ReadByte();

			OutPacket op = new OutPacket(20);
			op.WriteInt(20);
			op.WriteShort(0x05);
			op.WriteShort(0X7c);
			op.WriteInt(140235265);
			op.WriteInt(chr.getuID());
			op.WriteInt(toggleType == 0 ? 716251136 : 716251314);

			WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr.getArea(), op.ToArray());
		}

		public static void UnknownStatimizer(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Hooked playerState with null of activeCharacter");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			OutPacket op = new OutPacket(24);
			op.WriteInt(24);
			op.WriteShort(4);
			op.WriteShort(47);
			op.WriteInt(1);
			op.WriteInt(chr.getuID());
			op.WriteByte();
			op.WriteByte();
			op.WriteByte(1);
			op.WriteByte();
			op.WriteByte();
			op.WriteByte();
			op.WriteByte();
			op.WriteByte(); // ok, there's some magic on those bytes, for ex. 3rd byte tells you, if you're able to trade in wild zone
			chr.getAccount().mClient.WriteRawPacket(op.ToArray());
		}
	}
}

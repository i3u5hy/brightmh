using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gameServer.Game.Objects;
using gameServer.Core.IO;
using gameServer.Tools;
using gameServer.Game;
using gameServer.Game.World;

namespace gameServer.Packets.Handlers
{
	public class GuildManagement
	{
		public static void DeclareWar(MartialClient c, InPacket p)
		{
			return;
		}
		
		public static void CreateGuild(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Hooked guild.Refresh with null of activeCharacter");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			Guild guild = chr.getGuild();

			byte managementType = p.ReadByte(); // 0 - disband; 1 - create; 2 - donate; 3 - hat
			byte managementArg	= p.ReadByte(); // 1 - bang, 2 - mun, 3 - pa, 4 - nohing, 5 - dan, 6 - gak, 7 - gyo, 8 - gung
			string managementName = MiscFunctions.obscureString(p.ReadString(18));

			switch(managementType)
			{
				case 0:
				{
					if(guild == null)
					{
						Logger.LogCheat(Logger.HackTypes.Guild, c, "Hooked guild disband with char that ain't in guild");
						c.Close();
						return;
					}

					guild.Delete();
					WMap.Instance.removeGuild(guild);
					foreach(GuildMember i in guild.guildMembers)
					{
						Character tmp = i.getOnlineCharacter();
						tmp.getAccount().mClient.WriteRawPacket(GuildPackets.quitGuildForInternal(tmp));
						WMap.Instance.getGrid(tmp.getMap()).sendTo3x3Area(tmp, tmp.getArea(), GuildPackets.quitGuildForExternals(tmp));
						tmp.setGuild(null);
						i.setOnlineCharacter(null);
					}
					return;
				}
				case 1:
				{
					if(guild != null)
					{
						Logger.LogCheat(Logger.HackTypes.Guild, c, "Hooked guild create with char that is in guild");
						c.Close();
						return;
					}

					Guild newGuild = new Guild(managementArg, managementName);
					c.WriteRawPacket(GuildPackets.createGuildResponse(chr, managementType, managementArg, managementName));
					WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), GuildPackets.extCharGuild(chr));
					return;
				}
				case 2:
				{
					if(guild == null)
					{
						Logger.LogCheat(Logger.HackTypes.Guild, c, "Hooked guild donate with char that ain't in guild");
						c.Close();
						return;
					}


					return;
				}
				case 3:
				{
					if(guild == null)
					{
						Logger.LogCheat(Logger.HackTypes.Guild, c, "Hooked guild hat change with char that ain't in guild");
						c.Close();
						return;
					}

					guild.guildHat = managementArg;
					WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), GuildPackets.extCharGuild(chr));
					c.WriteRawPacket(GuildPackets.refreshGuild(chr));
					return;
				}
			}
			return;
		}

		public static void UpdateNews(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Hooked guild.Refresh with null of activeCharacter");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			if(chr.getGuild() == null)
			{
				Logger.LogCheat(Logger.HackTypes.Guild, c, "Hooked guild news with char that ain't in guild");
				c.Close();
				return;
			}

			Guild guild = chr.getGuild();

			if(chr.getGuild() == null)
			{
				Logger.LogCheat(Logger.HackTypes.Guild, c, "Hooked guild news with char that ain't in guild");
				c.Close();
				return;
			}

			GuildMember gMember = guild.findGuildMember(chr.getuID());

			if(gMember.getGuildRank() < 4) // aint master?
			{
				Logger.LogCheat(Logger.HackTypes.Guild, c, "Hooked guildManagement with char that ain't a guild master");
				c.Close();
				return;
			}

			string news = MiscFunctions.obscureString(p.ReadString(195));

			guild.guildNews = news;

			guild.sendToGuildMembers(GuildPackets.getRefreshNewsGuildPacket(news));
		}

		public static void Refresh(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Hooked guild.Refresh with null of activeCharacter");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			OutPacket op = new OutPacket(40);
			op.WriteInt(40);
			op.WriteShort(5);
			op.WriteShort(0x41);
			op.WriteInt(1);
			op.WriteShort(13413);
			c.WriteRawPacket(op.ToArray());

			op = new OutPacket(32);
			op.WriteInt(32);
			op.WriteShort(4);
			op.WriteShort(97);
			op.WriteInt(1);
			op.WriteShort(-15349);
			op.WriteShort((short)chr.getuID());
			op.WriteLong();
			op.WriteLong();
			c.WriteRawPacket(op.ToArray());

			op = new OutPacket(136);
			op.WriteInt(136);
			op.WriteShort(4);
			op.WriteShort(81);
			op.WriteInt(1);
			op.WriteShort(-15349);
			op.WriteShort((short)chr.getuID());
			op.WriteShort(1);
			op.WriteShort(30726);
			op.WriteString("PolishPoverty");
			c.WriteRawPacket(op.ToArray());

			op = new OutPacket(20);
			op.WriteInt(20);
			op.WriteInt(5);
			op.WriteInt(937683714); // those values.. lelellele
			op.WriteInt(680);
			op.WriteInt(939117056);
			c.WriteRawPacket(op.ToArray());
		}
	}
}

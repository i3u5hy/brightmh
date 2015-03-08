using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gameServer.Game;
using gameServer.Core.IO;

namespace gameServer.Game.Objects
{
	public class Guild
	{
		public byte guildType, guildHat;
		public short guildIcon;
		public int guildID, guildFame, guildGold, guildKills, guildDeaths;
		public string guildName, guildNews;
		public List<GuildMember> guildMembers = new List<GuildMember>(50);

		public Guild()
		{

		}

		public Guild(byte guildType, string guildName)
		{
			this.guildType = guildType;
			this.guildName = guildName;
		}

		public void Delete()
		{

		}

		public void sendToGuildMembers(byte[] packet)
		{
			foreach(GuildMember i in guildMembers)
			{
				Character chr = i.getOnlineCharacter();
				if(chr != null)
					chr.getAccount().mClient.WriteRawPacket(packet);
			}
		}

		public GuildMember findGuildMember(int uID)
		{
			GuildMember gM = guildMembers.Find(x => x.getuID() == uID);
			return gM != null ? gM : null;
		}
	}
}

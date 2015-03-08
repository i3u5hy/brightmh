using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gameServer.Game.Objects
{
	public class GuildMember
	{
		private byte guildRank;
		private int uID, donatedFame, donatedGold, contribution;
		private Character chr;

		public void setOnlineCharacter(Character chr)
		{
			this.chr = chr;
		}

		public Character getOnlineCharacter()
		{
			return this.chr;
		}

		public int getuID()
		{
			return uID;
		}

		public byte getGuildRank()
		{
			return this.guildRank;
		}
	}
}

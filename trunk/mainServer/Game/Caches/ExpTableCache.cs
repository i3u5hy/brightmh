using System.Collections.Generic;

namespace gameServer.Game.Caches
{
	public class ExpTableCache
	{
		private static readonly ExpTableCache instance = new ExpTableCache();
		private ExpTableCache()
		{
		}

		public static ExpTableCache Instance
		{
			get
			{
				return instance;
			}
		}

		public Dictionary<byte, int> expTable = new Dictionary<byte, int>();
		public Dictionary<byte, int> multipzs = new Dictionary<byte, int>();

		public long getExpRequirement(byte level)
		{
			return this.expTable[level] * this.multipzs[level];
		}

		public int getExpRequirementInt(byte level)
		{
			return this.expTable[level];
		}

		public int getMultiplifier(byte level)
		{
			return multipzs[level];
		}

		public int getSafeMultiplifier(byte level)
		{
			return multipzs[level] == 0 ? 1 : multipzs[level];
		}

		public void addExpRequirements(byte level, int exp, int multiplifier)
		{
			multipzs.Add(level, multiplifier);
			expTable.Add(level, exp);
			return;
		}
	}
}

using System.Collections.Generic;

namespace gameServer.Game.Misc
{
	class FameNickNames
	{
		private static readonly FameNickNames instance = new FameNickNames();
		private FameNickNames() { }

		public static FameNickNames Instance
		{
			get
			{
				return instance;
			}
		}

		public Dictionary<byte, int> NickNames = new Dictionary<byte, int>();

		public void addFameRequirements(byte slot, int fame)
		{
			NickNames.Add(slot, fame);
			return;
		}

		public byte getFameNickID(int fame)
		{
			foreach(KeyValuePair<byte, int> entry in NickNames)
			{
				if(fame > entry.Value)
					return entry.Key;
			}
			return 0;
		}
	}
}

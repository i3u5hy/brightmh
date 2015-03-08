using System.Collections.Generic;

namespace gameServer.Game.Misc {
	public sealed class AngerLevel
	{
		private static readonly AngerLevel instance = new AngerLevel();
		private AngerLevel() {
		}

		public static AngerLevel Instance
		{
			get
			{
				return instance;
			}
		}

		public Dictionary<byte, short[]> fury = new Dictionary<byte, short[]>();

		public void addAnger(byte level, short power, short time) {
			fury.Add(level, new short[] { power, time });
			return;
		}
	}
}

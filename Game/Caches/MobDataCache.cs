using System.Collections.Generic;
using gameServer.Game.World;

namespace gameServer.Game.Caches
{
	public class MobDataCache
	{
		private static readonly MobDataCache instance = new MobDataCache();
		private MobDataCache()
		{
		}

		public static MobDataCache Instance
		{
			get
			{
				return instance;
			}
		}

		private Dictionary<short, MobData> MobData = new Dictionary<short, MobData>();

		public void addMobData(MobData MobClass)
		{
			MobData.Add(MobClass.getmID(), MobClass);
		}

		public MobData getMobData(short key)
		{
			if(MobData.ContainsKey(key))
				return MobData[key];
			return null;
		}
	}
}

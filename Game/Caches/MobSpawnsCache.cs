using System.Collections.Generic;
using gameServer.Game.World;

namespace gameServer.Game.Caches
{
	public class MobSpawnsCache
	{
		private static readonly MobSpawnsCache instance = new MobSpawnsCache();
		private MobSpawnsCache() { }

		public static MobSpawnsCache Instance
		{
			get
			{
				return instance;
			}
		}

		public List<MobSpawnData> mobSpawnData = new List<MobSpawnData>();

		public void addToList(MobSpawnData sData)
		{
			MobSpawnData found = this.mobSpawnData.Find(s => s.getMobID() == sData.getMobID() && s.getX1() == sData.getX1() && s.getY1() == sData.getY1());
			if(found == null)
				this.mobSpawnData.Add(sData);
			else
				found.raiseCount();
		}
	}
}

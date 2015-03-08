using System.Collections.Generic;
using gameServer.Game.Objects;

namespace gameServer.Game.Caches
{
	public class ManualDataCache
	{
		private static readonly ManualDataCache instance = new ManualDataCache();
		private ManualDataCache() { }
		public static ManualDataCache Instance { get { return instance; } }

		private Dictionary<int, ManualData> manualData = new Dictionary<int, ManualData>();

		public void addManualData(int id, ManualData manualClass)
		{
			manualData.Add(id, manualClass);
		}

		public ManualData getManualData(int key)
		{
			if(manualData.ContainsKey(key))
				return manualData[key];
			return null;
		}
	}
}

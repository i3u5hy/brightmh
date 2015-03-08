using System.Collections.Generic;
using gameServer.Game.World;

namespace gameServer.Game.Caches {
	public class NPCDataCache {
		private static readonly NPCDataCache instance = new NPCDataCache();
		private NPCDataCache() {
		}

		public static NPCDataCache Instance
		{
			get
			{
				return instance;
			}
		}

		private Dictionary<int, NPCData> npcData = new Dictionary<int, NPCData>();

		public void addNPCData(NPCData npcClass) {
			npcData.Add(npcClass.getmID(), npcClass);
		}

		public NPCData getNPCData(int key) {
			if(!npcData.ContainsKey(key))
				return null;
			return npcData[key];
		}

		public NPCData getNPCDataByuID(int globalNPCuniqueID, short map) {
			List<NPC> npcs = WMap.Instance.getNpcs();
			NPC found = npcs.Find(x => x.getMap() == map && x.getuID() == globalNPCuniqueID);
			if(found == null)
				return null;
			return npcData[found.getmID()];
		}
	}
}

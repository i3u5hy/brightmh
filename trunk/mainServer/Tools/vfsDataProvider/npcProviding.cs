using System;
using System.IO;
using gameServer.Game.Caches;
using gameServer.Game.World;

namespace gameServer.Tools.vfsDataProvider {
	class npcProviding {
		public static void loadNPCs() {
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\script\\npcs.scr", "data\\npcs\\npcs.scr");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/npcs/npcs.scr"))
				return;

			int npc_counts = 0;

			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/npcs/npcs.scr");

			int u = 0;
			while(u * 1676 < data.Length) {
				NPCData npcData = new NPCData(data[u * 1676] + data[u * 1676 + 1] * 256);

				if(npcData == null) {
					u++;
					continue;
				}

				string npcName = null;
				for(int x = 2;x < 52;x++) {
					char _byte = Convert.ToChar(data[x + u * 1676]);
					if(_byte == 0x22) continue;
					if(_byte < 0x20) break;
					if(_byte == '\\') npcName += '\\';
					npcName += _byte;
				}

				for(byte x = 0;x < 60;x++) {
					int _itemID = data[(u * 1676) + 128 + (12 * x) + 0] + data[(u * 1676) + 128 + (12 * x) + 1] * 256 + data[(u * 1676) + 128 + (12 * x) + 2] * 65536 + data[(u * 1676) + 128 + (12 * x) + 3] * 16777216;
					if(_itemID < 200000000 || _itemID > 299999999) continue;
					npcData.addToItems(_itemID);
				}

				npcData.setName(npcName);
				npcData.setModule(data[u * 1676 + 118]);
				npc_counts++;
				NPCDataCache.Instance.addNPCData(npcData);

				u++;
			}

			Logger.WriteLog(Logger.LogTypes.Info, "Loaded data for {0} npcs", npc_counts);
		}
	}
}

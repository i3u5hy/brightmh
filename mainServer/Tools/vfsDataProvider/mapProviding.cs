using System;
using System.Collections.Generic;
using System.IO;
using gameServer.Game;
using gameServer.Game.Caches;
using gameServer.Game.World;
using gameServer.Servers;

namespace gameServer.Tools.vfsDataProvider {
	class mapProviding {
		public static bool loadMaps() {
			List<string> maps = new List<string>();
			int createdGrids = 0;
			int createdAreas = 0;
			int createdNPCs = 0;
			int createdAreaTriggers = 0;
			int createdMobs = 0;
			int createdTowns = 0;

			if(!Constants.VFSSkip)
			{
				if(vfsDataProvider.Instance.fileNames.Count > 0)
				{
					foreach(string namez in vfsDataProvider.Instance.fileNames)
					{
						if(namez.Length < 12)
							continue;
						if(namez.Substring(0, 8) == "data\\map")
						{
							int mapDirID;
							string mapDirIDs = namez.Substring(8, 3);
							if(int.TryParse(mapDirIDs, out mapDirID) == true)
							{
								if(maps.Contains(mapDirIDs))
									continue;
								maps.Add(mapDirIDs);
								createdGrids++;
								createdAreas += loadMap(mapDirIDs);
								createdNPCs += loadNPCs(mapDirIDs);
								createdAreaTriggers += loadAreaTriggers(mapDirIDs);
								createdMobs += loadMobSpawns(mapDirIDs);
								createdTowns += loadTowns(mapDirIDs);
							}
						}
					}
				}
			}
			else
			{
				for(int i = 0;i <= 301;i++)
				{
					if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/maps/map" + i.ToString("D3") + ".bin"))
					{
						continue;
					}
					createdGrids++;
					createdAreas += loadMap(i.ToString("D3"));
					createdNPCs += loadNPCs(i.ToString("D3"));
					createdAreaTriggers += loadAreaTriggers(i.ToString("D3"));
					createdMobs += loadMobSpawns(i.ToString("D3"));
					createdTowns += loadTowns(i.ToString("D3"));
				}
			}

			if(createdGrids == 0)
			{
				return false;
			}
			Logger.WriteLog(Logger.LogTypes.Info, "Created {0} grids: {1} areas {2} npcs {3} areaTs {4} mobs {5} towns", createdGrids, createdAreas, createdNPCs, createdAreaTriggers, createdMobs, createdTowns);
			return true;
		}

		public static int loadMap(string mapIdentifier) {
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\map" + mapIdentifier + "\\region" + mapIdentifier + ".bin", "data\\regions\\region" + mapIdentifier + ".bin");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/regions/region" + mapIdentifier + ".bin"))
				return 0;

			int mapID = Convert.ToInt32(mapIdentifier);
			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/regions/region" + mapIdentifier + ".bin");
			int x = BitConverter.ToInt32(data, 0);
			int y = BitConverter.ToInt32(data, 4);
			int fxPos = BitConverter.ToInt32(data, 8 + x * y);
			int fyPos = BitConverter.ToInt32(data, 8 + x * y + 4);

			Grid grid = new Grid();
			grid.setgID(mapID);
			grid.setgSize(new int[] { x, y });
			grid.setaSize(256);
			grid.setStartingGridPosition(new float[] { fxPos, fyPos });
			grid.setEndingGridPosition(new float[] { fxPos + (grid.getgSize()[0] * grid.getaSize()), fyPos + (grid.getgSize()[1] * grid.getaSize()) });
			WMap.Instance.addGrid(grid);

			int position = 8;
			for(int i = 0;i < y;i++) {
				for(int u = 0;u < x;u++) {
					WMap.Instance.getGrid(mapID).getArea((u * ((x > y) ? (x) : (y))) + i).setRegionID((byte)data[position]);
					position++;
				}
			}

			return x * y;
		}

		public static int loadNPCs(string mapIdentifier) {
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\map" + mapIdentifier + "\\npc" + mapIdentifier + ".arr", "data\\npcs\\npc" + mapIdentifier + ".arr");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/npcs/npc" + mapIdentifier + ".arr"))
				return 0;

			int npc_counts = 0;

			int mapID = Convert.ToInt32(mapIdentifier);
			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/npcs/npc" + mapIdentifier + ".arr");

			if(data.Length < 20)
				return 0;

			int u = 0;
			int npc_data_length = 0;
			if(mapIdentifier != "207")
				npc_data_length = 28;
			else
				npc_data_length = 20;
			while(u * npc_data_length < data.Length) {
				int npcmID = BitConverter.ToInt16(data, u * npc_data_length);
				float x = BitConverter.ToSingle(data, u * npc_data_length + 4);
				float y = BitConverter.ToSingle(data, u * npc_data_length + 8);

				NPC npc = new NPC(npcmID, u+1);
				npc.setMap((short)mapID);
				npc.setPosition(new float[] { x, y });
				Area area = WMap.Instance.getGrid(mapID).getAreaByRound(x, y);

				if(area != null) {
					WMap.Instance.addToNPCs(npc);
					area.addNPC(npc);
					npc_counts++;
				}

				u++;
			}

			return npc_counts;
		}

		public static int loadMobSpawns(string mapIdentifier)
		{
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\map" + mapIdentifier + "\\mob" + mapIdentifier + ".arr", "data\\mobs\\mob" + mapIdentifier + ".arr");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/mobs/mob" + mapIdentifier + ".arr") && !File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/mobs/mob" + mapIdentifier + ".mre"))
				return 0;

			int mob_counts = 0;

			short mapID = Convert.ToInt16(mapIdentifier);

			List<spawnData> writeStorage = new List<spawnData>();
			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/mobs/mob" + mapIdentifier + ".arr");

			int position = 0;
			while(position < data.Length)
			{
				addToList(writeStorage, new spawnData((short)(data[position] + data[position + 1] * 256), mapID,
					BitConverter.ToSingle(data, position + 4), BitConverter.ToSingle(data, position + 8),
					BitConverter.ToSingle(data, position + 12), BitConverter.ToSingle(data, position + 16)));
				position += 20;
			}

			int pool=0;
			foreach(spawnData i in writeStorage)
			{
				new MobController(i.mobID, i.count, pool, i.map, (int)i.X1, (int)i.Y1, (int)i.X2, (int)i.Y2);
				pool++;
				mob_counts++;
			}
			return mob_counts;
		}

		private static void addToList(List<spawnData> liste, spawnData sData)
		{
			spawnData found = liste.Find(s => s.mobID == sData.mobID && s.X1 == sData.X1 && s.Y1 == sData.Y1);
			if(found == null)
				liste.Add(sData);
			else
				found.count++;
		}

		private class spawnData
		{
			public short mobID
			{
				get;
				set;
			}
			public short map
			{
				get;
				set;
			}
			public float X1
			{
				get;
				set;
			}
			public float X2
			{
				get;
				set;
			}
			public float Y1
			{
				get;
				set;
			}
			public float Y2
			{
				get;
				set;
			}
			public float RX
			{
				get;
				set;
			}
			public float RY
			{
				get;
				set;
			}
			public int count
			{
				get;
				set;
			}
			public float radius
			{
				get;
				set;
			}

			public spawnData(short id, short map, float x1, float y1, float x2, float y2)
			{
				this.mobID = id;
				this.map = map;
				this.X1 = x1;
				this.Y1 = y1;
				this.X2 = x2;
				this.Y2 = y2;
				this.count = 1;
				this.RX = X1 + X2 / 2;
				this.RY = Y1 + Y2 / 2;
				this.radius = Math.Min(Math.Abs(X2), Math.Abs(Y2));
			}
		}

		public static int loadAreaTriggers(string mapIdentifier) {
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\map" + mapIdentifier + "\\map" + mapIdentifier + ".bin", "data\\maps\\map" + mapIdentifier + ".bin");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/maps/map" + mapIdentifier + ".bin"))
				return 0;

			int areaTrigger_counts = 0;

			int mapID = Convert.ToInt32(mapIdentifier);
			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/maps/map" + mapIdentifier + ".bin");

			for(int i = 0;i < 12;i++) {
				if(BitConverter.ToSingle(data, 8 + (i * 8)) == 0 && 0 == BitConverter.ToSingle(data, 12 + (i * 8)))
					continue;
				AreaTrigger areaTrigger = new AreaTrigger();
				areaTrigger.setFromPosition(new float[] { BitConverter.ToSingle(data, 8 + (i * 8)), BitConverter.ToSingle(data, 12 + (i * 8)) });
				areaTrigger.setfMap((short)mapID);
				areaTrigger.setToPosition(new float[] { BitConverter.ToSingle(data, 200 + (i * 8)), BitConverter.ToSingle(data, 204 + (i * 8)) });
				areaTrigger.settMap(BitConverter.ToInt16(data, 136 + (i * 4)));
				areaTrigger.setRequiredItem(
					(areaTrigger.getFromPosition()[0] == -1567 && areaTrigger.getFromPosition()[1] == 1991 && areaTrigger.gettMap() == 3) ? (213062201) :
					(areaTrigger.getFromPosition()[0] == -1634 && areaTrigger.getFromPosition()[1] == 3050 && areaTrigger.gettMap() == 3) ? (213062200) :
					(0)); // damn koreanz
				WMap.Instance.getGrid(mapID).getAreaByRound(areaTrigger.getFromPosition()[0], areaTrigger.getFromPosition()[1]).addAreaTrigger(areaTrigger);
				areaTrigger_counts++;
			}

			return areaTrigger_counts;
		}

		public static int loadTowns(string mapIdentifier)
		{
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\map" + mapIdentifier + "\\regiontable" + mapIdentifier + ".bin", "data\\maps\\regiontable" + mapIdentifier + ".bin");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/maps/regiontable" + mapIdentifier + ".bin"))
				return 0;

			int town_counts = 0;

			short mapID = Convert.ToInt16(mapIdentifier);
			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/maps/regiontable" + mapIdentifier + ".bin");
			int position = 32;
			List<Waypoint> townz = new List<Waypoint>();
			while(position < data.Length)
			{
				if(BitConverter.ToSingle(data, position) == 0.0)
				{
					position += 48;
					continue;
				}

				if(data[position + 1] == 0x0 || data[position+2] == 0x0)
				{
					position += 48;
					continue;
				}

				Waypoint waypoint = new Waypoint(BitConverter.ToSingle(data, position), BitConverter.ToSingle(data, position + 4));

				Waypoint found = townz.Find(s => s.getX() == waypoint.getX() && s.getY() == waypoint.getY());
				if(found != null)
				{
					position+=48;
					continue;
				}

				if(townz.Contains(waypoint))
				{
					position += 48;
					continue;
				}

				town_counts++;
				townz.Add(waypoint);
				position += 48;
			}

			TownCoordsCache.Instance.addTowns(mapID, townz);
			return town_counts;
		}
	}
}
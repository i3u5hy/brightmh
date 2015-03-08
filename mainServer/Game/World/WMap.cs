using System;
using System.Collections.Generic;
using gameServer.Game.Objects;

namespace gameServer.Game.World
{
	public sealed class WMap {
		private static readonly WMap instance = new WMap();
		private WMap() { }
		public static WMap Instance { get { return instance; } }

		public int mobsCount = 0;
		private Dictionary<int, Grid> grids = new Dictionary<int, Grid>();
		private List<NPC> npcs = new List<NPC>();
		private List<Area> synchronizedAreas = new List<Area>();
		private List<Character> worldCharacters = new List<Character>();
		public Dictionary<int, Item> items = new Dictionary<int, Item>();
		private List<Character> vendingList = new List<Character>();
		private List<Guild> guildList = new List<Guild>();

		public Guild findGuildID(int gID)
		{
			Guild guild = guildList.Find(x => x.guildID == gID);
			return guild != null ? guild : null;
		}

		public void addGuild(Guild guild)
		{
			guildList.Add(guild);
		}

		public void removeGuild(Guild guild)
		{
			guildList.Remove(guild);
		}

		public List<Area> getSynchronizedAreas()
		{
			return this.synchronizedAreas;
		}

		public List<Character> getVendingList()
		{
			return vendingList;
		}

		public void addVendorList(Character ch)
		{
			vendingList.Add(ch);
		}

		public Boolean removeVendorList(Character ch)
		{
			return vendingList.Remove(ch);
		}

		public void sendToAllCharactersExcept(Character chr, byte[] packet)
		{
			foreach(Character i in worldCharacters)
			{
				if(i == chr)
					continue;
				i.getAccount().mClient.WriteRawPacket(packet);
			}
		}

		public void addToSynchronizedAreas(Area area)
		{
			if(synchronizedAreas.Contains(area))
				return;
			if(area.getMobs().Count == 0)
				return;
			synchronizedAreas.Add(area);
			//MobThreadPool.run();
		}

		public void removeFromSynchronizedAreas(Area area)
		{
			if(!synchronizedAreas.Contains(area))
				return;
			if(area.getCharacters().Count > 0)
				return;
			synchronizedAreas.Remove(area);
			if(synchronizedAreas.Count == 0)
			{
				// stop thread
			}
		}

		public void addGrid(Grid g){
			g.initGrid();
			this.grids.Add(g.getgID(), g);
		}
	
		public void removeGrid(Grid g) {
			if(!this.grids.ContainsKey(g.getgID())) return;
			this.grids.Remove(g.getgID());
		}
	
		public Boolean gridExists(int gID) {
			return this.grids.ContainsKey(gID);
		}
	
		public Grid getGrid(int gID) {
			if(!this.grids.ContainsKey(gID)) return null;
			return this.grids[gID];
		}
	
		public static float distance(float a, float b)
		{
			return (float)Math.Sqrt(Math.Pow((double)(a - b), 2));
		}

		public static float distance(float tx, float ty, float dx, float dy)
		{
			return (float)Math.Sqrt(Math.Pow((double)(tx - dx), (double)2) + Math.Pow((double)(ty - dy), (double)2));	  
		}
	
		public static int distance(int a, int b)
		{
			return (int)Math.Sqrt(Math.Pow((double)(a - b), 2));
		}

		public List<NPC> getNPCsOfId(int npcID) {
			List<NPC> _npcList = new List<NPC>();
			foreach(NPC _npc in this.npcs) {
				if(_npc.getmID() != npcID) continue;
				_npcList.Add(_npc);
			}
			return _npcList;
		}

		public List<NPC> getNpcs() {
			return npcs;
		}

		public void setNpcs(List<NPC> npcs) {
			this.npcs = npcs;
		}

		public void addToNPCs(NPC npc) {
			this.npcs.Add(npc);
		}

		public List<Character> getWorldCharacters() {
			return this.worldCharacters;
		}

		public void setCharacters(List<Character> characters) {
			this.worldCharacters = characters;
		}
	
		public Boolean removeFromCharacters(Character character) {
			if(this.worldCharacters.Contains(character)) {
				this.worldCharacters.Remove(character);
				return true;
			}
			if(worldCharacters.Count == 0)
			{
				Timers.HealTimer.mainStopper();
			}
			return false;
		}
	
		public Boolean addToCharacters(Character character) {
			if(this.worldCharacters.Contains(character))
				return false;
			this.worldCharacters.Add(character);
			Timers.HealTimer.run();
			return true;
		}

		public Character findPlayerByName(string name)
		{
			foreach(Character chara in this.worldCharacters)
			{
				if(chara.getName() == name)
					return chara;
			}
			return null;
		}

		public Character findPlayerByuID(int uID)
		{
			foreach(Character chara in this.worldCharacters)
			{
				if(chara.getuID() == uID)
					return chara;
			}
			return null;
		}
	}
}

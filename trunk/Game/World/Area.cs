using System;
using System.Collections.Generic;
using gameServer.Packets;

namespace gameServer.Game.World
{
	public class Area
	{
		private int aID;
		private int[] areaPosition;
		private Grid grid;
		private byte regionID;
		private float[] startingAreaPosition;
		private float[] endingAreaPosition;
		private List<AreaTrigger> areaTriggers = new List<AreaTrigger>();
		private List<Character> areaCharacters = new List<Character>();
		private List<NPC> npcs = new List<NPC>();
		private List<Mob> mobs = new List<Mob>();

		public Area(int aID, int[] areaPosition, Grid grid)
		{
			this.aID = aID;
			this.areaPosition = areaPosition;
			this.grid = grid;
			this.startingAreaPosition = new float[] { grid.getStartingGridPosition()[0] + (grid.getaSize() * areaPosition[0]),
				grid.getStartingGridPosition()[1] + (grid.getaSize() * areaPosition[1]) };
			this.endingAreaPosition = new float[] { grid.getStartingGridPosition()[0] + ((grid.getaSize() * areaPosition[0]+1)-1),
				grid.getStartingGridPosition()[1] + ((grid.getaSize() * areaPosition[1]+1)-1) };
		}

		public float[] getAreaStartingPosition() {
			return this.startingAreaPosition;
		}

		public void sendAreaInit(Character character) {
			MartialClient connection = character.getAccount().mClient;
            if (character.getInnitedAreas().Contains(this.aID)) return;
			foreach(Character aCharacter in areaCharacters) {
				if(aCharacter == character) continue;
				try {
					Console.WriteLine("AreaInt> Packet sent for {0} from {1}", character.getName(), aCharacter.getName());
					connection.WriteRawPacket(CharacterPackets.extCharPacket(aCharacter));
				} catch(Exception e) { Console.WriteLine(e); }
			}
			foreach (NPC aNPC in npcs) {
				try {
					connection.WriteRawPacket(aNPC.npcSpawn(character));
				} catch (Exception e) { Console.WriteLine(e); }
			}
			foreach(Mob aMob in mobs) {
				try {
					connection.WriteRawPacket(aMob.getInitialPacket());
				} catch(Exception e) { Console.WriteLine(e); }
			}
		}

		public int getaID()
		{
			return this.aID;
		}

		public void setaID(int aID)
		{
			this.aID = aID;
		}

		public int[] getAreaPosition()
		{
			return this.areaPosition;
		}

		public void setAreaPosition(int[] areaPosition)
		{
			this.areaPosition = areaPosition;
		}

		public Grid getGrid()
		{
			return this.grid;
		}

		public void setGrid(Grid grid)
		{
			this.grid = grid;
		}

		public List<AreaTrigger> getAreaTriggers()
		{
			return this.areaTriggers;
		}

		public void setAreaTriggers(List<AreaTrigger> areaTriggers)
		{
			this.areaTriggers = areaTriggers;
		}

		public void addAreaTrigger(AreaTrigger areaTrigger)
		{
			this.areaTriggers.Add(areaTrigger);
		}

		public List<NPC> getNpcs()
		{
			return npcs;
		}

		public void setNpcs(List<NPC> npcs)
		{
			this.npcs = npcs;
		}

		public void addNPC(NPC npc)
		{
			if(this.npcs.Contains(npc))
				return;
			this.npcs.Add(npc);
		}

		public void addMob(Mob mob)
		{
			if(this.mobs.Contains(mob))
				return;
			this.mobs.Add(mob);
		}

		public byte getRegionID()
		{
			return this.regionID;
		}

		public void setRegionID(byte regionID)
		{
			this.regionID = regionID;
		}

		public List<Mob> getMobs()
		{
			return this.mobs;
		}

		public List<Character> getCharacters()
		{
			return this.areaCharacters;
		}

		public void setCharacters(List<Character> characters)
		{
			this.areaCharacters = characters;
		}

		public void addCharacter(Character character)
		{
			if (this.areaCharacters.Contains(character)) return;
				this.areaCharacters.Add(character);
		}

		public Mob findMobByuID(int uID)
		{
			foreach(Mob i in mobs)
			{
				if(i.getuID() == uID)
					return i;
			}
			return null;
		}

		public void removeCharacter(Character character)
		{
			if(!this.areaCharacters.Contains(character))
				return;
			this.areaCharacters.Remove(character);
			WMap.Instance.removeFromSynchronizedAreas(this);
		}
	}
}

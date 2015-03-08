using System;
using System.Collections.Generic;
using gameServer.Packets;
using gameServer.Threads;

namespace gameServer.Game.World
{
	public class Grid {
		private int gID;
		private int[] gSize;

		private int aSize;
		private float[] startingGridPosition;
		private float[] endingGridPosition;

		private MobThreadPool gridPool;

		private Dictionary<int, Area> areas = new Dictionary<int, Area>();
		private List<Character> vendings = new List<Character>();
	
		public Grid() {
			
		}

		public void addToVendings(Character vending)
		{
			if(!vendings.Contains(vending))
				vendings.Add(vending);
		}

		public void removeFromVendings(Character vending)
		{
			if(vendings.Contains(vending))
				vendings.Remove(vending);
		}

		public List<Character> getVendings()
		{
			return this.vendings;
		}

		public void initMobThreadPool()
		{
			//gridPool = new MobThreadPool(2000);
		}

		public MobThreadPool getThreadPool()
		{
			return this.gridPool;
		}

		public void initGrid() {
			for(int i = 0;i < this.gSize[1];i++) {
				for(int u = 0;u < this.gSize[0];u++) {
					Area area = new Area((u * this.getrGSize()) + i, new int[] { u, i }, this);
					this.addArea(area);
				}
			}
		}

		private void addArea(Area a) {
			if(this.areaExists(a.getaID())) return;
			this.areas.Add(a.getaID(), a);
		}

		public Boolean areaExists(Area area) {
			return this.areas.ContainsValue(area);
		}

		public Boolean areaExists(int aID) {
			return this.areas.ContainsKey(aID);
		}

		public Area getArea(int aID) {
			if(!areaExists(aID))
				return null;
			return this.areas[aID];
		}

		public Area getArea(int[] coords) {
			int calculate = (coords[0] * this.getrGSize()) + coords[1];
			if(!areaExists(calculate))
				return null;
			return this.areas[calculate];
		}
	
		public Area getAreaByRound(float tx, float ty){
			int[] ret = new int[]{-1,-1};
			if (tx >= this.getStartingGridPosition()[0] && tx <= this.getEndingGridPosition()[0] && ty >= this.getStartingGridPosition()[1] && ty <= this.getEndingGridPosition()[1]){
				float gx = WMap.distance(this.getStartingGridPosition()[0], tx);
				float gy = WMap.distance(this.getStartingGridPosition()[1], ty);

				ret[0] = (int)(Math.Floor(gx / this.getaSize()));
				ret[1] = (int)(Math.Floor(gy / this.getaSize()));
				return this.getArea(ret);
			}
			return null;
		}

		public void sendTo3x3Area(Area area, byte[] packet)
		{
			if(!this.areaExists(area)) return;
			for(int i = 0;i < 3;i++) {
				for(int u = 0;u < 3;u++) {
					Area nearCentral = getArea(new int[] { area.getAreaPosition()[0] - 1 + i, area.getAreaPosition()[1] - 1 + u });
					if(nearCentral == null) continue;
					foreach(Character characterAround in nearCentral.getCharacters()) {
						try {
							characterAround.getAccount().mClient.WriteRawPacket(packet);
						}
						catch(Exception e) {
							Console.WriteLine(e);
						}
					}
				}
			}
		}

		public void sendTo3x3AreaRemoveItem(Area area, int uID)
		{
			for(int i = 0;i < 3;i++)
			{
				for(int u = 0;u < 3;u++)
				{
					Area nearCentral = getArea(new int[] { area.getAreaPosition()[0] - 1 + i, area.getAreaPosition()[1] - 1 + u });
					if(nearCentral == null)
						continue;
					foreach(Character characterAround in nearCentral.getCharacters())
					{
						try
						{
							characterAround.getAccount().mClient.WriteRawPacket(ItemPackets.removeDroppedItemForCharacter(characterAround, uID));
						}
						catch(Exception e)
						{
							Console.WriteLine(e);
						}
					}
				}
			}
		}

		public void sendTo3x3Area(Character character, Area area, byte[] packet)
		{
			if(!this.areaExists(area)) return;
			for (int i =0; i < 3; i++) {
				for (int u =0; u < 3; u++) {
					Area nearCentral = getArea(new int[] { area.getAreaPosition()[0]-1+i, area.getAreaPosition()[1]-1+u });
					if(nearCentral == null) continue;
					foreach(Character characterAround in nearCentral.getCharacters()) {
						if(characterAround == character) continue;
						try {
							Console.WriteLine("Area> Packet sent for {0} from {1}", characterAround.getName(), character.getName());
							characterAround.getAccount().mClient.WriteRawPacket(packet);
						} catch (Exception e) { Console.WriteLine(e); }
					}
				}
			}
		}
	
		public void sendTo3x3AreaMovement(Character character, Area area, byte[] packet) {
			if(!this.areaExists(area)) return;
			List<int> relistInnitedAreas = new List<int>();
			for (int i =0; i < 3; i++) {
				for (int u =0; u < 3; u++) {
					Area nearCentral = getArea(new int[] { area.getAreaPosition()[0]-1+i, area.getAreaPosition()[1]-1+u });
					if(nearCentral == null) continue;
					foreach(Character characterAround in nearCentral.getCharacters()) {
						if(characterAround == character)
							continue;
						if(!character.getInnitedAreas().Contains(nearCentral.getaID()))
						{
							characterAround.getAccount().mClient.WriteRawPacket(CharacterPackets.extCharPacket(character));
						}
						try {
							Console.WriteLine("AreaMove> Packet sent for {0} from {1}", characterAround.getName(), character.getName());
							characterAround.getAccount().mClient.WriteRawPacket(packet);
						}
						catch(Exception e) {
							Console.WriteLine(e);
						}
					}
					if(character.getInnitedAreas().Contains(nearCentral.getaID()))
					{
						relistInnitedAreas.Add(nearCentral.getaID());
						continue;
					}
					nearCentral.sendAreaInit(character);
					relistInnitedAreas.Add(nearCentral.getaID());
				}
			}
			character.removeInnitedAreas();
			character.setInnitedAreas(relistInnitedAreas);
		}
	
		public void sendTo3x3AreaSpawn(Character character, Area area, Boolean just_a_refresh_for_the_world = false) {
			if(!this.areaExists(area)) return;
			character.removeInnitedAreas();
			for (int i =0; i < 3; i++) {
				for (int u =0; u < 3; u++) {
					Area nearCentral = getArea(new int[] { area.getAreaPosition()[0]-1+i, area.getAreaPosition()[1]-1+u });
					if(nearCentral == null) continue;
                    if(!just_a_refresh_for_the_world) nearCentral.sendAreaInit(character);
					foreach(Character characterAround in nearCentral.getCharacters()) {
						if(characterAround == character) continue;
						try {
							Console.WriteLine("AreaSpawn> extCharPacket from: {0} for {1}", character.getName(), characterAround.getName());
							characterAround.getAccount().mClient.WriteRawPacket(CharacterPackets.extCharPacket(character));
						}
						catch(Exception e) {
							Console.WriteLine(e);
						}
					}
					character.addInnitedArea(nearCentral.getaID());
					WMap.Instance.addToSynchronizedAreas(area);
				}
			}
		}

		public Fightable getFightableNear(Area area, int uID)
		{
			for(int i = 0;i < 3;i++)
			{
				for(int u = 0;u < 3;u++)
				{
					Area nearCentral = getArea(new int[] { area.getAreaPosition()[0] - 1 + i, area.getAreaPosition()[1] - 1 + u });
					if(nearCentral == null)
						continue;
					foreach(Fightable f in nearCentral.getMobs())
					{
						if(f.getuID() == uID)
							return f;
					}
					foreach(Fightable f in nearCentral.getCharacters())
					{
						if(f.getuID() == uID)
							return f;
					}
				}
			}
			return null;
		}

		public void sendTo3x3AreaLeave(Character character, Area area)
		{
			if(!this.areaExists(area)) return;
			for (int i =0; i < 3; i++) {
				for (int u =0; u < 3; u++) {
					Area nearCentral = getArea(new int[] { area.getAreaPosition()[0]-1+i, area.getAreaPosition()[1]-1+u });
					if(nearCentral == null) continue;
					foreach(Character characterAround in nearCentral.getCharacters()) {
						if(characterAround == character) continue;
						try {
							Console.WriteLine("AreaLeave> vanPacket from: {0} for {1}", character.getName(), characterAround.getName());
							characterAround.getAccount().mClient.WriteRawPacket(CharacterPackets.vanCharPacket(character));
						} catch (Exception e) { Console.WriteLine(e); }
					}
				}
			}
			character.getArea().removeCharacter(character);
			character.removeInnitedAreas();
			WMap.Instance.removeFromSynchronizedAreas(area);
		}

		public int getgID() {
			return gID;
		}

		public void setgID(int gID) {
			this.gID = gID;
		}

		public int[] getgSize() {
			return gSize;
		}

		public void setgSize(int[] gSize) {
			this.gSize = gSize;
		}

		public int getaSize() {
			return aSize;
		}

		public void setaSize(int aSize) {
			this.aSize = aSize;
		}

		public float[] getStartingGridPosition() {
			return startingGridPosition;
		}

		public void setStartingGridPosition(float[] gridPosition) {
			this.startingGridPosition = gridPosition;
		}
	
		public float[] getEndingGridPosition() {
			return endingGridPosition;
		}

		public void setEndingGridPosition(float[] endingGridPosition) {
			this.endingGridPosition = endingGridPosition;
		}
	
		public int getrGSize() {
			return (getgSize()[0] > getgSize()[1]) ? (getgSize()[0]) : (getgSize()[1]) ;
		}
	}
}

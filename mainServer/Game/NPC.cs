using System;
using gameServer.Core.IO;
using gameServer.Game.Caches;

namespace gameServer.Game.World
{
	public sealed class NPC
	{
		private int uID, mID;
		private short map;
		private float[] npcPosition;

		public NPC(int mID, int uID)
		{
			this.uID = uID;
			this.mID = mID;
		}

		public int getuID() {
			return this.uID;
		}

		public byte[] npcSpawn(Character character)
		{
			OutPacket initNPCData = new OutPacket(615);
			initNPCData.WriteInt(615);
			initNPCData.WriteShort(0x04);
			initNPCData.WriteShort(0x04);
			initNPCData.WriteByte(0x01);
			initNPCData.WriteInt(character.getuID());
			initNPCData.WriteInt(character.getArea().getaID());
			initNPCData.WriteFloat(character.getPosition()[0]);
			initNPCData.WriteFloat(character.getPosition()[1]);
			initNPCData.WriteByte(3);
			initNPCData.WriteInt(this.uID);
			initNPCData.WriteInt();
			initNPCData.WritePaddedString(this.getName(), 16);
			initNPCData.Skip(18);
			initNPCData.WriteInt(this.getModule());
			initNPCData.Skip(10);
			initNPCData.WriteInt(this.getmID());
			initNPCData.Skip(16);
			initNPCData.WriteFloat(this.npcPosition[0]);
			initNPCData.WriteFloat(this.npcPosition[1]);
			initNPCData.Skip(502);
			initNPCData.WriteByte(0x22);
			initNPCData.WriteByte(0x08);
			return initNPCData.ToArray();
		}

		public byte[] npcSpawnChained(Character chr)
		{
			OutPacket initNPCData = new OutPacket(589);
			initNPCData.WriteByte(3);
			initNPCData.WriteInt(this.uID);
			initNPCData.WriteInt();
			initNPCData.WritePaddedString(this.getName(), 16);
			initNPCData.Skip(18);
			initNPCData.WriteInt(this.getModule());
			initNPCData.Skip(10);
			initNPCData.WriteInt(this.getmID());
			initNPCData.Skip(16);
			initNPCData.WriteFloat(this.npcPosition[0]);
			initNPCData.WriteFloat(this.npcPosition[1]);
			return initNPCData.ToArray();
		}

		public int getmID() {
			return this.mID;
		}

		public int getModule() {
			return getNPCDataStreaming().getModule();
		}

		public short getMap()
		{
			return map;
		}

		public void setMap(short map)
		{
			this.map = map;
		}

		public float[] getPosition()
		{
			return this.npcPosition;
		}

		public void setPosition(float[] position)
		{
			this.npcPosition = position;
		}

		public String getName()
		{
			return getNPCDataStreaming().getName();
		}

		public NPCData getNPCDataStreaming()
		{
			return NPCDataCache.Instance.getNPCData(mID);
		}
	}
}
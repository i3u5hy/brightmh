
namespace gameServer.Game.World
{
	public class MobSpawnData
	{
		private int mobID;
		private short map;
		private float X1;
		private float X2;
		private float Y1;
		private float Y2;
		private byte count;

		public MobSpawnData(int id, short map, float x1, float y1, float x2, float y2)
		{
			this.mobID = id;
			this.map = map;
			this.X1 = x1;
			this.Y1 = y1;
			this.X2 = x2;
			this.Y2 = y2;
			this.count = 1;
		}

		public int getMobID()
		{
			return this.mobID;
		}

		public float getX1()
		{
			return this.X1;
		}

		public float getY1()
		{
			return this.Y1;
		}

		public void raiseCount()
		{
			this.count++;
		}
	}
}

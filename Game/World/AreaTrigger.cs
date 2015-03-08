
namespace gameServer.Game.World
{
	public class AreaTrigger
	{
		private int atID;
		private short fMap;
		private float[] fromPosition;
		private short tMap;
		private float[] toPosition;
		private int requiredItem = 0;

		public AreaTrigger()
		{

		}

		public int getatID()
		{
			return atID;
		}

		public void setatID(int atID)
		{
			this.atID = atID;
		}

		public short getfMap()
		{
			return fMap;
		}

		public void setfMap(short fMap)
		{
			this.fMap = fMap;
		}

		public float[] getFromPosition()
		{
			return fromPosition;
		}

		public void setFromPosition(float[] position)
		{
			this.fromPosition = position;
		}

		public float[] getToPosition()
		{
			return toPosition;
		}

		public void setToPosition(float[] position)
		{
			this.toPosition = position;
		}

		public short gettMap()
		{
			return tMap;
		}

		public void settMap(short tMap)
		{
			this.tMap = tMap;
		}

		public int getRequiredItem()
		{
			return requiredItem;
		}

		public void setRequiredItem(int requiredItem)
		{
			this.requiredItem = requiredItem;
		}
	}
}

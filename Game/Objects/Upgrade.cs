
namespace gameServer.Game.Objects
{
	public class Upgrade
	{
		private int id;
		private int oldID;
		private int upgrader;
		private int newit;
		private float itstage;
		private float upgradelvl;
		private float failrate;
		private float breakoption;
		private int upgradeskill;

		public Upgrade(int id, int oldID, int upgrader, int newIt, float itstage, float upgradelvl, float failrate, float breakoption, int upgradeskill)
		{
			this.id = id;
			this.oldID = oldID;
			this.upgrader = upgrader;
			this.newit = newIt;
			this.itstage = itstage;
			this.upgradelvl = upgradelvl;
			this.failrate = failrate;
			this.breakoption = breakoption;
			this.upgradeskill = upgradeskill;
		}

		public int getID()
		{
			return id;
		}

		public int getOldit()
		{
			return oldID;
		}

		public int getUpgrader()
		{
			return upgrader;
		}

		public int getNewit()
		{
			return newit;
		}

		public float getItstage()
		{
			return itstage;
		}

		public float getUpgradelvl()
		{
			return upgradelvl;
		}

		public float getFailrate()
		{
			return failrate;
		}

		public float getBreakoption()
		{
			return breakoption;
		}

		public int getUpgradeskill()
		{
			return upgradeskill;
		}
	}
}

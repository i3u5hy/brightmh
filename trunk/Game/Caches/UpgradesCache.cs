using System.Collections.Generic;
using gameServer.Game.Objects;

namespace gameServer.Game.Misc
{
	class Upgrades
	{
		private static readonly Upgrades instance = new Upgrades();
		private Upgrades()
		{
		}

		public static Upgrades Instance
		{
			get
			{
				return instance;
			}
		}

		public List<Upgrade> upgrades = new List<Upgrade>();

		public void addItemUpgrading(Upgrade classe)
		{
			upgrades.Add(classe);
			return;
		}

		public Upgrade getUpgradeClasse(int oldItemID, int upgraderID)
		{
			Upgrade found = upgrades.Find(s => s.getOldit() == oldItemID && s.getUpgrader() == upgraderID);
			return found;
		}
	}
}

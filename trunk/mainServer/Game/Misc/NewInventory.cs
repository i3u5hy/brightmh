using System.Collections.Generic;

namespace gameServer.Game.Misc
{
	public sealed class NewInventory
	{
		private static readonly NewInventory instance = new NewInventory();
		private NewInventory() { }
		public static NewInventory Instance
		{
			get
			{
				return instance;
			}
		}

		public List<int[]> newInventory = new List<int[]>();

		public void addItemTonInventory(int line, int row, int itemID, int quantity)
		{
			newInventory.Add(new int[] { line, row, itemID, quantity });
			return;
		}
	}
}

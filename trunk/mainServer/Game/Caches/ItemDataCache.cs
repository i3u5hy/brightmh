using System.Collections.Generic;
using gameServer.Game.Objects;

namespace gameServer.Game.Caches
{
	public class ItemDataCache
	{
		private static readonly ItemDataCache instance = new ItemDataCache();
		private ItemDataCache() { }
		public static ItemDataCache Instance { get { return instance; } }
		private Dictionary<int, ItemData> itemData = new Dictionary<int, ItemData>();

		public void addItemData(ItemData iData)
		{
			itemData.Add(iData.getID(), iData);
		}

		public ItemData getItemData(int key)
		{
			if(key < 200000000 || key > 299999999) return null;
			if(!itemData.ContainsKey(key)) return new ItemData();
			return itemData[key];
		}
	}
}

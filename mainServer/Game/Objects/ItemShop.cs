using System.Collections.Generic;

namespace gameServer.Game.Objects
{
	public class ItemShop
	{
		private static readonly ItemShop instance = new ItemShop();
		private ItemShop()
		{
		}

		public static ItemShop Instance
		{
			get
			{
				return instance;
			}
		}

		private Dictionary<int, ShopItem> itemShop = new Dictionary<int, ShopItem>();

		public void addItemData(int iID, ShopItem si)
		{
			itemShop.Add(iID, si);
		}

		public ShopItem getShopItemData(int key)
		{
			if(!itemShop.ContainsKey(key))
				return null;
			return itemShop[key];
		}
	}
}

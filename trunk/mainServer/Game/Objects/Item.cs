using System.Collections.Generic;
using System.Linq;

namespace gameServer.Game.Objects {
	public class ItemVendor
	{
		private Item item;
		private int id, invIndex;
		private short amount;
		private long price;

		public ItemVendor(Item item, int invIndex, long price, short amount, int id)
		{
			this.item = item;
			this.invIndex = invIndex;
			this.price = price;
			this.amount = amount;
			this.id = id;
		}

		public Item getItemFrame()
		{
			return item;
		}

		public void decrementAmount(short value)
		{
			amount -= value;
		}

		public int getItemID()
		{
			return id;
		}

		public int getInvIndex()
		{
			return invIndex;
		}

		public short getQuantity()
		{
			return amount;
		}

		public long getPrice()
		{
			return price;
		}
	}

	public class Item
	{
		private int itemID = 0;
		private short quantity = 0;
		private int ending = 0; // just for sickle etc ??

		public Item(int itemID = 0, short quantity = 1, long expiration = 0) {
			if(itemID != 0)
				quantity = 1;
			this.itemID = itemID;
			this.quantity = quantity;
			checkEnding();
		}

		private void checkEnding() {

		}

		public void setItemID(int itemID) {
			this.itemID = itemID;
		}

		public int getItemID() {
			if(this == null)
				return 0;
			return this.itemID;
		}

		public void setQuantity(short quantity)
		{
			this.quantity = quantity;
		}

		public short getQuantity()
		{
			return this.quantity;
		}

		public long getEnding() {
			return this.ending;
		}
	}

	public class ShopItem
	{
		private int price;
		private Dictionary<int, int> items = new Dictionary<int, int>(); //itemID & quantity

		public ShopItem(int price, Dictionary<int, int> items)
		{
			this.price = price;
			this.items = items;
		}

		public int getItemID()
		{
			return this.items.Keys.ElementAt(0);
		}

		public int getItemQuantity()
		{
			return this.items.Values.ElementAt(0);
		}

		public int getPrice()
		{
			return this.price;
		}
	}
}

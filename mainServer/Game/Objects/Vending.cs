using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using gameServer.Packets;
using gameServer.Game.World;

namespace gameServer.Game.Objects
{
	public class Vending
	{
		private Dictionary<int, ItemVendor> itemsMap = new Dictionary<int, ItemVendor>();
		private Dictionary<int, Point> itemCoords = new Dictionary<int, Point>();
		private Character owner;
		private string shopName;
		private int vendingPointsSpeed = 360000;
		private Timer vendingPoints = new Timer();
		private int moneyCap;
		private long totalMoney = 0;

		public Vending(Character owner, string name)
		{
			this.owner = owner;
			this.shopName = name;
			vendingPoints.Interval = vendingPointsSpeed;
			vendingPoints.Elapsed += new ElapsedEventHandler(this.addVendingPoints_elapsed);
			vendingPoints.Start();
		}

		public Dictionary<int, ItemVendor> getItems()
		{
			return itemsMap;
		}

		public Dictionary<int, Point> getCoords()
		{
			return itemCoords;
		}
	
		public String getShopName() {
			return shopName;
		}
	
		public void removeItemList(byte id) {
			itemsMap.Remove(id);
			itemCoords.Remove(id);
		}
	
		public byte[] addToVendorList() {
			WMap.Instance.addVendorList(owner);
			return VendingPackets.getVendorListPacket(owner);
		}

		public void addVendingPoints_elapsed(object sender, ElapsedEventArgs e)
		{
			this.owner.setVp((byte)(this.owner.getVp() + 1));
			this.owner.getAccount().mClient.WriteRawPacket(VendingPackets.getFameVendingPacket(this.owner));
		}

		public byte[] removeFromVendorList() {
			if(!WMap.Instance.removeVendorList(owner))
				Console.WriteLine("Something went wrong deleting this vendor");
			return VendingPackets.getVendorListPacket(owner);
		}
	
		public void deleteVendor() {
			//stops timer
			vendingPoints.Stop();
		}
	
		public byte[] addItem(ItemVendor item, int state, int x, int y) {
			int check = -1;
			check = owner.getInventory().getSeqSaved()[item.getInvIndex()];
			if(check < 0)
				Console.WriteLine("Item does not exist in inventory");
		
			if(totalMoney + item.getPrice()*item.getQuantity() > moneyCap - owner.getCoin())
				Console.WriteLine("You reached the selling value cap. The total of all items cannot be more if the sum is more as " + moneyCap + ".");
		
			totalMoney += item.getPrice();
			itemsMap.Add(item.getItemID(), item);
			itemCoords.Add(item.getItemID(), new Point(x, y));
			return VendingPackets.addItemToVendor(item, state, owner, x, y);
		}
	
		public byte[] removeItem(byte index, int state, int x, int y)  {
			if(!itemsMap.ContainsKey(index))
			{
				Console.WriteLine("Item does not exist!");
				return null;
			}
			
			ItemVendor item = itemsMap[index];

			if(item == null)
			{
				Console.WriteLine("Item does not exist!");
				return null;
			}
		
			itemsMap.Remove(index);
			itemCoords.Remove(index);
			return VendingPackets.addItemToVendor(item, state, owner, x, y);
		}
	
		public ItemVendor createItem(Item item, int invIndex, long price, short amount, int itemUid) {
			int check = -1;
			if(owner.getInventory().getSeqSaved()[invIndex] == -1)
			{
				Console.WriteLine("Item does not exist!");
				return null;
			}
			check = owner.getInventory().getSeqSaved()[invIndex];
			if(check < 0)
			{
				Console.WriteLine("Item does not exist in inventory");
				return null;
			}
			return new ItemVendor(item, invIndex, price, amount, itemUid);
		}
	
		public Boolean soldItem(Character buy, long price, byte index, int invSlot, int x, int y, short amount) {
			if(buy.getCoin() - (price * amount) < 0 || owner.getCoin() + (price*amount) > long.MaxValue)
			{
				Console.WriteLine("Let me show you, where i've been.");
				return false;
			}
			if(!itemsMap.ContainsKey(index))
			{
				Console.WriteLine("no shit sherlock, item doesn't exist!");
				return false;
			}

			ItemVendor item = itemsMap[index];
			if(item == null)
			{
				Console.WriteLine("Item does not exist!");
				return false;
			}

			if(amount > item.getQuantity())
			{
				Console.WriteLine("This is not the right amount.");
				return false;
			}

			buy.setCoin(owner.getCoin() - (price * amount));
			owner.setCoin(owner.getCoin() + (price * amount));
		
			//when item not deleted
			if(item.getQuantity() > 1 &&  amount != item.getQuantity()) {
				item.decrementAmount(amount);
				//replace with new values in vendor
				itemsMap[index] = item;
			}
			else {
				//remove item vendor
				removeItemList(index);
			}
			
			//remove/decrement item
			owner.getInventory().updateInv();
			if(!owner.getInventory().removeItem(invSlot, amount))
			{
				Console.WriteLine("chuj kurwa");
				return false;
			}
			owner.getInventory().saveInv();
		
			//add item
			Item newItem = new Item(item.getItemID(), item.getQuantity());
			Console.WriteLine("ItemID: " + newItem.getItemID());
			buy.getInventory().updateInv();
			if(!buy.getInventory().addItem(y, x, newItem))
			{
				Console.WriteLine("Something went wrong while adding the item");
				return false;
			}
			buy.getInventory().saveInv();
			buy.getAccount().mClient.WriteRawPacket(VendingPackets.buyItemFromVendor(buy, owner, index, invSlot, x, y, amount));
			owner.getAccount().mClient.WriteRawPacket(VendingPackets.buyItemFromVendorSecondSite(buy, owner, index, invSlot, x, y, amount));
			return true;
		}
	
		public void open(Character open, int vendorId) {
			open.getAccount().mClient.WriteRawPacket(VendingPackets.openVendorFrame(open, itemsMap, itemCoords, owner.getuID(), vendorId));
		}
	}
}

/* ------------------------------------------------------------------------------*
* 																				 *
* 									Property of:								 *
* 										EoMH									 *
* 																				 *
* ------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using gameServer.Game.Caches;

namespace gameServer.Game.Objects
{
	public class Inventory
	{
		private int pages;
		private Dictionary<int, Item> inv = new Dictionary<int, Item>();
		private List<int> seq = new List<int>();
		private int indexHold = -1;
		private int indexToSwap = -1;
		private Item holdingItem;
		private Boolean equipping = false;
		private int maxcoins = 1000000000;
		private short maxVendingPoints = 225;
		//saved lists
		private Dictionary<int, Item> invSaved = new Dictionary<int, Item>();
		private List<int> seqSaved = new List<int>();
		private Character owner;

		public Inventory(Character owner)
		{
			this.owner = owner;
			this.pages = owner.getInvPages();
			this.seq = new List<int>();
			this.seqSaved = new List<int>();
			this.fillSequences();
		}

		public Character getOwner()
		{
			return this.owner;
		}

		public int getIndexToSwap()
		{
			return this.indexToSwap;
		}

		public void setIndexHold(int index)
		{
			this.indexHold = index;
		}

		public void setIndexToSwap(int swap)
		{
			this.indexToSwap = swap;
		}

		public void setHoldingItem(Item item)
		{
			this.holdingItem = item;
		}

		public void printInv()
		{
			//list
			Console.WriteLine();
			int i = 0;
			foreach(int tehInv in inv.Keys)
			{
				Console.Write("|" + i + ":" + tehInv + "|");
				i++;
			}
			Console.WriteLine();
		}

		public void printSeq(){
			//list
			Console.WriteLine();
			int i=0;
			foreach(int tehSeq in seq)
			{
				Console.Write("|" + i + ":" + tehSeq + "|");
				i++;
			}
			Console.WriteLine();
		}

		public List<int> getSeq()
		{
			return this.seq;
		}

		//move item to inventory from cargo
		public Boolean moveFromCargo(Item item, int cargoSlot, int line, int row)
		{
			updateInv();

			Cargo cargo = owner.getCargo();
			
			cargo.updateCargo();

			//ADD TO INVENTORY
			Item it;
			inv.TryGetValue(row * 100 + line, out it);
			if(it != null && item.getItemID() == it.getItemID() && item.getQuantity() + it.getQuantity() > ItemDataCache.Instance.getItemData(item.getItemID()).getMaxStack())
			{
				Console.WriteLine("Cannot move item [max stack]");
				return false;
			}

			//get the hashes from all blocking items
			List<int> hash = this.checkBlockingItems(line, row, item);

			//exception in checkBlockingItems
			if(hash == null)
			{
				Console.WriteLine("Cannot buy item [crosses inventory border]");
				return false;
			}

			//move to an empty slot
			if(hash.Count == 0)
			{
				seq[nextFreeSequence()] = (row * 100) + line;
				putIntoInv(line, row, item);
				cargo.removeItem(cargoSlot);
				saveInv();
				return true;
			}

			//swap
			if(hash.Count == 1)
			{
				indexHold = seq.IndexOf(hash[0]);
				indexToSwap = nextFreeSequence();
				if(indexToSwap == -1)
				{
					Console.WriteLine("Cannot buy item [no free space in inv]");
					return false;
				}
				holdingItem = inv[hash[0]];
				removeItemFromInv(hash[0]);
				putIntoInv(line, row, item);
				seq[indexToSwap] = (row * 100) + line;
				seq[indexHold] = -1;
				cargo.removeItem(cargoSlot);
				saveInv();
				return true;
			}

			Console.WriteLine("Count {0}", hash.Count);
			return false;
		}

		//buy an item
		public Boolean buyItem(Item item, int line, int row)
		{
			updateInv();
			Item it;
			inv.TryGetValue(row * 100 + line, out it);
			if(it != null && item.getItemID() == it.getItemID() && item.getQuantity() + it.getQuantity() > ItemDataCache.Instance.getItemData(item.getItemID()).getMaxStack())
			{
				Console.WriteLine("Cannot buy item [max stack]");
				return false;
			}

			//get the hashes from all blocking items
			List<int> hash = this.checkBlockingItems(line, row, item);

			//exception in checkBlockingItems
			if(hash == null)
			{
				Console.WriteLine("Cannot buy item [crosses inventory border]");
				return false;
			}

			//move to an empty slot
			if(hash.Count == 0)
			{
				if(!addItem(line, row, item))
				{
					Console.WriteLine("Cannot buy item [no space]");
					return false;
				}
				saveInv();
				Console.WriteLine("Item bought at free line {0} row {1}",  line, row);
				return true;
			}

			//swap
			if(hash.Count == 1)
			{
				indexHold = seq.IndexOf(hash[0]);
				indexToSwap = nextFreeSequence();
				if(indexToSwap == -1)
				{
					Console.WriteLine("Cannot buy item [no free space in inv]");
					return false;
				}
				Console.WriteLine("Item bought at free seq {0}, line {1} row {2}", indexToSwap, line, row);
				holdingItem = inv[hash[0]];
				removeItemFromInv(hash[0]);
				putIntoInv(line, row, item);
				seq[indexToSwap] = (row * 100) + line;
				seq[indexHold] = -1;
				saveInv();
				return true;
			}
			Console.WriteLine("lolItem bought at free seq {0}, line {1} row {2} hash.Count {3}", indexToSwap, line, row, hash.Count);
			return false;
		}

		//craft an item
		public Boolean craftItem(Item it)
		{
			Item newItem = new Item(it.getItemID());

			if(it.getItemID() == 0)
			{
				Console.WriteLine("Cannot pick item [item has wrong id]");
				return false;
			}

			int line = 0;
			int row = 0;
			int i = 0;
			Boolean noMoreSeq = false;

			//move through seq to search stack
			while(noMoreSeq == false && i < 240)
			{
				if(seq[i] != -1)
				{
					Item item = inv[seq[i]];
					if(item == null)
					{
						Console.WriteLine("Cannot pick item [item in inv missing]");
						return false;
					}
					if(item.getItemID() == it.getItemID() && item.getQuantity() + newItem.getQuantity() <= ItemDataCache.Instance.getItemData(item.getItemID()).getMaxStack())
					{
						//stack
						item.setQuantity((short)(item.getQuantity() + newItem.getQuantity()));
						return true;
					}
				}
				else
				{
					noMoreSeq = true;
				}
				i++;
			}

			//move through all lines and rows until free slot is found
			while(row < pages * 5)
			{
				if(line == 8)
					line = 0;
				while(line < 8)
				{
					if(addItem(line, row, newItem))
					{
						return true;
					}
					line++;
				}
				row++;
			}
			Console.WriteLine("Cannot pick item [no free space in inv]");
			return true;
		}

		public Boolean sellItem(int index, int amount, Item tehItem = null)
		{
			updateInv();
			int hash = seq[index];
			if(hash == -1)
			{
				Console.WriteLine("Cannot sell item [item does not exist]");
				return false;
			}
			Item item = inv[hash];
			if(item == null)
			{
				Console.WriteLine("Cannot sell item [item does not exist]");
				return false;
			}
			if(tehItem != null)
			{
				tehItem.setItemID(item.getItemID());
			}
			removeItem(index, amount);
			saveInv();
			return true;
		}

		public int getMaxCoins()
		{
			return maxcoins;
		}

		public void updateVendingPoints(Character cur)
		{
			//byte[] vendingp = CharacterPackets.getFameVendingPacket(cur);
			//cur.addWritePacketWithId(vendingp);
		}

		//save seq and inv
		public void saveInv()
		{
			invSaved.Clear();
			seqSaved.Clear();
			invSaved = new Dictionary<int, Item>(inv);
			seqSaved = new List<int>(seq);
		}

		//update seq and inv
		public void updateInv()
		{
			inv.Clear();
			seq.Clear();
			inv = new Dictionary<int, Item>(invSaved);
			seq = new List<int>(seqSaved);
		}

		public int getIndexHold()
		{
			return indexHold;
		}

		public Item getHoldingItem()
		{
			return holdingItem;
		}

		public int getPages()
		{
			return pages;
		}

		public Dictionary<int, Item> getInvSaved()
		{
			return invSaved;
		}

		public void setInvSaved(Dictionary<int, Item> invSaved)
		{
			this.invSaved = invSaved;
		}

		public List<int> getSeqSaved()
		{
			return seqSaved;
		}

		public void setSeqSaved(List<int> seqSaved)
		{
			this.seqSaved = seqSaved;
		}

		// make sure all sequences are present
		private void fillSequences()
		{
			for(int i = 0;i <= 240;i++)
			{
				seq.Add(-1);
				seqSaved.Add(-1);
			}
		}

		// find first free sequence (aka. first that has value of 0)
		public int nextFreeSequence()
		{
			int c = this.seq.IndexOf((int)-1);
			return c;
		}
		// increase amount of pages in inventory
		public void addPages(int amount)
		{
			if(amount > 0 || amount < 4)
				this.pages += amount;
		}

		public void setPages(int amount)
		{
			if(amount > 0 || amount <= 6)
				this.pages = amount;
		}

		public int getSeqAtIndex(int index)
		{
			if(seq.Contains(index))
			{
				return seq[index];
			}
			else
				return -1;
		}

		public Item getItemBySeqIndexing(int seqIndex)
		{
			if(seqIndex < 0)
			{
				Console.WriteLine("Cannot decrement item [illegal index]");
				return null;
			}

			if(seq.ElementAt(seqIndex) == -1)
			{
				Console.WriteLine("Cannot decrement item [missing item]");
				return null;
			}

			if(!inv.ContainsKey(seq.ElementAt(seqIndex)))
			{
				Console.WriteLine("decrement > inv doesn't contain a key");
				return null;
			}

			return inv[(seq.ElementAt(seqIndex))];
		}

		//decrement item with given seq index and delete it when amount is 0
		public Boolean decrementItem(int seqIndex, Item item = null)
		{
			updateInv();
			if(seqIndex < 0)
			{
				Console.WriteLine("Cannot decrement item [illegal index]");
				return false;
			}

			if(seq.ElementAt(seqIndex) == -1)
			{
				Console.WriteLine("Cannot decrement item [missing item]");
				return false;
			}

			if(!inv.ContainsKey(seq.ElementAt(seqIndex)))
			{
				Console.WriteLine("decrement > inv doesn't contain a key");
				return false;
			}

			item = inv[(seq.ElementAt(seqIndex))];

			if(item == null)
			{
				Console.WriteLine("Cannot decrement item [missing item]");
				return false;
			}

			if(item.getQuantity() == 0)
			{
				Console.WriteLine("Cannot decrement item [item amount is 0]");
				return false;
			}

			item.setQuantity((short)(item.getQuantity() - 1));

			if(item.getQuantity() == 0)
			{
				removeItemFromInv(seq.ElementAt(seqIndex));
				seq[seqIndex] = -1;
			}
			saveInv();
			return true;
		}

		//decrement item with given seq index and delete it when amount is 0
		public Boolean _decrementItem(int seqIndex, Item item = null)
		{
			if(seqIndex < 0)
			{
				Console.WriteLine("Cannot decrement item [illegal index]");
				return false;
			}

			if(seq.ElementAt(seqIndex) == -1)
			{
				Console.WriteLine("Cannot decrement item [missing item]");
				return false;
			}

			if(!inv.ContainsKey(seq.ElementAt(seqIndex)))
			{
				Console.WriteLine("decrement > inv doesn't contain a key");
				return false;
			}

			item = inv[(seq.ElementAt(seqIndex))];

			if(item == null)
			{
				Console.WriteLine("Cannot decrement item [missing item]");
				return false;
			}

			if(item.getQuantity() == 0)
			{
				Console.WriteLine("Cannot decrement item [item amount is 0]");
				return false;
			}

			item.setQuantity((short)(item.getQuantity() - 1));

			if(item.getQuantity() == 0)
			{
				removeItemFromInv(seq.ElementAt(seqIndex));
				seq[seqIndex] = -1;
			}
			return true;
		}

		public Boolean decrementItem(int seqIndex, int newQuantity)
		{
			updateInv();
			if(seqIndex < 0)
			{
				Console.WriteLine("Cannot decrement item [illegal index]");
				return false;
			}

			if(seq.ElementAt(seqIndex) == -1)
			{
				Console.WriteLine("Cannot decrement item [missing item]");
				return false;
			}

			if(!inv.ContainsKey(seq.ElementAt(seqIndex)))
			{
				Console.WriteLine("decrement > inv doesn't contain a key");
				return false;
			}

			Item item = inv[(seq.ElementAt(seqIndex))];

			if(item == null)
			{
				Console.WriteLine("Cannot decrement item [missing item]");
				return false;
			}

			if(item.getQuantity() == 0)
			{
				Console.WriteLine("Cannot decrement item [item amount is 0]");
				return false;
			}

			item.setQuantity((short)newQuantity);

			if(item.getQuantity() == 0)
			{
				removeItemFromInv(seq.ElementAt(seqIndex));
				seq[seqIndex] = -1;
			}
			saveInv();
			return true;
		}

		public Boolean _decrementItem(int seqIndex, int newQuantity)
		{
			if(seqIndex < 0)
			{
				Console.WriteLine("Cannot decrement item [illegal index]");
				return false;
			}

			if(seq.ElementAt(seqIndex) == -1)
			{
				Console.WriteLine("Cannot decrement item [missing item]");
				return false;
			}

			if(!inv.ContainsKey(seq.ElementAt(seqIndex)))
			{
				Console.WriteLine("decrement > inv doesn't contain a key");
				return false;
			}

			Item item = inv[(seq.ElementAt(seqIndex))];

			if(item == null)
			{
				Console.WriteLine("Cannot decrement item [missing item]");
				return false;
			}

			if(item.getQuantity() == 0)
			{
				Console.WriteLine("Cannot decrement item [item amount is 0]");
				return false;
			}

			item.setQuantity((short)newQuantity);

			if(item.getQuantity() == 0)
			{
				removeItemFromInv(seq.ElementAt(seqIndex));
				seq[seqIndex] = -1;
			}
			return true;
		}

		//equip an item
		public Boolean equipItem(int fromInvID, int toEquipID, Equipment equip)
		{
			updateInv();
			if(equipping == true)
			{
				fromInvID = seq.IndexOf(8);
				if(fromInvID == -1)
				{
					Console.WriteLine("Cannot equip item [item index missing]");
					return false;
				}
			}

			Item itemF;
			//swap if holdingItem
			if(indexHold != -1)
			{
				int saveSwapHash = seq[indexToSwap];
				seq[indexToSwap] = -1;
				seq[nextFreeSequence()] = seq[indexHold];
				seq[indexHold] = saveSwapHash;
				indexHold = -1;
				itemF = holdingItem;
			}
			else
			{
				//remove item from inv
				if(seq[fromInvID] == -1)
				{
					Console.WriteLine("Cannot equip item [item missing]");
					return false;
				}
				itemF = inv[seq[fromInvID]];
				if(itemF == null)
				{
					Console.WriteLine("Cannot equip item [item null(ghost)]");
					return false;
				}
				removeItemFromInv(seq[fromInvID]);
				seq[fromInvID] = -1;
			}

			ItemData itemE = ItemDataCache.Instance.getItemData(itemF.getItemID());
			Console.WriteLine("to equip slot {0} | itemcate {1}", toEquipID, itemE.getCategory());
			if(!itemE.getClassUsable()[equip.getOwner().getcClass() - 1])
			{
				Console.WriteLine("nie da sie ubrać qq :[ nie dla klasy huj");
				return false;
			}
			if(itemE.getMinLvl() > equip.getOwner().getLevel())
			{
				Console.WriteLine("nie da sie ubrać qq :[ za niski lvlion");
				return false;
			}
			if(itemE.getMaxLvl() < equip.getOwner().getLevel())
			{
				Console.WriteLine("nie da sie ubrać qq :[ za wysoki lvlion");
				return false;
			}
			if(itemE.getFaction() != 0 && equip.getOwner().getFaction() != itemE.getFaction())
			{
				Console.WriteLine("not for yar faction");
				return false;
			}
			short[] equipstats = itemE.getRequirementStats();
			short[] charstats = equip.getOwner().getCStats();
			for(int i = 0;i < 5;i++)
			{
				if(equipstats[i] > charstats[i])
				{
					Console.WriteLine("nie da sie ubrać qq :[ za niskie staty :<<");
					return false;
				}
			}
			switch(toEquipID)
			{
				/*0, cap
				1, neck
				2, cape
				3, jacket
				4, pants
				5, armor
				6, brace
				7, wep 1
				8, wep 2
				9, ring 1
				10, ring 2
				11, shoes
				12, bird
				13, tablet
				14, fame pad
				15, mount
				16, bead*/
				case 0:
				{
					if(itemE.getCategory() != 18)
						return false;
					break;
				}
				case 1:
				{
					if(itemE.getCategory() != 20)
						return false;
					break;
				}
				case 2:
				{
					break;
				}
				case 3:
				{
					if(itemE.getCategory() != 13)
						return false;
					break;
				}
				case 4:
				{
					if(itemE.getCategory() != 14)
						return false;
					break;
				}
				case 5:
				{
					if(itemE.getCategory() != 15)
						return false;
					break;
				}
				case 6:
				{
					if(itemE.getCategory() != 22)
						return false;
					break;
				}
				case 7:
					case 8:
					{
						if(itemE.getCategory() < 1 || itemE.getCategory() > 12)
							return false;
						break;
					}
				case 9:
					case 10:
					{
						if(itemE.getCategory() != 21)
							return false;
						break;
					}
				case 11:
				{
					if(itemE.getCategory() != 17)
						return false;
					break;
				}
				case 12:
					case 13:
					{
						if(itemE.getCategory() != 23 && itemE.getCategory() != 53)
							return false;
						break;
					}
				case 14:
				{
					break;
				}
				case 15:
				{
					if(itemE.getCategory() != 52 && itemE.getCategory() != 53 && itemE.getCategory() != 55)
						return false;
					break;
				}
				case 16:
				{
					if(itemE.getCategory() != 47)
						return false;
					break;
				}
				default:
				{
					return false;
				}
			}

			//equip
			if(equip.getEquipments().ContainsKey((byte)toEquipID))
			{
				//8 is imaginary slot
				seq[nextFreeSequence()] = 8;
				putIntoInv(8, 0, equip.getEquipments()[(byte)toEquipID]);
				equip.getEquipments()[(byte)toEquipID] = itemF;
				equipping = true;
			}
			else
			{
				equip.getEquipments()[(byte)toEquipID] = itemF;
			}

			saveInv();
			return true;
		}

		//unequip an item
		public Boolean unequipItem(int fromEquipID, int line, int row, Equipment equip)
		{
			updateInv();

			//REMOVE FROM EQUIP

			if(!equip.getEquipments().ContainsKey((byte)fromEquipID))
			{
				Console.WriteLine("Cannot unequip item [item missing]");
				return false;
			}
			Item itemF = equip.getEquipments()[(byte)fromEquipID];

			//ADD TO INVENTORY

			//get the hashes from all blocking items
			List<int> hash = this.checkBlockingItems(line, row, itemF);

			//exception in checkBlockingItems
			if(hash == null)
			{
				Console.WriteLine("Cannot unequip item [crosses inventory border]");
				return false;
			}

			//move to an empty slot
			if(hash.Count == 0)
			{
				seq[nextFreeSequence()] = (row * 100) + line;
				putIntoInv(line, row, itemF);
				saveInv();
				equip.getEquipments().Remove((byte)fromEquipID);
				return true;
			}

			//swap
			if(hash.Count == 1)
			{
				indexHold = seq.IndexOf(hash[0]);
				indexToSwap = nextFreeSequence();
				if(indexToSwap == -1)
				{
					Console.WriteLine("Cannot unequip item [no free space in inv]");
					return false;
				}
				holdingItem = inv[hash[0]];
				removeItemFromInv(hash[0]);
				putIntoInv(line, row, itemF);
				seq[indexToSwap] = (row * 100) + line;
				seq[indexHold] = -1;
				saveInv();
				equip.getEquipments().Remove((byte)fromEquipID);
				return true;
			}
			Console.WriteLine("Cannot unequip item [too many items blocking]");
			return false;
		}

		//drop item from inv to ground
		public int dropItem(int fromInvID, int amount, Character cur, Boolean isCoin)
		{
			int uid = 0;

			//amount must be >0
			if(amount == 0)
			{
				Console.WriteLine("Cannot drop item [amount is 0]");
				return -1;
			}

			//amount must be <=10000
			if(amount > 10000 && !isCoin)
			{
				Console.WriteLine("Cannot drop item [amount is >=10000]");
				return -1;
			}

			//if it was an equipped item
			if(equipping == true)
			{
				fromInvID = seq.IndexOf(8);
				if(fromInvID == -1)
				{
					Console.WriteLine("Cannot equip item [item index missing]");
					return -1;
				}
			}

			//SWAPPED BEFORE
			if(indexHold != -1)
			{
				//swap indexes first
				int saveSwapHash = seq[indexToSwap];
				seq[indexToSwap] = -1;
				seq[nextFreeSequence()] = seq[indexHold];
				seq[indexHold] = saveSwapHash;

				//create drop
				ItemData it = ItemDataCache.Instance.getItemData(holdingItem.getItemID());
				//uid=it.dropItem(cur.getCurrentMap(), cur.getLocation(), amount).getuid();
				//TODO

				indexHold = -1;
			}
			else
			{
				//NOT SWAPPED BEFORE

				ItemData it;
				//coin is not an item in inv
				if(isCoin == false)
				{
					if(seq[fromInvID] == -1)
					{
						Console.WriteLine("Cannot drop item [item missing]");
						return -1;
					}
					Item itemF = inv[seq[fromInvID]];
					if(itemF == null)
					{
						Console.WriteLine("Cannot drop item [item null(ghost)]");
						return -1;
					}
					//wrong amount
					if(amount > itemF.getQuantity())
					{
						Console.WriteLine("Cannot drop item [item amount not sync]");
						return -1;
					}

					//create drop
					it = ItemDataCache.Instance.getItemData(inv[seq[fromInvID]].getItemID());

					//reduce amount or remove item
					itemF.setQuantity((short)(itemF.getQuantity() - amount));
					if(itemF.getQuantity() == 0)
					{
						removeItemFromInv(seq[fromInvID]);
						seq[fromInvID] = -1;
					}

					//if it was an equipped item
					equipping = false;

				}
				else
				{
					it = ItemDataCache.Instance.getItemData(217000501);
					//subtract money and check if possible
					/*if(subtractCoins(amount) == false)
					{
						Console.WriteLine("Cannot drop item [not enough coins]");
						return -1;
					}*/
				}

				//TODO
				//uid=it.dropItem(cur.getCurrentMap(), cur.getLocation(), amount).getuid();
			}
			return uid;
		}

		//move item in inventory
		public Boolean moveItem(int fromInvID, int toInvID, int amount, int line, int row)
		{
			updateInv();

			//equipping uses toInvID as fromInvID what the hell and mysterious things when swapped
			if(equipping == true)
			{
				int seq8 = seq.IndexOf(8);
				if(seq8 == -1)
					fromInvID = toInvID;
				else
					fromInvID = seq8;
			}

			//index must exist
			if(indexHold == -1 && fromInvID >= 0 && seq[fromInvID] == -1)
			{
				Console.WriteLine("Cannot move item [item missing]");
				return false;
			}

			//get item1
			Item itemF;
			if(indexHold == -1)
				itemF = inv[seq[fromInvID]];
			else
				itemF = holdingItem;

			//there must be item1
			if(itemF == null)
			{
				Console.WriteLine("Cannot move item [item null(ghost)]");
				return false;
			}

			//wrong amount
			if(amount > itemF.getQuantity() || amount == 0)
			{
				Console.WriteLine("Cannot move item [item amount not sync]");
				return false;
			}
			if(amount > 10000)
			{
				Console.WriteLine("Cannot move item [amount is >10000]");
				return false;
			}

			//get the hashes from all blocking items
			List<int> hash = this.checkBlockingItems(line, row, itemF);

			//exception in checkBlockingItems
			if(hash == null)
			{
				Console.WriteLine("Cannot move item [crosses inventory border]");
				return false;
			}

			//move to an empty slot
			if(hash.Count == 0)
			{
				//SWAPPED BEFORE
				if(indexHold != -1)
				{
					putIntoInv(line, row, holdingItem);
					seq[indexHold] = seq[indexToSwap];
					seq[indexToSwap] = -1;
					seq[nextFreeSequence()] = (row * 100) + line;
					indexHold = -1;
					//NOT SWAPPED BEFORE
				}
				else
				{
					itemF.setQuantity((short)(itemF.getQuantity() - amount));
					if(itemF.getQuantity() == 0)
					{
						removeItemFromInv(seq[fromInvID]);
						seq[fromInvID] = (row * 100) + line;
					}
					else
					{
						int nfs = nextFreeSequence();
						if(nfs != -1)
						{
							seq[nextFreeSequence()] = (row * 100) + line;
						}
						else
						{
							Console.WriteLine("Cannot move item [no free space in inv]");
							return false;
						}
					}

					Item newItemF = new Item(itemF.getItemID());
					newItemF.setQuantity((short)amount);
					putIntoInv(line, row, newItemF);

					equipping = false;
				}
				saveInv();
				return true;
			}

			if(hash.Count == 1)
			{
				if(seq.IndexOf(hash[0]) != -1)
				{
					//SWAPPED BEFORE
					if(indexHold != -1)
					{
						//swap indexes first
						int saveSwapHash = seq[indexToSwap];
						seq[indexToSwap] = -1;
						seq[nextFreeSequence()] = seq[indexHold];
						seq[indexHold] = saveSwapHash;
						indexHold = indexToSwap;

						holdingItem = inv[hash[0]];
						if(holdingItem.getItemID() == itemF.getItemID() && holdingItem.getQuantity() + amount <= ItemDataCache.Instance.getItemData(itemF.getItemID()).getMaxStack())
						{
							holdingItem.setQuantity((short)(holdingItem.getQuantity() + amount));
							indexHold = -1;
						}
						else
						{
							//do not allow bigger stacks than maxstack
							if(holdingItem.getItemID() == itemF.getItemID() && holdingItem.getQuantity() + amount > ItemDataCache.Instance.getItemData(itemF.getItemID()).getMaxStack())
							{
								Console.WriteLine("Cannot move item [stack too big]");
								return false;
							}
							if(removeItemFromInv(hash[0]) == false)
							{
								Console.WriteLine("Cannot move item [swapped item error]");
								return false;
							}
							putIntoInv(line, row, itemF);
							int saveIndexHold = indexHold;
							indexToSwap = indexHold;
							indexHold = seq.IndexOf(hash[0]);
							if(indexHold == -1)
							{
								Console.WriteLine("Cannot move item [item missing]");
								return false;
							}
							seq[saveIndexHold] = (row * 100) + line;
							seq[indexHold] = -1;
						}
						//NOT SWAPPED BEFORE
					}
					else
					{
						Item itemTo = inv[hash[0]];
						if(itemTo.getItemID() == itemF.getItemID() && itemTo.getQuantity() + amount <= ItemDataCache.Instance.getItemData(itemTo.getItemID()).getMaxStack())
						{
							itemF.setQuantity((short)(itemF.getQuantity() - amount));
							if(itemF.getQuantity() == 0)
							{
								removeItemFromInv(seq[fromInvID]);
								seq[fromInvID] = -1;
							}
							itemTo.setQuantity((short)(itemTo.getQuantity() + amount));
						}
						else
						{
							//do not allow bigger stacks than maxstack
							if(itemTo.getItemID() == itemF.getItemID() && itemTo.getQuantity() + amount > ItemDataCache.Instance.getItemData(itemTo.getItemID()).getMaxStack())
							{
								Console.WriteLine("Cannot move item [stack too big]");
								return false;
							}
							indexHold = seq.IndexOf(hash[0]);
							indexToSwap = fromInvID;
							holdingItem = itemTo;
							removeItemFromInv(hash[0]);
							removeItemFromInv(seq[fromInvID]);
							putIntoInv(line, row, itemF);
							seq[indexToSwap] = (row * 100) + line;
							seq[indexHold] = -1;

							equipping = false;
						}
					}
					saveInv();
					return true;
				}
			}
			Console.WriteLine("Cannot move item [too many items blocking]");
			return false;
		}

		//try to put item into inv
		public Boolean pickItem(Item it, int amount)
		{
			updateInv();

			Item newItem = new Item(it.getItemID(), (short)amount);
			if(ItemDataCache.Instance.getItemData(newItem.getItemID()).getIsStackable() == false)
				newItem.setQuantity(1);

			if(it.getItemID() == 0)
			{
				Console.WriteLine("Cannot pick item [item has wrong id]");
				return false;
			}

			//coin
			if(it.getItemID() == 217000501)
			{
				//add money
				/*int dif = addCoins(newItem.getQuantity());
				if(dif != 0)
				{
					Console.WriteLine("Cannot pick item [coin limit]");
					return false;
				}*/
				saveInv();
				return true;
			}

			int line = 0;
			int row = 0;
			int i = 0;
			Boolean noMoreSeq = false;

			//move through seq to search stack
			while(noMoreSeq == false && i < 240)
			{
				if(seq[i] != -1)
				{
					Item item = inv[seq[i]];
					if(item == null)
					{
						Console.WriteLine("Cannot pick item [item in inv missing]");
						return false;
					}
					if(item.getItemID() == it.getItemID() && item.getQuantity() + newItem.getQuantity() <= ItemDataCache.Instance.getItemData(item.getItemID()).getMaxStack())
					{
						//stack
						if(ItemDataCache.Instance.getItemData(item.getItemID()).getIsStackable() == false)
							item.setQuantity(1);
						else
							item.setQuantity((short)(item.getQuantity() + newItem.getQuantity()));
						saveInv();
						return true;
					}
				}
				else
				{
					noMoreSeq = true;
				}
				i++;
			}

			//move through all lines and rows until free slot is found
			while(row < pages * 5)
			{
				if(line == 8)
					line = 0;
				while(line < 8)
				{
					if(addItem(line, row, newItem))
					{
						saveInv();
						return true;
					}
					line++;
				}
				row++;
			}
			Console.WriteLine("Cannot pick item [no free space in inv]");
			return false;
		}

		// adds item to inventory only if all needed slots are empty, return true is success, false otherwise
		public Boolean addItem(int line, int row, Item it)
		{
			//get the hashes from all blocking items
			List<int> hash = this.checkBlockingItems(line, row, it);

			//exception in checkBlockingItems
			if(hash == null)
			{
				Console.WriteLine("Error occured in checkingBlockingItems");
				return false;
			}

			//move to an empty slot
			if(hash.Count == 0)
			{
				if(put(line, row, it) == false)
				{
					Console.WriteLine("Cannot put item into given slot");
					return false;
				}
				return true;
			}

			//stack
			if(hash.Count == 1)
			{
				if(inv[hash[0]].getItemID() == it.getItemID() && inv[hash[0]].getQuantity() + it.getQuantity() <= ItemDataCache.Instance.getItemData(it.getItemID()).getMaxStack())
				{
					inv[hash[0]].setQuantity((short)(inv[hash[0]].getQuantity() + it.getQuantity()));
					return true;
				}
			}

			//items block
			Console.WriteLine("Items blocking (" + hash.Count + ")");
			return false;
		}

		//remove only in inv list
		private Boolean removeItemFromInv(int hash)
		{
			//cannot handle empty hash
			if(hash == -1)
			{
				Console.WriteLine("Cannot remove item with hash=-1");
				return false;
			}

			//get item
			Item itemF = inv[hash];

			//item must be valid
			if(itemF == null)
			{
				Console.WriteLine("Item with given hash is null");
				return false;
			}

			int width = ItemDataCache.Instance.getItemData(itemF.getItemID()).getWidth();
			int height = ItemDataCache.Instance.getItemData(itemF.getItemID()).getHeight();
			int line = hash % 100;
			int row = (hash - line) / 100;

			//remove all items from inv with given hash
			for(int j = 0;j < height;j++)
			{
				for(int i = 0;i < width;i++)
				{
					if(inv.ContainsKey(((row + j) * 100 + line + i)))
					{
						inv.Remove((row + j) * 100 + line + i);
					}
					else
					{
						Console.WriteLine("Item key in inv missing");
						return false;
					}
				}
			}

			return true;
		}

		//remove item with given id
		public Boolean removeItem(int invID)
		{
			updateInv();
			//if it was an equipped item
			if(equipping == true)
			{
				invID = seq.IndexOf(8);
				if(invID == -1)
				{
					Console.WriteLine("Cannot remove item [item index missing]");
					return false;
				}
			}

			if(indexHold != -1)
			{
				//swap indexes first
				int saveSwapHash = seq[indexToSwap];
				seq[indexToSwap] = seq[indexHold];
				seq[indexHold] = saveSwapHash;

				indexHold = -1;
			}
			else
			{
				int hash = seq[invID];
				//try to remove item from inv
				if(removeItemFromInv(hash) == false)
				{
					Console.WriteLine("Cannot remove item [item index missing]");
					return false;
				}
				//reset index slot
				seq[invID] = -1;
			}

			//if it was an equipped item
			equipping = false;
			saveInv();
			return true;
		}

		//remove from stack
		public Boolean removeItem(int invID, int amount)
		{
			int hash = seq[invID];
			if(hash == -1)
			{
				Console.WriteLine("Cannot remove item [item index missing]");
				return false;
			}
			Item item = inv[hash];
			if(item == null)
			{
				Console.WriteLine("Cannot remove item [item missing]");
				return false;
			}
			if(amount < item.getQuantity())
			{
				item.setQuantity((short)(item.getQuantity() - amount));
				return true;
			}
			else
			{
				removeItem(invID);
				return true;
			}
		}

		//put into seq
		private Boolean putIntoSeq(int line, int row)
		{
			//try to find next free sequence
			int nextSeq = nextFreeSequence();
			//free sequence not found
			if(nextSeq == -1)
			{
				Console.WriteLine("No empty slot in seq found");
				return false;
			}
			//sequence found and adding hash
			seq[nextSeq] = (row * 100) + line;
			return true;
		}

		//put into inv
		public void putIntoInv(int line, int row, Item it)
		{
			for(int i = line;i < (line + ItemDataCache.Instance.getItemData(it.getItemID()).getWidth());i++)
			{
				for(int u = row;u < (row + ItemDataCache.Instance.getItemData(it.getItemID()).getHeight());u++)
				{
					this.inv[(u * 100) + i] = it;
				}
			}
		}

		// insert item to all the slots it requires, no boundary checks are performed
		private Boolean put(int line, int row, Item it)
		{
			//try to put item into sequence
			if(putIntoSeq(line, row) == false)
			{
				Console.WriteLine("Cannot put item into line " + line + " and row " + row);
				return false;
			}
			putIntoInv(line, row, it);
			return true;
		}

		/* check if item will fit in inventory
		Returns list containing all items that are using same slots as tmp */
		public List<int> checkBlockingItems(int line, int row, Item tmp)
		{
			int width = ItemDataCache.Instance.getItemData(tmp.getItemID()).getWidth();
			List<Item> ls = new List<Item>();
			int height = ItemDataCache.Instance.getItemData(tmp.getItemID()).getHeight();
			Item it = null;
			List<int> hash = new List<int>();

			// boundary check
			if(!this.checkWlimit(line, width))
				return null;
			if(!this.checkHlimit(row, height))
				return null;

			// main loop
			for(int i = line;i < (line + width);i++)
			{
				for(int u = row;u < (row + height);u++)
				{
					// if item is in slot add it to list
					if(this.inv.ContainsKey((u * 100) + i))
					{
						it = this.inv[(u * 100) + i];
						if(!ls.Contains(it) && it != tmp)
						{
							//move to top left corner spot
							int ii = i;
							int uu = u;
							Boolean found = false;
							do
							{
								ii--;
							} while(inv.ContainsKey((uu * 100) + ii) && found == false && ii >= 0 && inv[(uu * 100) + ii] == it);
							ii++;
							do
							{
								uu--;
							} while(inv.ContainsKey((uu * 100) + ii) && found == false && uu >= 0 && inv[(uu * 100) + ii] == it);
							uu++;
							ls.Add(it);
							hash.Add((uu * 100) + ii);
						}
					}
				}
			}
			return hash;
		}

		// check if item will fit to inventory by height
		private Boolean checkHlimit(int row, int height)
		{
			if((this.pages * 5) >= (row + height))
			{
				int page = (int)Math.Floor((decimal)row / 5);
				if((row + height) <= ((page + 1) * 5) && row >= (page * 5))
				{
					return true;
				}
			}
			return false;
		}

		// check if item will fit to inventory by width
		private Boolean checkWlimit(int line, int width)
		{
			if(line + width <= 8)
				return true;
			return false;
		}
	}
}
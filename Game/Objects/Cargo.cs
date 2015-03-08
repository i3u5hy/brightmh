using System;
using System.Collections.Generic;
using gameServer.Game.Caches;

namespace gameServer.Game.Objects
{
	public class Cargo
	{
		private Dictionary<int, Item> cargo = new Dictionary<int, Item>();
		private List<int> seq = new List<int>();
		private Boolean equipping = false;
		//saved lists
		private Dictionary<int, Item> cargoSaved = new Dictionary<int, Item>();
		private List<int> seqSaved = new List<int>();
		private Character owner;

		public Cargo(Character owner)
		{
			this.owner = owner;
			this.seq = new List<int>();
			this.seqSaved = new List<int>();
			this.fillSequences();
		}

		// make sure all sequences are present
		private void fillSequences()
		{
			for(int i = 0;i <= 120;i++)
			{
				seq.Add(-1);
				seqSaved.Add(-1);
			}
		}

		//move item in cargo
		public Boolean moveItem(int fromCargoID, int toCargoID, int line, int row)
		{
			updateCargo();

			//equipping uses toCargoID as fromCargoID what the hell and mysterious things when swapped
			if(equipping == true)
			{
				int seq8 = seq.IndexOf(8);
				if(seq8 == -1)
					fromCargoID = toCargoID;
				else
					fromCargoID = seq8;
			}

			//index must exist
			if(owner.getInventory().getIndexHold() == -1 && fromCargoID >= 0 && seq[fromCargoID] == -1)
			{
				Console.WriteLine("Cannot move item [item missing]");
				return false;
			}

			//get item1
			Item itemF;
			if(owner.getInventory().getIndexHold() == -1)
				itemF = cargo[seq[fromCargoID]];
			else
				itemF = owner.getInventory().getHoldingItem();

			//there must be item1
			if(itemF == null)
			{
				Console.WriteLine("Cannot move item [item null(ghost)]");
				return false;
			}

			short amount = itemF.getQuantity();

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
				Console.WriteLine("Cannot move item [crosses cargo border]");
				return false;
			}

			//move to an empty slot
			if(hash.Count == 0)
			{
				//SWAPPED BEFORE
				if(owner.getInventory().getIndexHold() != -1)
				{
					putIntoCargo(line, row, owner.getInventory().getHoldingItem());
					seq[owner.getInventory().getIndexHold()] = seq[owner.getInventory().getIndexToSwap()];
					seq[owner.getInventory().getIndexToSwap()] = -1;
					seq[nextFreeSequence()] = (row * 100) + line;
					owner.getInventory().setIndexHold(-1);
					//NOT SWAPPED BEFORE
				}
				else
				{
					itemF.setQuantity((short)(itemF.getQuantity() - amount));
					if(itemF.getQuantity() == 0)
					{
						removeItemFromCargo(seq[fromCargoID]);
						seq[fromCargoID] = (row * 100) + line;
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
							Console.WriteLine("Cannot move item [no free space in cargo]");
							return false;
						}
					}

					Item newItemF = new Item(itemF.getItemID());
					newItemF.setQuantity((short)amount);
					putIntoCargo(line, row, newItemF);

					equipping = false;
				}
				saveCargo();
				return true;
			}

			if(hash.Count == 1)
			{
				if(seq.IndexOf(hash[0]) != -1)
				{
					//SWAPPED BEFORE
					if(owner.getInventory().getIndexHold() != -1)
					{
						//swap indexes first
						int saveSwapHash = seq[owner.getInventory().getIndexToSwap()];
						seq[owner.getInventory().getIndexToSwap()] = -1;
						seq[nextFreeSequence()] = seq[owner.getInventory().getIndexHold()];
						seq[owner.getInventory().getIndexHold()] = saveSwapHash;
						owner.getInventory().setIndexHold(owner.getInventory().getIndexToSwap());

						owner.getInventory().setHoldingItem(cargo[hash[0]]);
						if(owner.getInventory().getHoldingItem().getItemID() == itemF.getItemID() && owner.getInventory().getHoldingItem().getQuantity() + amount <= ItemDataCache.Instance.getItemData(itemF.getItemID()).getMaxStack())
						{
							owner.getInventory().getHoldingItem().setQuantity((short)(owner.getInventory().getHoldingItem().getQuantity() + amount));
							owner.getInventory().setIndexHold(-1);
						}
						else
						{
							//do not allow bigger stacks than maxstack
							if(owner.getInventory().getHoldingItem().getItemID() == itemF.getItemID() && owner.getInventory().getHoldingItem().getQuantity() + amount > ItemDataCache.Instance.getItemData(itemF.getItemID()).getMaxStack())
							{
								Console.WriteLine("Cannot move item [stack too big]");
								return false;
							}
							if(removeItemFromCargo(hash[0]) == false)
							{
								Console.WriteLine("Cannot move item [swapped item error]");
								return false;
							}
							putIntoCargo(line, row, itemF);
							int saveIndexHold = owner.getInventory().getIndexHold();
							owner.getInventory().setIndexToSwap(owner.getInventory().getIndexHold());
							owner.getInventory().setIndexHold(seq.IndexOf(hash[0]));
							if(owner.getInventory().getIndexHold() == -1)
							{
								Console.WriteLine("Cannot move item [item missing]");
								return false;
							}
							seq[saveIndexHold] = (row * 100) + line;
							seq[owner.getInventory().getIndexHold()] = -1;
						}
						//NOT SWAPPED BEFORE
					}
					else
					{
						Item itemTo = cargo[hash[0]];
						if(itemTo.getItemID() == itemF.getItemID() && itemTo.getQuantity() + amount <= ItemDataCache.Instance.getItemData(itemTo.getItemID()).getMaxStack())
						{
							itemF.setQuantity((short)(itemF.getQuantity() - amount));
							if(itemF.getQuantity() == 0)
							{
								removeItemFromCargo(seq[fromCargoID]);
								seq[fromCargoID] = -1;
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
							owner.getInventory().setIndexHold(seq.IndexOf(hash[0]));
							owner.getInventory().setIndexToSwap(fromCargoID);
							owner.getInventory().setHoldingItem(itemTo);
							removeItemFromCargo(hash[0]);
							removeItemFromCargo(seq[fromCargoID]);
							putIntoCargo(line, row, itemF);
							seq[owner.getInventory().getIndexToSwap()] = (row * 100) + line;
							seq[owner.getInventory().getIndexHold()] = -1;

							equipping = false;
						}
					}
					saveCargo();
					return true;
				}
			}
			Console.WriteLine("Cannot move item [too many items blocking]");
			return false;
		}

		//buy an item
		public Boolean insertItemFromInventory(Inventory inv, int itemSlot, int line, int row)
		{
			updateCargo();
			inv.updateInv();

			Item item = null;
			if(inv.getIndexHold() != -1) item = inv.getHoldingItem();
			else item = inv.getItemBySeqIndexing(itemSlot);
			if(item == null)
			{
				Console.WriteLine("[inv -> cargo] tried to move not existing item");
				return false;
			} else Console.WriteLine("moving item: {0} at index {1} and seqIndex {2} and line {3} and row {4}", ItemDataCache.Instance.getItemData(item.getItemID()).getName(), itemSlot, inv.getSeq()[itemSlot], line, row);

			short amount = item.getQuantity();

			Item it;
			cargo.TryGetValue(row * 100 + line, out it);
			if(it != null && item.getItemID() == it.getItemID() && item.getQuantity() + it.getQuantity() > ItemDataCache.Instance.getItemData(item.getItemID()).getMaxStack())
			{
				Console.WriteLine("[inv -> cargo] cannot move item, max stack");
				return false;
			}

			List<int> hash = this.checkBlockingItems(line, row, item);

			//exception in checkBlockingItems
			if(hash == null)
			{
				Console.WriteLine("Cannot move item [crosses inventory border]");
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
				if(inv.getIndexHold() != -1)
				{
					inv.setHoldingItem(null);
				}
				else
				{
					inv.removeItem(itemSlot);
				}
				saveCargo();
				inv.saveInv();
				Console.WriteLine("Item putt3d at free line {0} row {1}", line, row);
				return true;
			}

			//swap
			if(hash.Count == 1)
			{
				owner.getInventory().setIndexHold(seq.IndexOf(hash[0]));
				owner.getInventory().setIndexToSwap(nextFreeSequence());
				if(owner.getInventory().getIndexToSwap() == -1)
				{
					Console.WriteLine("Cannot buy item [no free space in inv]");
					return false;
				}
				Console.WriteLine("Item putt3d at free seq {0}, line {1} row {2}", owner.getInventory().getIndexToSwap(), line, row);
				owner.getInventory().setHoldingItem(cargo[hash[0]]);
				removeItemFromCargo(hash[0]);
				putIntoCargo(line, row, item);
				seq[owner.getInventory().getIndexToSwap()] = (row * 100) + line;
				seq[owner.getInventory().getIndexHold()] = -1;
				inv.removeItem(itemSlot);
				saveCargo();
				inv.saveInv();
				return true;
			}
			Console.WriteLine("too m4ny it3mz 2 sw4p");
			return false;
		}

		//remove only in cargo list
		public Boolean removeItemFromCargo(int hash)
		{
			//cannot handle empty hash
			if(hash == -1)
			{
				Console.WriteLine("Cannot remove item with hash=-1");
				return false;
			}

			//get item
			Item itemF = cargo[hash];

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
					if(cargo.ContainsKey(((row + j) * 100 + line + i)))
					{
						cargo.Remove((row + j) * 100 + line + i);
					}
					else
					{
						Console.WriteLine("Item key in cargo missing");
						return false;
					}
				}
			}

			return true;
		}

		//remove item with given id
		public Boolean removeItem(int invID)
		{
			updateCargo();
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

			if(owner.getInventory().getIndexHold() != -1)
			{
				//swap indexes first
				int saveSwapHash = seq[owner.getInventory().getIndexToSwap()];
				seq[owner.getInventory().getIndexToSwap()] = seq[owner.getInventory().getIndexHold()];
				seq[owner.getInventory().getIndexHold()] = saveSwapHash;

				owner.getInventory().setIndexHold(-1);
			}
			else
			{
				int hash = seq[invID];
				//try to remove item from inv
				if(removeItemFromCargo(hash) == false)
				{
					Console.WriteLine("Cannot remove item [item index missing]");
					return false;
				}
				//reset index slot
				seq[invID] = -1;
			}

			//if it was an equipped item
			equipping = false;
			saveCargo();
			return true;
		}

		//save seq and inv
		public void saveCargo()
		{
			cargoSaved.Clear();
			seqSaved.Clear();
			cargoSaved = new Dictionary<int, Item>(cargo);
			seqSaved = new List<int>(seq);
		}

		//update seq and inv
		public void updateCargo()
		{
			cargo.Clear();
			seq.Clear();
			cargo = new Dictionary<int, Item>(cargoSaved);
			seq = new List<int>(seqSaved);
		}

		public List<int> getSeqSaved()
		{
			return seqSaved;
		}

		public void setSeqSaved(List<int> seqSaved)
		{
			this.seqSaved = seqSaved;
		}

		public Dictionary<int, Item> getCargoSaved()
		{
			return cargoSaved;
		}

		public Boolean addItem(int line, int row, Item it)
		{
			//get the hashes from all blocking items
			List<int> hash = this.checkBlockingItems(line, row, it);

			//exception in checkBlockingItems
			if(hash == null)
			{
				//Console.WriteLine("Error occured in checkingBlockingItems");
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
				if(cargo[hash[0]].getItemID() == it.getItemID() && cargo[hash[0]].getQuantity() + it.getQuantity() <= ItemDataCache.Instance.getItemData(it.getItemID()).getMaxStack())
				{
					cargo[hash[0]].setQuantity((short)(cargo[hash[0]].getQuantity() + it.getQuantity()));
					return true;
				}
			}

			//items block
			//Console.WriteLine("Items blocking (" + hash.Count + ")");
			return false;
		}

		//put into seq
		private Boolean putIntoSeq(int line, int row)
		{
			//try to find next free sequence
			int nextSeq = nextFreeSequence();
			//free sequence not found
			if(nextSeq == -1)
			{
				//Console.WriteLine("No empty slot in seq found");
				return false;
			}
			//sequence found and adding hash
			seq[nextSeq] = (row * 100) + line;
			return true;
		}

		// find first free sequence (aka. first that has value of 0)
		public int nextFreeSequence()
		{
			int c = this.seq.IndexOf((int)-1);
			//Console.WriteLine("New sequence; " + c);
			return c;
		}

		//put into inv
		public void putIntoCargo(int line, int row, Item it)
		{
			for(int i = line;i < (line + ItemDataCache.Instance.getItemData(it.getItemID()).getWidth());i++)
			{
				for(int u = row;u < (row + ItemDataCache.Instance.getItemData(it.getItemID()).getHeight());u++)
				{
					this.cargo[(u * 100) + i] = it;
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
			putIntoCargo(line, row, it);
			return true;
		}

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

			//Console.WriteLine("b4 main loop");

			// main loop
			for(int i = line;i < (line + width);i++)
			{
				for(int u = row;u < (row + height);u++)
				{
					// if item is in slot add it to list
					if(this.cargo.ContainsKey((u * 100) + i))
					{
						it = this.cargo[(u * 100) + i];
						if(!ls.Contains(it) && it != tmp)
						{
							//move to top left corner spot
							int ii = i;
							int uu = u;
							Boolean found = false;
							do
							{
								ii--;
							} while(cargo.ContainsKey((uu * 100) + ii) && found == false && ii >= 0 && cargo[(uu * 100) + ii] == it);
							ii++;
							do
							{
								uu--;
							} while(cargo.ContainsKey((uu * 100) + ii) && found == false && uu >= 0 && cargo[(uu * 100) + ii] == it);
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
			//Console.WriteLine("checkHlimit: {0} | {1}", row, height);
			if((2 * 10) >= (row + height))
			{
				int page = (int)Math.Floor((decimal)row / 10);
				//Console.WriteLine("if(({0} + {1}) <= (({2} + 1) * 10) && {0} >= (2 * 10))", row, height, page);
				if((row + height) <= ((page + 1) * 10) && row >= (page * 10))
				{
					//Console.WriteLine("guwno return true checkhujlimit");
					return true;
				}
			}
			return false;
		}

		// check if item will fit to inventory by width
		private Boolean checkWlimit(int line, int width)
		{
			//Console.WriteLine("checkWlimit: {0} | {1}", line, width);
			if(line + width <= 6)
				return true;
			return false;
		}
	}
}

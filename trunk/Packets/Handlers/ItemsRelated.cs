using System;
using System.Collections.Generic;
using System.Linq;
using gameServer.Core.IO;
using gameServer.Game;
using gameServer.Game.Caches;
using gameServer.Game.Misc;
using gameServer.Game.Objects;
using gameServer.Game.World;
using gameServer.Packets.Handlers.UseItem;
using gameServer.Tools;

namespace gameServer.Packets.Handlers
{
	public class VendingManagement
	{
		public static void StateVending(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook openVending while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte stateType = p.ReadByte();
			string shopName = p.ReadString(30);

			if(stateType == 1)
			{
				if(chr.getVending() != null)
				{
					Console.WriteLine("u already have teh vending biach");
					return;
				}

				chr.setVending(new Vending(chr, shopName));
				c.WriteRawPacket(VendingPackets.createVendorFrame(chr, 1, shopName));
				if(chr.getMap() == 1)
				{
					WMap.Instance.getGrid(1).addToVendings(chr);
				}
			} else {
				chr.getVending().deleteVendor();
				chr.setVending(null);
				c.WriteRawPacket(VendingPackets.createVendorFrame(chr, 0));
			}

			WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), VendingPackets.getExtVending(chr));
		}

		public static void BuyFromVending(MartialClient c, InPacket p)
		{

		}
	}

	public class CargoManagement
	{
		public static void MoveFromInv(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook cargo -> inv while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte managementType = p.ReadByte();
			byte fromInvIndex = p.ReadByte();
			byte toCargoSlot = p.ReadByte();
			byte toCargoLine = p.ReadByte();
			byte toCargoRow = p.ReadByte();

			Cargo cargo = chr.getCargo();
			Console.WriteLine("Cargo > {0} | {1} | {2} | {3} | {4}", managementType, fromInvIndex, toCargoSlot, toCargoLine, toCargoRow);
			if(!cargo.insertItemFromInventory(chr.getInventory(), fromInvIndex, toCargoRow, toCargoLine))
			{
				Console.WriteLine("da fuaaark");
				return;
			}

			OutPacket op = new OutPacket(24);
			op.WriteInt(24);
			op.WriteShort(4);
			op.WriteShort(44);
			op.WriteInt(1);
			op.WriteInt(chr.getuID());
			op.WriteShort(1);
			op.WriteByte(managementType);
			op.WriteByte(fromInvIndex);
			op.WriteByte(toCargoSlot);
			op.WriteByte(toCargoLine);
			op.WriteByte(toCargoRow);
			op.WriteByte(42);
			c.WriteRawPacket(op.ToArray());
		}

		public static void MoveToInv(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook cargo -> inv while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte fromCargoIndex = p.ReadByte();
			byte toInvSlot = p.ReadByte();
			byte toInvLine = p.ReadByte();
			byte toInvRow = p.ReadByte();

			Cargo cargo = chr.getCargo();
			Inventory inv = chr.getInventory();
			Console.WriteLine("Cargo > {0} | {1} | {2} | {3}", fromCargoIndex, toInvSlot, toInvLine, toInvRow);

			cargo.updateCargo();

			if(!cargo.getCargoSaved().ContainsKey((byte)fromCargoIndex))
			{
				Console.WriteLine("Cannot moveItemToInv [item missing]");
				return;
			}
			Item itemF = cargo.getCargoSaved()[(byte)fromCargoIndex];

			if(!inv.moveFromCargo(itemF, fromCargoIndex, toInvRow, toInvLine))
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "do kurwy nendzy");
				return;
			}
			cargo.saveCargo();

			OutPacket op = new OutPacket(24);
			op.WriteInt(24);
			op.WriteShort(4);
			op.WriteShort(45);
			op.WriteInt(1);
			op.WriteInt(chr.getuID());
			op.WriteShort(1);
			op.WriteByte(fromCargoIndex);
			op.WriteByte(toInvSlot);
			op.WriteByte(toInvLine);
			op.WriteByte(toInvRow);
			op.WriteShort(-16625);
			c.WriteRawPacket(op.ToArray());
		}

		public static void Move(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook cargo movement while not being ingame.");
				c.Close();
				return;
			}

			byte fromCargoIndex = p.ReadByte();
			short unknownMovement = p.ReadShort();
			byte toCargoSlot = p.ReadByte();
			byte toCargoLine = p.ReadByte();
			byte toCargoRow = p.ReadByte();
			Character chr = c.getAccount().activeCharacter;
			Cargo cargo = chr.getCargo();
			Console.WriteLine("Cargo > {0} | {1} | {2} | {3}", fromCargoIndex, toCargoSlot, toCargoLine, toCargoRow);

			if(!cargo.moveItem(fromCargoIndex, toCargoSlot, toCargoRow, toCargoLine))
			{
				Console.WriteLine("problem with move item");
				return;
			}

			OutPacket op = new OutPacket(24);
			op.WriteInt(24);
			op.WriteShort(4);
			op.WriteShort(46);
			op.WriteInt(1);
			op.WriteInt(chr.getuID());
			op.WriteShort(1);
			op.WriteByte(fromCargoIndex);
			op.WriteShort(unknownMovement);
			op.WriteByte(toCargoSlot);
			op.WriteByte(toCargoLine);
			op.WriteByte(toCargoRow);
			c.WriteRawPacket(op.ToArray());
		}
	}

	public class InventoryManagement
	{
		private static int[] payedItems = new int[] {
			217000035, // Gold Bar (1   Gold)
			217000036, // Gold Bar (2   Gold)
			217000037, // Gold Bar (5   Gold)
			217000028, // Gold Bar (10  Gold)
			217000029, // Gold Bar (100 Gold)
			217000242, // Gold Bar (500 Gold)
			217000243, // Gold Bar (1k  Gold)
			292000004, // Silv Bar (10  Silv)
			292000005, // Silv Bar (100 Silv)
			217000040, // Silv Bar (1   Silv)
		};

		public static void MHShop(MartialClient c, InPacket p)
		{
			byte[] decrypted = p.ReadBytes(80);

			int itemIdentificator = BitConverter.ToInt32(decrypted, 0);

			ShopItem shopItem = ItemShop.Instance.getShopItemData(itemIdentificator);

			if(shopItem == null)
			{
				Console.WriteLine("wrong id selected..");
				return;
			}

			if(c.getAccount().MHPoints < shopItem.getPrice())
			{
				Console.WriteLine("teh hacksorz..");
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			Console.WriteLine("si: {0} | {1} | {2} | {3}", itemIdentificator, shopItem.getItemID(), shopItem.getItemQuantity(), shopItem.getPrice());

			OutPacket op = new OutPacket(156);
			op.WriteInt(156);
			op.WriteShort(0x04);
			op.WriteShort(0x4b);
			op.WriteInt(1);
			op.WriteInt(chr.getuID());
			op.WriteInt(1);
			op.WriteRepeatedByte(1, 4);
			op.WriteRepeatedByte(1, 4);
			op.WriteInt(shopItem.getItemID());
			op.WriteInt(shopItem.getItemQuantity());
			op.WriteInt(shopItem.getItemID());
			op.WriteInt(shopItem.getItemQuantity());
			op.WriteInt(shopItem.getItemID());
			op.WriteInt(shopItem.getItemQuantity());
			op.WriteInt(shopItem.getItemID());
			op.WriteInt(shopItem.getItemQuantity()); // item quantity
			op.WriteRepeatedByte(1, 112);
			//op.Position = 152;
			//c.getAccount().MHPoints -= shopItem.getPrice();
			//op.WriteInt(shopItem.getPrice());
			c.WriteRawPacket(op.ToArray());

			Console.WriteLine(BitConverter.ToString(op.ToArray()));

			//TODO: Delivery items

			//System.Console.WriteLine("sent: {0}", System.BitConverter.ToString(op.ToArray()));
		}

		public static void ViewInventory(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Hooked viewInventory with null of activeCharacter");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			OutPacket op = new OutPacket(28);
			op.WriteInt(28);
			op.WriteShort(0x04);
			op.WriteShort(0x1e);
			op.WriteInt(0x01);
			op.WriteInt(chr.getuID());
			op.WriteShort(0x01);
			op.WriteByte(0xf8);
			op.WriteByte(0x01);

			p.Skip(4);

			op.WriteBytes(p.ReadBytes(4));
			op.WriteByte(0x9e);
			op.WriteByte(0x0f);
			op.WriteByte(0xbf);
			c.WriteRawPacket(op.ToArray());
		}

		public static void CraftItem(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook craftItem while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			int craftingID = p.ReadInt();
			int manualInventoryIndex = p.ReadInt(); // better to be sure, than be rzaah XD
			if(manualInventoryIndex < 0)
			{
				Console.WriteLine("manuel < 0");
				return;
			}

			Inventory inv = chr.getInventory();
			inv.updateInv();

			List<int> seq = new List<int>(inv.getSeqSaved());
			Dictionary<int, Item> items = new Dictionary<int, Item>(inv.getInvSaved());

			if(!items.ContainsKey(seq[manualInventoryIndex]))
			{
				Console.WriteLine("unknown item at index {0}", manualInventoryIndex);
				return;
			}
			Item item = items[seq[manualInventoryIndex]];

			ItemData itemData = ItemDataCache.Instance.getItemData(item.getItemID());
			if(itemData == null)
			{
				Console.WriteLine("unknown itemdata for item of ID {0}", item.getItemID());
				return;
			}

			if(itemData.getCategory() != 1010)
			{
				Console.WriteLine("dat shit ain't manual");
				return;
			}

			ManualData manual = ManualDataCache.Instance.getManualData(craftingID);
			if(manual == null)
			{
				Console.WriteLine("manual wasn't found..");
				return;
			}

			List<Item> providedMaterials = new List<Item>();
			List<int> providedMaterialID = new List<int>();
			List<int> providedMaterialQa = new List<int>();
			List<int> providedMaterialIndex = new List<int>();
			for(int i = 0;i < 8;i++)
			{
				int tempMaterialIndex = p.ReadInt();
				Console.WriteLine("indexez of provided mats {0}", tempMaterialIndex);
				if(tempMaterialIndex == -1)
					break;
				if(seq.ElementAt(tempMaterialIndex) == -1)
					return;
				if(!items.ContainsKey(seq[tempMaterialIndex]))
					return;
				Item tempMaterial = items[seq[tempMaterialIndex]];
				if(tempMaterial == null)
				{
					Console.WriteLine("unknown tempMaterial at index {0}", tempMaterialIndex);
					return;
				}
				if(tempMaterial.getQuantity() < 1)
				{
					Console.WriteLine("tempMaterial has less than 1 quantity :< {0}", tempMaterialIndex);
					return;
				}
				providedMaterials.Add(tempMaterial);
				providedMaterialID.Add(tempMaterial.getItemID());
				providedMaterialQa.Add(tempMaterial.getQuantity());
				providedMaterialIndex.Add(tempMaterialIndex);
			}

			if(providedMaterials.Count == 0)
			{
				Console.WriteLine("playa doesn't supplied materials at all");
				return;
			}

			List<int> deductedAmount = new List<int>(providedMaterialQa);

			List<int> requiredMaterialID = manual.getRequiredMaterials();
			List<int> requiredMaterialQa = manual.getRequiredQuantities();
			for(int i=0;i<providedMaterials.Count;i++) // let's check if playa has satisfied our data provided manual <3
			{
				if(providedMaterialQa[i] < 1)
					continue;
				for(int x=0;x<requiredMaterialID.Count;x++)
				{
					if(requiredMaterialQa[x] <= 0)
						continue;
					if(requiredMaterialID[x] == providedMaterialID[i])
					{
						if(requiredMaterialQa[x] >= providedMaterialQa[i])
						{
							requiredMaterialQa[x] -= providedMaterialQa[i];
							providedMaterialQa[i] = 0;
						}
						else
						{
							int tempQa = requiredMaterialQa[x];
							requiredMaterialQa[x] = 0;
							providedMaterialQa[i] -= tempQa;
						}
					}
				}
			}

			if(requiredMaterialQa.Sum() != 0)
			{
				Console.WriteLine("user hasn't applied all of the needed materialz, damn cheatz");
				return;
			}

			int craftedItemID = manual.getProducedItemID();

			p.Position = 73;
			int row = p.ReadByte();
			int line = p.ReadByte();
			if(!inv.craftItem(new Item(craftedItemID)))
			{
				Console.WriteLine("InvCraftItem > Cannot craft item");
				return;
			}

			for(int i = 0;i < providedMaterialIndex.Count;i++)
			{
				if(!inv._decrementItem(providedMaterialIndex[i], providedMaterialQa[i]))
				{
					Console.WriteLine("damn..?");
				}
			}

			if(!inv._decrementItem(manualInventoryIndex))
			{
				Console.WriteLine("damn man, again, wut happend to u?");
			}

			OutPacket op = new OutPacket(168); // 'it has succeded all the checks n stuff now on to kfc.' - cause we all luv Rzaah
			op.WriteInt(168);
			op.WriteShort(0x04);
			op.WriteShort(0x28);
			op.WriteInt(0x01);
			op.WriteInt(chr.getuID());
			op.WriteInt(0x01);
			p.Position = 4;
			op.WriteBytes(p.ReadBytes(68));
			op.WriteInt(1);
			for(int i = 0;i < 8;i++)
			{
				if(providedMaterialIndex.Count > i)
				{
					op.WriteInt(deductedAmount[i] - providedMaterialQa[i]);
				} else op.WriteInt(0);
			}
			/* end_time - TODO:
			 * op.Position = 153;
			 * op.WriteByte(0xff); */
			op.Position = 154;
			p.Position = 73;
			op.WriteShort(p.ReadShort());
			op.WriteInt(craftedItemID);
			/* end_time - TODO:
			 * op.WriteInt(craftedItem.getExpiration()); */
			op.WriteInt(); // meanwhile..
			p.Position = 72;
			op.WriteBytes(p.ReadBytes(4));
			c.WriteRawPacket(op.ToArray());

			inv.saveInv();
		}

		public static void UpgradeItem(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook upgradeItem while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte[] decrypted = p.ReadBytes(10);
			byte oldItemIndex = decrypted[8];
			byte upgraderIndex = decrypted[9];
			Inventory inv=chr.getInventory();

			inv.updateInv();

			List<int> seq = new List<int>(inv.getSeqSaved());
			Dictionary<int, Item> items = new Dictionary<int, Item>(inv.getInvSaved());

			int oldItemHash = seq[oldItemIndex];
			int upgraderHash = seq[upgraderIndex];
			if(oldItemHash == -1 || upgraderHash == -1)
			{
				Console.WriteLine("can't upgrade items.. cause they don't exist o.o");
				return;
			}

			Item oldItem = items[oldItemHash];
			if(oldItem == null)
			{
				Console.WriteLine("Tried to use not existing item..");
				return;
			}
			if(oldItem.getQuantity() > 1)
			{
				Console.WriteLine("wtf..");
				return;
			}
			Item upgrader = items[upgraderHash];
			if(upgrader == null)
			{
				Console.WriteLine("Tried to use not existing item..");
				return;
			}

			Upgrade upgrade = Upgrades.Instance.getUpgradeClasse(oldItem.getItemID(), upgrader.getItemID());
			if(upgrade == null)
			{
				Console.WriteLine("not found dla {0} | {1}", oldItem.getItemID(), upgrader.getItemID());
				return;
			} else Console.WriteLine("znaleziony upgrade: {0} | {1}", upgrade.getOldit(), upgrade.getNewit());
		
			//if(!SkillMaster.canUpgrade(chr, upgrade.getUpgradeskill())){
			//	throw new InventoryException("Cannot upgrade item [your upgrade lvl is too low]");
			//}
		
			byte newItemIndex = 0;
			int newItemHash = -1;
			
			//change item
			oldItem.setItemID(upgrade.getNewit());
			oldItem.setQuantity(1);
			//position of new item
			newItemIndex=oldItemIndex;
			newItemHash = oldItemHash;
		
			//remove upgrader
			inv.removeItem(upgraderIndex, 1);
			if(inv.getSeq()[upgraderIndex] == -1)
			{
				Console.WriteLine("usuwamy itemek");
				c.WriteRawPacket(StaticPackets.getInventoryDeletePacket(chr, upgraderIndex, 1));
			} else Console.WriteLine("co kurwa");

			inv.saveInv();
			c.WriteRawPacket(StaticPackets.getUpgradePacket(chr, oldItem, newItemIndex, newItemHash));
		}

		public static void UseItem(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook useItem while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			p.Skip(1);
			byte usingIndex = p.ReadByte();
			Item item = chr.getInventory().getItemBySeqIndexing(usingIndex);
			if(item == null)
			{
				Logger.LogCheat(Logger.HackTypes.Items, c, "Tried to use not existing item.");
				return;
			}

			ItemData itemData = ItemDataCache.Instance.getItemData(item.getItemID());

			if(!itemData.getClassUsable()[chr.getcClass() - 1])
			{
				Console.WriteLine("not for teh class..");
				return;
			}

			if(itemData.getMinLvl() > chr.getLevel() || itemData.getMaxLvl() < chr.getLevel())
			{
				Console.WriteLine("not for yar level..");
				return;
			}

			if(itemData.getFaction() != 0 && chr.getFaction() != itemData.getFaction())
			{
				Console.WriteLine("not for yah faction..");
				return;
			}

			p.Skip(2);
			MainItemUsage.useItem(chr, item, usingIndex, p);
		}

		public static void Drop(MartialClient c, InPacket p)
		{
		}

		public static void SellToNPC(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook sellToNPC while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte[] decrypted = p.ReadBytes(9);

			Item item = new Item();

			if(!chr.getInventory().sellItem(decrypted[5], decrypted[8], item))
			{
				Console.WriteLine("sell to npc teh problemz");
				return;
			}

			if(payedItems.Contains(item.getItemID()))
			{
				ItemData itemData = ItemDataCache.Instance.getItemData(item.getItemID());
				int itemPrice = itemData.getNpcPrice() * decrypted[8];
				chr.setCoin(chr.getCoin() + itemPrice);
			}

			OutPacket op = new OutPacket(32);
			op.WriteInt(32);
			op.WriteShort(0x04);
			op.WriteShort(0x14);
			op.WriteInt(0x01);
			op.WriteInt(chr.getuID());
			op.WriteShort(0x01);
			op.WriteByte(0x01);
			op.WriteByte(decrypted[5]);
			op.WriteInt(decrypted[8]);
			op.WriteLong(chr.getCoin());
			c.WriteRawPacket(op.ToArray());
		}

		public static void DeleteItem(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook deleteItem while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte[] decrypted = p.ReadBytes(2);
			if(!chr.getInventory().removeItem(decrypted[1]))
			{
				Console.WriteLine("qq cant remove");
				return;
			}

			OutPacket op = new OutPacket(20);
			op.WriteInt(20);
			op.WriteShort(0x04);
			op.WriteShort(0x15);
			op.WriteInt();
			op.WriteInt(chr.getuID());
			op.WriteShort(0x01);
			op.WriteByte(decrypted[0]);
			op.WriteByte(decrypted[1]);
			c.WriteRawPacket(op.ToArray());
		}

		public static void MoveOrUnequip(MartialClient c, InPacket p)
		{
			Console.WriteLine("move or unequip");

			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook invManag while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte[] decrypted = p.ReadBytes(12);
			byte[] amountByte = { decrypted[8], decrypted[9], decrypted[10], decrypted[11] };
			int amount = BitTools.byteArrayToInt(amountByte);

			if(decrypted[0] == (byte)0x00)
			{
				if(!chr.getInventory().unequipItem(decrypted[1], decrypted[4], decrypted[3], chr.getEquipment()))
				{
					Console.WriteLine("problem with unequipItem");
					return;
				}

				WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), CharacterPackets.getExtEquipPacket(chr, decrypted[1], 0));
			}
			else
			{
				if(!chr.getInventory().moveItem(decrypted[1], decrypted[2], amount, decrypted[4], decrypted[3]))
				{
					Console.WriteLine("problem with move item");
					return;
				}
			}

			OutPacket op = new OutPacket(28);
			op.WriteInt(28);
			op.WriteShort(0x04);
			op.WriteShort(0x10);
			op.WriteInt();
			op.WriteInt(c.getAccount().activeCharacter.getuID());
			op.WriteShort(0x01);
			op.WriteBytes(new byte[] { decrypted[0], decrypted[1], decrypted[2], decrypted[3], decrypted[4] });
			op.WriteByte();
			op.WriteBytes(new byte[] { decrypted[8], decrypted[9], decrypted[10], decrypted[11] });
			c.WriteRawPacket(op.ToArray());

			CharacterFunctions.calculateCharacterStatistics(chr);
		}

		public static void Equip(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook equip while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte changeType = p.ReadByte();
			byte[] swapSlots = p.ReadBytes(2);

			if(changeType == (byte)0x00)
			{
				if(!chr.getEquipment().swapEquips(swapSlots[0], swapSlots[1]))
				{
					Logger.LogCheat(Logger.HackTypes.Equip, c, "Attempted to swap weapons, while one of them or even both are null.");
					c.Close();
					return;
				}

				WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), CharacterPackets.getExtEquipPacket(chr, swapSlots[0], chr.getEquipment().getEquipments()[swapSlots[0]].getItemID()));
				WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), CharacterPackets.getExtEquipPacket(chr, swapSlots[1], chr.getEquipment().getEquipments()[swapSlots[1]].getItemID()));
			}
			else
			{
				if(!chr.getInventory().equipItem(swapSlots[0], swapSlots[1], chr.getEquipment()))
				{
					Console.WriteLine("so sorz : >");
					return;
				}

				WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), CharacterPackets.getExtEquipPacket(chr, swapSlots[1], chr.getEquipment().getEquipments()[swapSlots[1]].getItemID()));
			}

			OutPacket op = new OutPacket(24);
			op.WriteInt(24);
			op.WriteShort(0x04);
			op.WriteShort(0x0c);
			op.WriteInt(135593729);
			op.WriteInt(c.getAccount().activeCharacter.getuID());
			op.WriteShort(0x01);
			op.WriteByte(changeType);
			op.WriteBytes(swapSlots);
			c.WriteRawPacket(op.ToArray());

			CharacterFunctions.calculateCharacterStatistics(chr);
		}

		public static void Pickup(MartialClient c, InPacket p)
		{
			byte[] decrypted = p.ReadBytes(7);
			byte[] uid = new byte[4];
			for(int i = 0;i < 4;i++)
			{
				uid[i] = decrypted[i];
			}
			int uID = BitTools.byteArrayToInt(uid);
			int col = (int)decrypted[4] & 0xFF;
			int row = (int)decrypted[5] & 0xFF;

			Item item = null;
			if(WMap.Instance.items.ContainsKey(uID))
			{
				item = WMap.Instance.items[uID];
			}

			if(item == null)
			{
				Console.WriteLine("item null qq");
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			if(!chr.getInventory().pickItem(item, item.getQuantity()))
			{
				Console.WriteLine("InvManagement > Cannot pick item [coin limit]");
				return;
			}

			c.WriteRawPacket(StaticPackets.getPickItemPacket(chr, item, item.getQuantity(), uID, decrypted[4], decrypted[5], decrypted[6]));

			Console.WriteLine("amount {0}", item.getQuantity());

			WMap.Instance.getGrid(c.getAccount().activeCharacter.getMap()).sendTo3x3AreaRemoveItem(chr.getArea(), uID);

			WMap.Instance.items.Remove(uID);
		}

		public static void BuyFromNPC(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.CreateCharacter, c, "Attempted to hook a NPC open while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte[] decrypted = p.ReadBytes(9);

			int npcID = BitConverter.ToInt32(decrypted, 0);

			if(WMap.Instance.getNpcs().ElementAtOrDefault(npcID) == null)
			{
				Logger.LogCheat(Logger.HackTypes.NPC, c, "Tried to hook NPC of uID {0}", npcID);
				return;
			}

			NPC npc = WMap.Instance.getNpcs()[npcID];

			OutPacket op = new OutPacket(56);
			op.WriteInt(56);
			op.WriteShort(4);
			op.WriteShort(19);
			op.WriteInt(1);
			op.WriteInt(c.getAccount().activeCharacter.getuID());

			if(decrypted[8] != 0)
			{
				int itemID = NPCDataCache.Instance.getNPCDataByuID(npcID, chr.getMap()).getItemFromSlot(decrypted[4]);
				if(itemID == -1)
				{
					Console.WriteLine("Somebody tried to buy not existing item");
					return;
				}

				ItemData itemData = ItemDataCache.Instance.getItemData(itemID);
				if(itemData == null)
				{
					Console.WriteLine("Tried to buy not existing item");
					return;
				}

				Item item = new Item(itemID, decrypted[8]);

				int itemPrice = 0;
				if(payedItems.Contains(itemData.getID()))
				{
					itemPrice = itemData.getNpcPrice() * decrypted[8];
					if(chr.getCoin() < itemPrice)
					{
						Console.WriteLine("Tried to buy free Gold Bars :3");
						return;
					}
				}

				Console.WriteLine("line {0} row {1}", decrypted[7], decrypted[6]);
				if(!chr.getInventory().buyItem(item, decrypted[7], decrypted[6]))
				{
					Console.WriteLine("npc.. something went wrong");
					return;
				}

				chr.setCoin(chr.getCoin() - itemPrice);

				op.WriteLong(chr.getCoin());
				op.WriteShort(0x01);
				op.WriteByte(decrypted[5]);
				op.WriteByte(decrypted[6]);
				op.WriteByte(decrypted[7]);
				op.WriteByte(chr.getVp()); // vending points (?)
				op.WriteZero(18);
				op.WriteInt(itemID);
				op.WriteByte(decrypted[8]);
			}
			else
			{
				op.WriteInt(npcID);
				op.WriteInt();
				op.WriteInt(0x01);
				op.WriteInt();
				op.WriteInt();
				op.WriteInt(0); // -100% extra charge => free buying
				op.WriteInt(8388608); // -100% discount => free selling
				op.WriteByte(0x80);
				op.WriteByte(0x3f);
				//op.WriteLong(1294138); // looks like.. areaID? for sure not modelID, or other shit, just the second short looks familiar
				//op.WriteLong(1); // must be 1 to open the shop o.o
				//op.WriteInt(64); // 0% ?
				//op.WriteInt(1065353216); // -100% extra charge => free buying
				//op.WriteInt(1065353216); // -100% discount => free selling

				/* IN TWO WORDS -> WEIRD SHIT HERE */
			}

			c.WriteRawPacket(op.ToArray());
		}
	}
}

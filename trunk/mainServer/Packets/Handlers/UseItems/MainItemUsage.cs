using System;
using System.Text.RegularExpressions;
using gameServer.Core.IO;
using gameServer.Game;
using gameServer.Game.Caches;
using gameServer.Game.Misc;
using gameServer.Game.Objects;
using gameServer.Game.World;
using gameServer.Tools;
using gameServer.Servers;

namespace gameServer.Packets.Handlers.UseItem
{
	class MainItemUsage
	{
		// Holy Grail ftw
		public static void useItem(Character chr, Item item, byte usingIndex, InPacket p)
		{
			MartialClient c = chr.getAccount().mClient;
			ItemData itemData = ItemDataCache.Instance.getItemData(item.getItemID());
			Boolean shouldDecrease = false;
			string determined = null;
			int determiner = 0;
			if(itemData.getIsStackable()) shouldDecrease = true;
			else
			{
				if(itemData.getTimeToExpire() == 0)
					shouldDecrease = true;
			}

			// well.. we don't care if it's handled by server.. let's just remove them & fuck haterz! qq
			if(shouldDecrease)
				if(!chr.getInventory().decrementItem(usingIndex))
				{
					Console.WriteLine("something went wrong with decrement..");
					return;
				}

			switch(itemData.getCategory())
			{
				case 1001: // healingz
				{
					if(itemData.getHealHP() > 0 || itemData.getHealMana() > 0 || itemData.getHealStamina() > 0)
						StaticPackets.releaseHealPacket(chr, (int)(chr.getCurHP() + itemData.getHealHP()), (short)(chr.getCurMP() + itemData.getHealMana()), (short)(chr.getCurSP() + itemData.getHealStamina()));
					break;
				}
				case 1002: // skillz o.o
				{
					StaticPackets.sendSystemMessageToClient(chr.getAccount().mClient, 1, "If you'd like to learn any skill, go to skills list and press CTRL+LMB.");
					break;
				}
				case 1003: // teleport
				{
					if(chr.getMap() == itemData.getTeleportMap() || chr.getMap() != itemData.getTeleportMap() && itemData.getSpecialEffect() != 0)
						CharacterFunctions.setPlayerPosition(chr, itemData.getTeleportX(), itemData.getTeleportY(), (short)itemData.getTeleportMap());
					break;
				}
				case 1007: // reset skills
				{
					chr.getSkills().resetAll();
					chr.getSkillBar().getSkillBar().Clear();
					break;
				}
				case 1011: // effect potions
				{
					chr.setEffect((byte)itemData.getSpecialEffect());
					break;
				}
				case 1012: // tae potion
				{
					break;
				}
				case 1013: // faction change
				{
					if(chr.getFaction() == 0)
						return;

					chr.setFaction(chr.getFaction() == 1 ? (byte)2 : (byte)1);
					break;
				}
				case 1015: // chuk amulet
				{
					determiner = BitConverter.ToInt32(p.ReadBytes(4), 0);
					if(determiner == 0) return;
					ItemData determinedItem = ItemDataCache.Instance.getItemData(determiner);
					if(determinedItem == null || determinedItem.getCategory() != 1003 || (determiner < 212100146 && determiner > 212100164 && determiner != 212100185 && determiner != 212100187))
					{
						Console.WriteLine("I CAN'T TURN 10 INTO 20 CHICKENZ");
						return;
					}
					CharacterFunctions.setPlayerPosition(chr, determinedItem.getTeleportX(), determinedItem.getTeleportY(), (short)determinedItem.getTeleportMap());
					break;
				}
				case 1016: // karma amulet
				{
					chr.setKarmaMessagingTimes((short)(chr.getKarmaMessagingTimes()+1));
					break;
				}
				case 1020: // name changer
				{
					p.Skip(4);
					string charName = MiscFunctions.obscureString(p.ReadString(16));
					if(charName.Length < 3 || Regex.Replace(charName, "[^A-Za-z0-9]+", "") != charName || MySQLTool.NameTaken(charName))
					{
						StaticPackets.sendSystemMessageToClient(chr.getAccount().mClient, 1, "Wrong input " + charName + ".");
						return;
					}

					chr.setName(charName);
					determined = charName;

					CharacterFunctions.refreshCharacterForTheWorld(chr);
					break;
				}
				case 1021: // face changer
				{
					chr.setFace((byte)itemData.getSpecialEffect());
					break;
				}
				case 1024:
				{
					// yy..?
					break;
				}
				case 1031: // red castle
				{
					determiner = BitConverter.ToInt32(p.ReadBytes(4), 0);
					if(determiner == 0) return;
					ItemData determinedItem = ItemDataCache.Instance.getItemData(determiner);
					if(determinedItem == null || determinedItem.getCategory() != 56 || ((determiner < 273001255 && determiner > 273001257) && determiner != 283000472 && determiner != 283000543 && determiner != 283000575 && determiner != 283000614 && determiner != 283000934 && determiner != 283001078 && determiner != 283001373 && determiner != 283001376))
					{
						Console.WriteLine("I CAN'T TURN 10 INTO 20 CHICKENZ");
						return;
					}
					CharacterFunctions.setPlayerPosition(chr, determinedItem.getTeleportX(), determinedItem.getTeleportY(), (short)determinedItem.getTeleportMap());
					break;
				}
				default:
				{
					StaticPackets.sendSystemMessageToClient(chr.getAccount().mClient, 1, "Feature not implemented yet");
					return;
				}
			}

			OutPacket op = new OutPacket(52);
			op.WriteInt(52);
			op.WriteShort(0x04);
			op.WriteShort(0x05);
			op.WriteInt(140328705);
			op.WriteInt(chr.getuID());
			op.WriteShort(0x01);
			op.WriteByte(0x01);
			op.WriteByte(usingIndex);
			op.WriteInt(item.getQuantity());
			op.WriteInt(793149441);
			op.WriteInt(/*determiner > 0 ? determiner : 0*/);
			op.WritePaddedString(determined, 17);
			op.WriteByte(0x90);
			op.WriteByte(0xd2);
			op.WriteByte(0x2a);
			c.WriteRawPacket(op.ToArray());

			OutPacket ops = new OutPacket(40);
			ops.WriteInt(40);
			ops.WriteShort(0x05);
			ops.WriteShort(0x05);
			ops.WriteInt(779458561);
			ops.WriteInt(chr.getuID());
			ops.WriteInt(item.getItemID());
			ops.WritePaddedString(determined, 17);
			ops.WriteByte(0x9e);
			ops.WriteByte(0x0f);
			ops.WriteByte(0xbf);
			WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), ops.ToArray());
		}
	}
}

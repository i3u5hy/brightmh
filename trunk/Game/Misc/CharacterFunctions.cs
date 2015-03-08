using System;
using System.Collections.Generic;
using System.Linq;
using gameServer.Game.Caches;
using gameServer.Game.Objects;
using gameServer.Game.World;
using gameServer.Packets;
using gameServer.Servers;
using gameServer.Tools;
using gameServer.Core.IO;
using MySql.Data.MySqlClient;

namespace gameServer.Game.Misc {
	public static class CharacterFunctions {
		public static void warpToNearestTown(Character chr)
		{
			Waypoint closestTown = TownCoordsCache.Instance.getClosestWaypointForMap(chr.getMap(), new Waypoint(chr.getPosition()[0], chr.getPosition()[1]));
			if(closestTown == null)
			{
				Area vvArea = WMap.Instance.getGrid(1).getAreaByRound(-1660, 2344);
				if(vvArea == null)
				{
					Logger.WriteLog(Logger.LogTypes.Error, "Pure warpToNearestTown error {0}|{1}|{2}", chr.getPosition()[0], chr.getPosition()[1], chr.getMap());
					StaticPackets.sendSystemMessageToClient(chr.getAccount().mClient, 1, "We're sorry, but an hard error has occured. Please report it to an admin.");
					chr.getAccount().mClient.Close();
					return;
				} setPlayerPosition(chr, -1660, 2344, 1);
			}
		}

		public static void refreshCharacterForTheWorld(Character chr)
		{
			WMap.Instance.getGrid(chr.getMap()).sendTo3x3AreaLeave(chr, chr.getArea());
			WMap.Instance.getGrid(chr.getMap()).sendTo3x3AreaSpawn(chr, chr.getArea(), true);
		}

		public static void setPlayerPosition(Character chr, float goX, float goY, short map)
		{
			MartialClient c = chr.getAccount().mClient;

			Logger.WriteLog(Logger.LogTypes.Debug, goX + " | " + goY + " | " + map);

			Area lastArea = chr.getArea();
			Area newArea = WMap.Instance.getGrid(map).getAreaByRound(goX, goY);

			if(newArea == null)
			{
				StaticPackets.sendSystemMessageToClient(chr.getAccount().mClient, 1, "The position " + goX + "|" + goY + "|" + map + " can't be reached.");
				Waypoint closestTown = TownCoordsCache.Instance.getClosestWaypointForMap(map, new Waypoint(goX, goY));
				if(closestTown == null)
				{
					Area vvArea = WMap.Instance.getGrid(1).getAreaByRound(-1660, 2344);
					if(vvArea == null)
					{
						Logger.WriteLog(Logger.LogTypes.Error, "Pure setPlayerPosition error {0}|{1}|{2}", goX, goY, map);
						StaticPackets.sendSystemMessageToClient(chr.getAccount().mClient, 1, "We're sorry, but an hard error has occured. Please report it to an admin.");
						c.Close();
						return;
					} else {
						goX = -1660;
						goY = 2344;
						map = 1;
						newArea = vvArea;
					}
				} else {
					goX = closestTown.getX();
					goY = closestTown.getY();
					newArea = WMap.Instance.getGrid(map).getAreaByRound(goX, goY);
				}
			}

			if(lastArea != null)
			{
				WMap.Instance.getGrid(chr.getMap()).sendTo3x3AreaLeave(chr, lastArea);
				lastArea.removeCharacter(chr);
			}

			if(newArea != null)
				chr.setArea(newArea);
			else
			{
				chr.getAccount().mClient.Close();
				return;
			}

			newArea.addCharacter(chr);

			chr.setMap(map);
			chr.setPosition(new float[] { goX, goY });

			OutPacket op = new OutPacket(5840);
			op.WriteInt(5824);
			op.WriteShort(4); // 4 - 5
			op.WriteShort(1); // 6 - 7
			op.WriteInt(1); // 8 - 11
			op.WriteInt(chr.getuID()); // 12 - 15
			op.WriteShort(1); // 16 - 19
			op.WriteShort(1); // 16 - 19
			op.WriteInt(chr.getMap()); // 20 - 23
			op.WriteInt(DateTime.Now.Year - 2000); // 24 - 27
			op.WriteByte((byte)DateTime.Now.Month); // 28
			op.WriteByte(DateTime.Now.Day > 30 ? (byte)0x1e : (byte)DateTime.Now.Day); // 29
			op.WriteInt(DateTime.Now.Hour); // 30 - 37

			for(int i=0;i<120;i++) {
				if(chr.getCargo().getSeqSaved()[i] != -1 && chr.getCargo().getCargoSaved()[chr.getCargo().getSeqSaved()[i]] != null) {
					op.WriteInt();
					op.WriteByte((byte)(chr.getCargo().getSeqSaved()[i] / 100));
					op.WriteByte((byte)(chr.getCargo().getSeqSaved()[i] % 100));
					Item item = chr.getCargo().getCargoSaved()[chr.getCargo().getSeqSaved()[i]];
					op.WriteInt(item.getItemID());
					ItemData itemData = ItemDataCache.Instance.getItemData(item.getItemID());
					if(itemData.getTimeToExpire() > 0) {

					}
					op.WriteShort(item.getQuantity());
				} else op.WriteZero(12);
			} // 38 - 1477

			op.Position = 1476;

			for(int i=0;i<chr.getCommunity().getFriendsList().Capacity;i++) {
				if(chr.getCommunity().getFriendsList().ElementAtOrDefault(i) != null) {
					op.WritePaddedString(chr.getCommunity().getFriendsList()[i], 17);
				} else op.WriteZero(17);
			} // 1476 - 1934

			op.WriteRepeatedByte(0x58, 40);

			op.Position = 1986;

			for(int i = 0;i < chr.getCommunity().getIgnoresList().Capacity;i++) {
				if(chr.getCommunity().getIgnoresList().ElementAtOrDefault(i) != null) {
					op.WritePaddedString(chr.getCommunity().getIgnoresList()[i], 17);
				} else op.WriteZero(17);
			} // 1987 - 2157

			op.WriteInt(363); // questsy
			op.WriteLong();
			op.WriteLong(138769276674441706);
			op.WriteLong(21692910);
			op.WriteShort();
			op.WriteShort(1);

			op.Position = 2248;

			for(byte i=0;i<240;i++) {
				if(chr.getInventory().getSeqSaved()[i] != -1 && chr.getInventory().getInvSaved()[chr.getInventory().getSeqSaved()[i]] != null) {
					op.WriteShort();
					op.WriteByte((byte)(chr.getInventory().getSeqSaved()[i] / 100));
					op.WriteByte((byte)(chr.getInventory().getSeqSaved()[i] % 100));
					Item item = chr.getInventory().getInvSaved()[chr.getInventory().getSeqSaved()[i]];
					op.WriteInt(item.getItemID());
					op.WriteInt(item.getQuantity());
				} else op.WriteZero(12);
			} // 2252 - 5133

			op.WriteLong(chr.getCoin());

			op.Position = 5140;

			for(byte i=0;i<21;i++) {
				if(chr.getSkillBar().getSkillBar().ContainsKey(i)) {
					int barID = chr.getSkillBar().getSkillBar()[i];
					if(barID > 200000000) op.WriteInt(1);
					else if(barID > 511) { op.WriteInt(5); barID -= 512; }
					else if(barID > 255) { op.WriteInt(6); barID -= 256; }
					else {
						SkillData skill = SkillDataCache.Instance.getSkill(chr.getSkills().getLearnedSkills().ElementAtOrDefault(barID));
						if(skill == null) op.WriteInt(0);
						else if(skill.getTypeSpecific() == 6) op.WriteInt(3);
						else if(skill.getTypeSpecific() == 7) op.WriteInt(4);
						else op.WriteInt(2);
					}
					op.WriteInt(barID);
				} else op.WriteZero(8);
			} // 5140 - 5299

			op.Position = 5320;

			for(int i = 0;i < 60;i++) {
				if(chr.getSkills().getLearnedSkills().Count > i && chr.getSkills().getLearnedSkills()[i] != 0) {
					op.WriteInt(chr.getSkills().getLearnedSkills()[i]);
					op.WriteInt(SkillDataCache.Instance.getSkill(chr.getSkills().getLearnedSkills()[i]).getSkillPoints());
				} else op.WriteLong();
			} // 5320 - 5799

			op.WriteFloat(chr.getPosition()[0]);
			op.WriteFloat(chr.getPosition()[1]);
			op.WriteInt(0x0c);
			op.WriteInt(140338688);
			op.WriteInt();
			op.WriteShort();
			op.WriteShort(10962);

			//s3c0nd p4ck3t
			op.WriteInt(16);
			op.WriteInt(7929861);
			op.WriteInt(chr.getuID());
			c.WriteRawPacket(op.ToArray());

			WMap.Instance.getGrid(chr.getMap()).sendTo3x3AreaSpawn(chr, chr.getArea());
		}

		public static int createInventories(Character chr)
		{
			Inventory inv = chr.getInventory();
			inv.setPages(chr.getInvPages());
			List<int> seqhash = new List<int>();
			for(int i = 0;i <= 240;i++) seqhash.Add(-1);
			inv.saveInv();
			inv.setSeqSaved(seqhash);

			foreach(int[] entry in NewInventory.Instance.newInventory)
			{
				chr.getInventory().addItem(entry[0], (short)entry[1], new Item(entry[2], (short)entry[3], ItemDataCache.Instance.getItemData(entry[2]).getTimeToExpire()));
			}

			chr.getInventory().saveInv();

			MySQLTool.SaveInventories(chr);
			return 1;
		}

		public static int createEquipments(Character chr)
		{
			switch(chr.getcClass())
			{
				case 1:
				{
					chr.getEquipment().getEquipments().Add(0, new Item(210110101));
					chr.getEquipment().getEquipments().Add(1, new Item(207114101));
					chr.getEquipment().getEquipments().Add(3, new Item(202110103));
					chr.getEquipment().getEquipments().Add(4, new Item(203110102));
					chr.getEquipment().getEquipments().Add(6, new Item(209114101));
					chr.getEquipment().getEquipments().Add(7, new Item(201011002));
					chr.getEquipment().getEquipments().Add(9, new Item(208114101));
					chr.getEquipment().getEquipments().Add(10, new Item(208114101));
					chr.getEquipment().getEquipments().Add(11, new Item(206110102));
					break;
				}

				case 2:
				{
					chr.getEquipment().getEquipments().Add(0, new Item(210220101));
					chr.getEquipment().getEquipments().Add(1, new Item(207224101));
					chr.getEquipment().getEquipments().Add(3, new Item(202220103));
					chr.getEquipment().getEquipments().Add(4, new Item(203220102));
					chr.getEquipment().getEquipments().Add(6, new Item(209225101));
					chr.getEquipment().getEquipments().Add(7, new Item(201011008));
					chr.getEquipment().getEquipments().Add(9, new Item(208224101));
					chr.getEquipment().getEquipments().Add(10, new Item(208224101));
					chr.getEquipment().getEquipments().Add(11, new Item(206220102));
					break;
				}

				case 3:
				{
					chr.getEquipment().getEquipments().Add(0, new Item(210130101));
					chr.getEquipment().getEquipments().Add(1, new Item(207134101));
					chr.getEquipment().getEquipments().Add(3, new Item(202130103));
					chr.getEquipment().getEquipments().Add(4, new Item(203130102));
					chr.getEquipment().getEquipments().Add(6, new Item(209135101));
					chr.getEquipment().getEquipments().Add(7, new Item(201011014));
					chr.getEquipment().getEquipments().Add(9, new Item(208134101));
					chr.getEquipment().getEquipments().Add(10, new Item(208134101));
					chr.getEquipment().getEquipments().Add(11, new Item(206130102));
					break;
				}

				case 4:
				{
					chr.getEquipment().getEquipments().Add(0, new Item(210140101));
					chr.getEquipment().getEquipments().Add(1, new Item(207144101));
					chr.getEquipment().getEquipments().Add(3, new Item(202140103));
					chr.getEquipment().getEquipments().Add(4, new Item(203140102));
					chr.getEquipment().getEquipments().Add(6, new Item(209140101));
					chr.getEquipment().getEquipments().Add(7, new Item(201011020));
					chr.getEquipment().getEquipments().Add(9, new Item(208144101));
					chr.getEquipment().getEquipments().Add(10, new Item(208144101));
					chr.getEquipment().getEquipments().Add(11, new Item(206140102));
					break;
				}
			}

			MySQLTool.SaveEquipments(chr);
			return 1;
		}

		

		public static void quitGameWorld(MartialClient c)
		{
			if(c.getAccount().activeCharacter == null)
				return;
			Character chr = c.getAccount().activeCharacter;
			WMap.Instance.getGrid(chr.getMap()).sendTo3x3AreaLeave(chr, chr.getArea());
			WMap.Instance.removeFromCharacters(chr);
			c.getAccount().activeCharacter = null;
		}

		public static bool isCharacterWearingItem(Character character, int itemID) {
			if(itemID == 0)
				return true;
			foreach(KeyValuePair<byte, Item> entry in character.getEquipment().getEquipments()) {
				if(entry.Value.getItemID() == itemID) {
					return true;
				}
			}
			return false;
		}

		public static void calculateCharacterStatistics(Character chr)
		{
			Boolean[] pureHeal = new Boolean[] { chr.getCurHP() == chr.getMaxHP(), chr.getCurMP() == chr.getMaxMP(), chr.getCurSP() == chr.getMaxSP() }; 

			chr.getEquipment().calculateEquipStats();

			short bonusMaxhp = 0;
			//if(bonusAttributes.containsKey("maxhp"))
			//	bonusMaxhp = (Short)bonusAttributes.get("maxhp");

			short bonusDmg = 0;
			//if(bonusAttributes.containsKey("bonusDmg"))
			//	bonusDmg = (Short)bonusAttributes.get("bonusDmg");

			short bonusAtkSucces = 0;
			//if(bonusAttributes.containsKey("bonusAtkSucces"))
			//	bonusAtkSucces = (Short)bonusAttributes.get("bonusAtkSucces");

			short[] stats = new short[5];
			short[] eqstats = chr.getEquipment().getStats();
			for(int i = 0;i < 5;i++)
			{
				stats[i] = (short)(chr.getCStats()[i] + eqstats[i]);
			}
			chr.setMaxHP((int)(30 + bonusMaxhp + chr.getEquipment().getHp() + stats[0] * 2.2 + stats[1] * 2.4 + stats[2] * 2.5 + stats[3] * 1.6 + stats[4] * 1.5));
			chr.setMaxMP((short)(30 + chr.getEquipment().getMana() + stats[0] * 1.4 + stats[1] * 1.7 + stats[2] * 1.5 + stats[3] * 3.5 + stats[4] * 1.5));
			chr.setMaxSP((short)(30 + chr.getEquipment().getStamina() + stats[0] * 0.9 + stats[1] * 1.3 + stats[2] * 1.5 + stats[3] * 1.7 + stats[4] * 1.3));
			chr.setRegHP((short)(stats[2] / 2 + stats[0] / 4));
			chr.setRegMP((short)(stats[3] / 2 + stats[1] / 4));
			chr.setRegSP((short)(stats[4] * 0.1));
			chr.setAtk((int)(chr.getLevel() / 2 + chr.getEquipment().getAtk() + stats[0] * 0.53 + stats[1] * 0.5 + stats[2] * 0.44 + stats[3] * 0.25 + stats[4] * 0.25)
				+ chr.getEquipment().getMaxDmg());
			chr.setDef((int)(chr.getLevel() / 2 + chr.getEquipment().getDeff() + stats[0] * 0.28 + stats[1] * 0.3 + stats[2] * 0.53 + stats[3] * 0.22 + stats[4] * 0.42));
			chr.setMinDmg((int)(bonusDmg + chr.getEquipment().getMinDmg()));
			chr.setMaxDmg((int)(bonusDmg + chr.getEquipment().getMaxDmg()));
			chr.setBasicAtkSuc((int)(stats[0] * 0.5 + stats[1] * 0.6 + stats[2] * 0.3 + stats[3] * 1 + stats[4] * 0.8 + chr.getLevel() * 6));
			chr.setBasicDefSuc((int)(stats[0] * 0.2 + stats[1] * 0.2 + stats[2] * 0.5 + stats[3] * 0.7 + stats[4] * 0.6 + chr.getLevel() * 4));
			chr.setBasicCritRate((int)(stats[0] * 0.1 + stats[1] * 1 + stats[2] * 0.1 + stats[3] * 0.5 + stats[4] * 0.3 + chr.getLevel() * 2) - 300);
			chr.setAdditionalAtkSuc(1000 + bonusAtkSucces);
			chr.setAdditionalDefSuc(500);
			chr.setAdditionalCritRate(2000);
			float atkSucMul = chr.getEquipment().getAtkSucMul();
			float defSucMul = chr.getEquipment().getDefSucMul();
			float critRateMul = chr.getEquipment().getCritRateMul();
			chr.setAtkSuc((int)(chr.getBasicAtkSuc() + chr.getAdditionalAtkSuc() * atkSucMul));
			chr.setDefSuc((int)(chr.getBasicDefSuc() + chr.getAdditionalDefSuc() * defSucMul));
			chr.setCritRate((int)(chr.getBasicCritRate() + chr.getAdditionalCritRate() * critRateMul));
			chr.setCritDmg((short)(chr.getEquipment().getCritDmg() + stats[1] - 10));

			if(chr.getCurHP() > chr.getMaxHP())
				chr.setCurHP(chr.getMaxHP());
			if(chr.getCurMP() > chr.getMaxMP())
				chr.setCurMP(chr.getMaxMP());
			if(chr.getCurSP() > chr.getMaxSP())
				chr.setCurSP(chr.getMaxSP());

			if(chr.getAccount().activeCharacter != null)
			{
				if(chr.getAccount().activeCharacter == chr)
				{
					StaticPackets.releaseHealPacket(chr, pureHeal[0] ? chr.getMaxHP() : chr.getCurHP(),
						pureHeal[1] ? chr.getMaxMP() : chr.getCurMP(),
							pureHeal[2] ? chr.getMaxSP() : chr.getCurSP());
				}
			}

			Console.WriteLine("{0} {1}", chr.getAtk(), chr.getDef());
		}
	}
}

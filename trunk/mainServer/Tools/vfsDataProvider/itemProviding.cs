using System;
using System.IO;
using gameServer.Game.Caches;
using gameServer.Game.Objects;

namespace gameServer.Tools.vfsDataProvider {
	public class itemProviding {
		public static Boolean loadItems() {
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\script\\items.scr", "data\\items.scr");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/items.scr"))
				return false;

			byte meh = 0;
			Boolean was_there_already_a_nletter = false;
			int i = 0, items_count = 0, position = 0, hop = 0, real_length = 0;
			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/items.scr");
			while(position < data.Length) {
				hop = 0;
				for(i = 0;i < 4;i++) {
					meh = data[position + 456 + i];
					if(meh != 0x00) {
						hop = ((i + 1) * (8 * meh));
					}
				}
				real_length = 460 + hop;

				//ID
				int itemID = data[52 + position] + data[53 + position] * 256 + data[54 + position] * 65536 + data[55 + position] * 16777216;
				if(itemID < 200000000 || itemID > 299999999) continue;
				ItemData itemData = new ItemData();
				itemData.setID(itemID);

				string itemName = "";
				was_there_already_a_nletter = false;
				for(int x = 0;x < 52;x++)
				{
					char _byte = Convert.ToChar(data[x + position]);
					if(_byte > 0x20) was_there_already_a_nletter = true;
					if(_byte == 0x20 && !was_there_already_a_nletter) continue;
					if(_byte == 0x22) continue;
					if(_byte < 0x20) break;
					itemName += _byte;
				}
				itemData.setName(itemName);

				//DESCRIPTION
				string itemDescription = "";
				was_there_already_a_nletter = false;
				for(int x = 56;x < 128;x++)
				{
					char _byte = Convert.ToChar(data[x + position]);
					if(_byte > 0x20) was_there_already_a_nletter = true;
					if(_byte == 0x20 && !was_there_already_a_nletter) continue;
					if(_byte == 0x22) continue;
					if(_byte < 0x20) break;
					itemDescription += _byte;
				}
				itemData.setDescription(itemDescription);

				itemData.setBaseID(data[128 + position] + data[129 + position] * 256 + data[130 + position] * 65536 + data[131 + position] * 16777216);
				itemData.setTextureID(data[132 + position] + data[133 + position] * 256 + data[134 + position] * 65536 + data[135 + position] * 16777216);
				itemData.setCategory(data[136 + position] + data[137 + position] * 256 + data[138 + position] * 65536 + data[139 + position] * 16777216);
				itemData.setAgainstType(data[140 + position] + data[141 + position] * 256 + data[142 + position] * 65536 + data[143 + position] * 16777216);
				itemData.setAgainstTypeBonus(data[144 + position] + data[145 + position] * 256 + data[146 + position] * 65536 + data[147 + position] * 16777216);
				itemData.setTypeDMG(data[148 + position] + data[149 + position] * 256 + data[150 + position] * 65536 + data[151 + position] * 16777216);
				itemData.setTypeDMGBonus(data[152 + position] + data[153 + position] * 256 + data[154 + position] * 65536 + data[155 + position] * 16777216);
				itemData.setAtkRange(BitConverter.ToSingle(data, position + 164));
				itemData.setNpcPrice(data[176 + position] + data[177 + position] * 256 + data[178 + position] * 65536 + data[179 + position] * 16777216);
				itemData.setIsStackable(Convert.ToBoolean(data[180 + position]));
				itemData.setMaxStack((short)(Convert.ToByte(itemData.getIsStackable()) * 99 + 1));
				itemData.setIsPermanent(data[181 + position]);
				itemData.setEquipSlot(data[182 + position]);
				itemData.setWidth(data[183 + position]);
				itemData.setHeight(data[184 + position]);
				itemData.setMinLvl(data[185 + position]);
				itemData.setMaxLvl(data[186 + position]);
				itemData.setReqStr(Convert.ToInt16(data[188 + position] + data[189 + position] * 256));
				itemData.setReqDex(Convert.ToInt16(data[190 + position] + data[191 + position] * 256));
				itemData.setReqVit(Convert.ToInt16(data[192 + position] + data[193 + position] * 256));
				itemData.setReqInt(Convert.ToInt16(data[194 + position] + data[195 + position] * 256));
				itemData.setReqAgi(Convert.ToInt16(data[196 + position] + data[197 + position] * 256));
				itemData.setClassUsable(new bool[] { Convert.ToBoolean(data[199 + position]), Convert.ToBoolean(data[200 + position]), Convert.ToBoolean(data[201 + position]), Convert.ToBoolean(data[202 + position]) });
				itemData.setFaction(data[208 + position]);
				itemData.setUpgradeLvl(data[226 + position]);
				itemData.setStr(Convert.ToInt16(data[228 + position] + data[229 + position] * 256));
				itemData.setBonusStr(Convert.ToInt16(data[230 + position] + data[231 + position] * 256));
				itemData.setDex(Convert.ToInt16(data[234 + position] + data[235 + position] * 256));
				itemData.setBonusDex(Convert.ToInt16(data[236 + position] + data[237 + position] * 256));
				itemData.setVit(Convert.ToInt16(data[240 + position] + data[241 + position] * 256));
				itemData.setBonusVit(Convert.ToInt16(data[242 + position] + data[243 + position] * 256));
				itemData.setInte(Convert.ToInt16(data[246 + position] + data[247 + position] * 256));
				itemData.setBonusInt(Convert.ToInt16(data[248 + position] + data[249 + position] * 256));
				itemData.setAgi(Convert.ToInt16(data[252 + position] + data[253 + position] * 256));
				itemData.setBonusAgi(Convert.ToInt16(data[254 + position] + data[255 + position] * 256));
				itemData.setHealHP(Convert.ToInt16(data[260 + position] + data[261 + position] * 256));
				itemData.setLife(Convert.ToInt16(data[264 + position] + data[265 + position] * 256));
				itemData.setBonusLife(Convert.ToInt16(data[268 + position] + data[269 + position] * 256));
				itemData.setHealMana(Convert.ToInt16(data[276 + position] + data[277 + position] * 256));
				itemData.setMana(Convert.ToInt16(data[280 + position] + data[281 + position] * 256));
				itemData.setBonusMana(Convert.ToInt16(data[284 + position] + data[285 + position] * 256));
				itemData.setHealStamina(Convert.ToInt16(data[288 + position] + data[289 + position] * 256));
				itemData.setStamina(Convert.ToInt16(data[292 + position] + data[293 + position] * 256));
				//itemData.setBonuStamina(data[296 + position] + data[297 + position] * 256); does it even exist?
				itemData.setAtkSCS(BitConverter.ToSingle(data, position + 300));
				itemData.setBonusAtkSCS(BitConverter.ToSingle(data, position + 304));
				itemData.setDefSCS(BitConverter.ToSingle(data, position + 312));
				itemData.setBonusDefSCS(BitConverter.ToSingle(data, position + 316));
				itemData.setCritChance(BitConverter.ToSingle(data, position + 324));
				itemData.setBonusCritChance(BitConverter.ToSingle(data, position + 328));
				itemData.setCritDMG(Convert.ToInt16(data[336 + position] + data[337 + position] * 256));
				itemData.setBonusCritDMG(Convert.ToInt16(data[338 + position] + data[339 + position] * 256));
				itemData.setMinDMG(Convert.ToInt16(data[342 + position] + data[343 + position] * 256));
				itemData.setMaxDMG(Convert.ToInt16(data[348 + position] + data[349 + position] * 256));
				itemData.setOffPower(Convert.ToInt16(data[356 + position] + data[357 + position] * 256));
				itemData.setBonusOffPower(Convert.ToInt16(data[356 + position] + data[357 + position] * 256));
				itemData.setDefPower(Convert.ToInt16(data[360 + position] + data[361 + position] * 256));
				itemData.setBonusDefPower(Convert.ToInt16(data[362 + position] + data[363 + position] * 256));
				itemData.setPvpDMGinc(data[368 + position]);
				itemData.setTimeToExpire(data[404 + position] + data[405 + position] * 256);
				itemData.setTeleportMap(data[position + 412]);
				itemData.setTeleportX(BitConverter.ToSingle(data, position + 416));
				itemData.setTeleportY(BitConverter.ToSingle(data, position + 420));
				itemData.setEffectID(data[424 + position] + data[425 + position] * 256 + data[426 + position] * 65536 + data[427 + position] * 16777216);
				itemData.setSetPieces(data[428 + position]);
				itemData.setSpecialEffect(data[432 + position]);
				if(real_length > 464)
				{
					itemData.setBuffIcon(data[460 + position]);
					itemData.setBuffValue(data[464 + position]);
					itemData.setBuffTime((short)(data[462 + position] + data[463 + position] * 256));

					if(real_length > 472)
					{
						itemData.setBuffIcon2(data[468 + position]);
						itemData.setBuffValue2(data[472 + position]);
						itemData.setBuffTime2((short)(data[470 + position] + data[471 + position] * 256));
					}
				}
				
				ItemDataCache.Instance.addItemData(itemData);
				items_count++;

				position += real_length;
			}

			Logger.WriteLog(Logger.LogTypes.Info, "Loaded data for {0} items", items_count);
			return true;
		}
	}
}

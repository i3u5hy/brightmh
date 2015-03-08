using System;

namespace gameServer.Game.Objects {
	public class ItemData {
		private int ID;
		private String name, description;
		private int baseID, textureID, category, againstType, againstTypeBonus, typeDMG, typeDMGBonus;
		private float atkRange;
		private int npcPrice;
		private Boolean isStackable;
		private short maxStack;
		private byte isPermanent, equipSlot, width, height;
		private short minLvl, maxLvl, reqStr, reqDex, reqVit, reqInt, reqAgi;
		private Boolean[] classUsable;
		private byte faction, upgradeLvl;
		private short str, bonusStr, dex, bonusDex, vit, bonusVit, inte, bonusInt, agi, bonusAgi, healHP, life, bonusLife, healMana, mana, bonusMana, healStamina, stamina;
		private float atkSCS, bonusAtkSCS, defSCS, bonusDefSCS, critChance, bonusCritChance;
		private short critDMG, bonusCritDMG;
		private short minDMG, maxDMG, offPower, bonusOffPower, defPower, bonusDefPower;
		private byte pvpDMGinc;
		private int timeToExpire, effectID;
		private byte setPieces, specialEffect;
		private int teleportMap;
		private float teleportX, teleportY;
		private byte buffIcon, buffValue, buffIcon2, buffValue2;
		private short buffTime, buffTime2;

		public ItemData() {

		}

		public void setMaxStack(short maxStack)
		{
			this.maxStack = maxStack;
		}

		public short getMaxStack()
		{
			return this.maxStack;
		}

		public short[] getRequirementStats()
		{
			return new short[] { reqStr, reqDex, reqVit, reqInt, reqAgi }; 
		}

		public int getID() {
			return ID;
		}

		public void setID(int ID) {
			this.ID = ID;
		}

		public String getName() {
			return name;
		}

		public void setName(String name) {
			this.name = name;
		}

		public String getDescription() {
			return description;
		}

		public void setDescription(String description) {
			this.description = description;
		}

		public int getBaseID() {
			return baseID;
		}

		public void setBaseID(int baseID) {
			this.baseID = baseID;
		}

		public int getTextureID() {
			return textureID;
		}

		public void setTextureID(int textureID) {
			this.textureID = textureID;
		}

		public int getCategory() {
			return category;
		}

		public void setCategory(int category) {
			this.category = category;
		}

		public int getAgainstType() {
			return againstType;
		}

		public void setAgainstType(int againstType) {
			this.againstType = againstType;
		}

		public int getAgainstTypeBonus() {
			return againstTypeBonus;
		}

		public void setAgainstTypeBonus(int againstTypeBonus) {
			this.againstTypeBonus = againstTypeBonus;
		}

		public int getTypeDMG() {
			return typeDMG;
		}

		public void setTypeDMG(int typeDMG) {
			this.typeDMG = typeDMG;
		}

		public int getTypeDMGBonus() {
			return typeDMGBonus;
		}

		public void setTypeDMGBonus(int typeDMGBonus) {
			this.typeDMGBonus = typeDMGBonus;
		}

		public float getAtkRange() {
			return atkRange;
		}

		public void setAtkRange(float atkRange) {
			this.atkRange = atkRange;
		}

		public int getNpcPrice() {
			return npcPrice;
		}

		public void setNpcPrice(int npcPrice) {
			this.npcPrice = npcPrice;
		}

		public Boolean getIsStackable() {
			return isStackable;
		}

		public void setIsStackable(Boolean isStackable) {
			this.isStackable = isStackable;
		}

		public byte getIsPermanent() {
			return isPermanent;
		}

		public void setIsPermanent(byte isPermanent) {
			this.isPermanent = isPermanent;
		}

		public byte getEquipSlot() {
			return equipSlot;
		}

		public void setEquipSlot(byte equipSlot) {
			this.equipSlot = equipSlot;
		}

		public byte getWidth() {
			return width;
		}

		public void setWidth(byte width) {
			this.width = width;
		}

		public byte getHeight() {
			return height;
		}

		public void setHeight(byte height) {
			this.height = height;
		}

		public short getMinLvl() {
			return minLvl;
		}

		public void setMinLvl(short minLvl) {
			this.minLvl = minLvl;
		}

		public short getMaxLvl() {
			return maxLvl;
		}

		public void setMaxLvl(short maxLvl) {
			this.maxLvl = maxLvl;
		}

		public short getReqStr() {
			return reqStr;
		}

		public void setReqStr(short reqStr) {
			this.reqStr = reqStr;
		}

		public short getReqDex() {
			return reqDex;
		}

		public void setReqDex(short reqDex) {
			this.reqDex = reqDex;
		}

		public short getReqVit() {
			return reqVit;
		}

		public void setReqVit(short reqVit) {
			this.reqVit = reqVit;
		}

		public short getReqInt() {
			return reqInt;
		}

		public void setReqInt(short reqInt) {
			this.reqInt = reqInt;
		}

		public short getReqAgi() {
			return reqAgi;
		}

		public void setReqAgi(short reqAgi) {
			this.reqAgi = reqAgi;
		}

		public Boolean[] getClassUsable() {
			return classUsable;
		}

		public void setClassUsable(Boolean[] classUsable) {
			this.classUsable = classUsable;
		}

		public byte getFaction() {
			return faction;
		}

		public void setFaction(byte faction) {
			this.faction = faction;
		}

		public byte getUpgradeLvl() {
			return upgradeLvl;
		}

		public void setUpgradeLvl(byte upgradeLvl) {
			this.upgradeLvl = upgradeLvl;
		}

		public short getStr() {
			return str;
		}

		public void setStr(short str) {
			this.str = str;
		}

		public short getBonusStr() {
			return bonusStr;
		}

		public void setBonusStr(short bonusStr) {
			this.bonusStr = bonusStr;
		}

		public short getDex() {
			return dex;
		}

		public void setDex(short dex) {
			this.dex = dex;
		}

		public short getBonusDex() {
			return bonusDex;
		}

		public void setBonusDex(short bonusDex) {
			this.bonusDex = bonusDex;
		}

		public short getVit() {
			return vit;
		}

		public void setVit(short vit) {
			this.vit = vit;
		}

		public short getBonusVit() {
			return bonusVit;
		}

		public void setBonusVit(short bonusVit) {
			this.bonusVit = bonusVit;
		}

		public short getInte() {
			return inte;
		}

		public void setInte(short inte) {
			this.inte = inte;
		}

		public short getBonusInt() {
			return bonusInt;
		}

		public void setBonusInt(short bonusInt) {
			this.bonusInt = bonusInt;
		}

		public short getAgi() {
			return agi;
		}

		public void setAgi(short agi) {
			this.agi = agi;
		}

		public short getBonusAgi() {
			return bonusAgi;
		}

		public void setBonusAgi(short bonusAgi) {
			this.bonusAgi = bonusAgi;
		}

		public short getHealHP() {
			return healHP;
		}

		public void setHealHP(short healHP) {
			this.healHP = healHP;
		}

		public short getLife() {
			return life;
		}

		public void setLife(short life) {
			this.life = life;
		}

		public short getBonusLife() {
			return bonusLife;
		}

		public void setBonusLife(short bonusLife) {
			this.bonusLife = bonusLife;
		}

		public short getHealMana() {
			return healMana;
		}

		public void setHealMana(short healMana) {
			this.healMana = healMana;
		}

		public short getMana() {
			return mana;
		}

		public void setMana(short mana) {
			this.mana = mana;
		}

		public short getBonusMana() {
			return bonusMana;
		}

		public void setBonusMana(short bonusMana) {
			this.bonusMana = bonusMana;
		}

		public short getHealStamina() {
			return healStamina;
		}

		public void setHealStamina(short healStamina) {
			this.healStamina = healStamina;
		}

		public short getStamina() {
			return stamina;
		}

		public void setStamina(short stamina) {
			this.stamina = stamina;
		}

		public float getAtkSCS() {
			return atkSCS;
		}

		public void setAtkSCS(float atkSCS) {
			this.atkSCS = atkSCS;
		}

		public float getBonusAtkSCS() {
			return bonusAtkSCS;
		}

		public void setBonusAtkSCS(float bonusAtkSCS) {
			this.bonusAtkSCS = bonusAtkSCS;
		}

		public float getDefSCS() {
			return defSCS;
		}

		public void setDefSCS(float defFSCS) {
			this.defSCS = defFSCS;
		}

		public float getBonusDefSCS() {
			return bonusDefSCS;
		}

		public void setBonusDefSCS(float bonusDefFSCS) {
			this.bonusDefSCS = bonusDefFSCS;
		}

		public float getCritChance() {
			return critChance;
		}

		public void setCritChance(float critChance) {
			this.critChance = critChance;
		}

		public float getBonusCritChance() {
			return bonusCritChance;
		}

		public void setBonusCritChance(float bonusCritChance) {
			this.bonusCritChance = bonusCritChance;
		}

		public short getCritDMG() {
			return critDMG;
		}

		public void setCritDMG(short critDMG) {
			this.critDMG = critDMG;
		}

		public short getBonusCritDMG() {
			return bonusCritDMG;
		}

		public void setBonusCritDMG(short bonusCritDMG) {
			this.bonusCritDMG = bonusCritDMG;
		}

		public short getMinDMG() {
			return minDMG;
		}

		public void setMinDMG(short minDMG) {
			this.minDMG = minDMG;
		}

		public short getMaxDMG() {
			return maxDMG;
		}

		public void setMaxDMG(short maxDMG) {
			this.maxDMG = maxDMG;
		}

		public short getOffPower() {
			return offPower;
		}

		public void setOffPower(short offPower) {
			this.offPower = offPower;
		}

		public short getBonusOffPower() {
			return bonusOffPower;
		}

		public void setBonusOffPower(short bonusOffPower) {
			this.bonusOffPower = bonusOffPower;
		}

		public short getDefPower() {
			return defPower;
		}

		public void setDefPower(short defPower) {
			this.defPower = defPower;
		}

		public short getBonusDefPower() {
			return bonusDefPower;
		}

		public void setBonusDefPower(short bonusDefPower) {
			this.bonusDefPower = bonusDefPower;
		}

		public byte getPvpDMGinc() {
			return pvpDMGinc;
		}

		public void setPvpDMGinc(byte pvpDMGinc) {
			this.pvpDMGinc = pvpDMGinc;
		}

		public int getTimeToExpire() {
			return timeToExpire;
		}

		public void setTimeToExpire(int timeToExpire) {
			this.timeToExpire = timeToExpire;
		}

		public int getEffectID() {
			return effectID;
		}

		public void setEffectID(int effectID) {
			this.effectID = effectID;
		}

		public byte getSetPieces() {
			return setPieces;
		}

		public void setSetPieces(byte setPieces) {
			this.setPieces = setPieces;
		}

		public byte getSpecialEffect() {
			return specialEffect;
		}

		public void setSpecialEffect(byte specialEffect) {
			this.specialEffect = specialEffect;
		}

		public int getTeleportMap() {
			return teleportMap;
		}

		public void setTeleportMap(int teleportMap) {
			this.teleportMap = teleportMap;
		}

		public float getTeleportX() {
			return teleportX;
		}

		public void setTeleportX(float teleportX) {
			this.teleportX = teleportX;
		}

		public float getTeleportY() {
			return teleportY;
		}

		public void setTeleportY(float teleportY) {
			this.teleportY = teleportY;
		}

		public byte getBuffIcon()
		{
			return buffIcon;
		}

		public void setBuffIcon(byte buffIcon)
		{
			this.buffIcon = buffIcon;
		}

		public byte getBuffIcon2()
		{
			return buffIcon2;
		}

		public void setBuffIcon2(byte buffIcon)
		{
			this.buffIcon2 = buffIcon;
		}

		public byte getBuffValue()
		{
			return buffValue;
		}

		public void setBuffValue(byte buffValue)
		{
			this.buffValue = buffValue;
		}

		public byte getBuffValue2()
		{
			return buffValue2;
		}

		public void setBuffValue2(byte buffValue2)
		{
			this.buffValue2 = buffValue2;
		}

		public short getBuffTime()
		{
			return buffTime;
		}

		public void setBuffTime(short buffTime)
		{
			this.buffTime = buffTime;
		}

		public short getBuffTime2()
		{
			return buffTime2;
		}

		public void setBuffTime2(short buffTime2)
		{
			this.buffTime2 = buffTime2;
		}
	}
}

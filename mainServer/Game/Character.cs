using System;
using System.Collections.Generic;
using System.Text;
using gameServer.Game.Misc;
using gameServer.Game.Objects;
using gameServer.Game.World;
using gameServer.Servers;
using gameServer.Tools;
using MySql.Data.MySqlClient;

namespace gameServer.Game
{
	public sealed class Character : Fightable
	{
		private int cID;
		private Account account;
		private String name;
		private short str, dex, vit, inte, agi, map = 1, statPoints, skillPoints, karmaMessagingTimes, gmShoutMessagingTimes;
		private long coin, exp;
		private int fame, maxHP, curHP, atk, def;
		private short maxMP, maxSP, curMP, curSP;
		private short regHP, regMP, regSP;
		private int minDmg, maxDmg, critDmg;
		private int basicAtkSuc, basicDefSuc, basicCritRate, additionalAtkSuc, additionalDefSuc, additionalCritRate;
		private int atkSuc, defSuc, critRate;
		private int lastHit;
		private float[] position = new float[] { -1660, 2344 };
		private byte level = 1, faction, face = 1, cClass = 1, effect, vp, invPages = 3;
		private Boolean deleteState, trading, death;
		private Area area;
		private List<int> innitedAreas = new List<int>();
		private Equipment equipment;
		private Inventory inventory;
		private Cargo cargo;
		private Skills skills;
		private SkillBar skillBar;
		private Community community;
		private Vending vending;
		private GuildMember guildMember;
		private Guild guild;

		public Character()
		{
			this.equipment = new Equipment(this);
			this.inventory = new Inventory(this);
			this.cargo = new Cargo(this);
			this.skills = new Skills(this);
			this.skillBar = new SkillBar();
			this.community = new Community();
		}

		public bool Create()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("(charName, charClass, face, Inte, Agi, Vit, Str, Dex, statPointz, ownerID) ");
			sb.Append("VALUES (");
			sb.Append("'" + this.name + "',");
			sb.Append(this.cClass + ",");
			sb.Append(this.face + ",");
			sb.Append(this.inte + ",");
			sb.Append(this.agi + ",");
			sb.Append(this.vit + ",");
			sb.Append(this.str + ",");
			sb.Append(this.dex + ",");
			sb.Append(this.statPoints + ",");
			sb.Append(this.account.aID);
			sb.Append(")");
			int charID = MySQLTool.Create("chars", sb);
			this.cID = charID;

			sb.Clear();
			sb.Append("(charID) VALUES (" + charID + ")");
			MySQLTool.Create("chars_eq", sb);

			sb.Clear();
			sb.Append("(charID) VALUES (" + charID + ")");
			MySQLTool.Create("chars_inv", sb);

			sb.Clear();
			sb.Append("(charID) VALUES (" + charID + ")");
			MySQLTool.Create("chars_sbar", sb);

			sb.Clear();
			sb.Append("(charID) VALUES (" + charID + ")");
			MySQLTool.Create("chars_skill", sb);

			sb.Clear();
			sb.Append("(charID) VALUES (" + charID + ")");
			MySQLTool.Create("chars_com", sb);
			return true;
		}

		public int Load(Account acc)
		{
			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "SELECT * FROM chars WHERE charID=" + cID;
				using(var reader = cmd.ExecuteReader())
				{
					reader.Read();

					if(!reader.HasRows)
					{
						return 0;
					}
					else
					{
						name = reader.GetString("charName");
						cClass = reader.GetByte("charClass");
						deleteState = reader.GetBoolean("removeState");
						invPages = reader.GetByte("invPages");
						face = reader.GetByte("face");
						effect = reader.GetByte("effect");
						faction = reader.GetByte("faction");
						level = reader.GetByte("level");
						curHP = reader.GetInt32("curHP");
						curMP = reader.GetInt16("curMP");
						curSP = reader.GetInt16("curSP");
						coin = reader.GetInt64("coin");
						fame = reader.GetInt32("fame");
						setPosition(new float[] { reader.GetFloat("locX"), reader.GetFloat("locY") });
						map = reader.GetInt16("map");
						inte = reader.GetInt16("Inte");
						agi = reader.GetInt16("Agi");
						vit = reader.GetInt16("Vit");
						str = reader.GetInt16("Str");
						dex = reader.GetInt16("Dex");
						statPoints = reader.GetInt16("statPointz");
						skillPoints = reader.GetInt16("skillPointz");
						karmaMessagingTimes = reader.GetInt16("karmaMessagingTimes");
						gmShoutMessagingTimes = reader.GetInt16("gmShoutMessagingTimes");
					}
				}
			}
			return 1;
		}

		public short[] getCStats() {
			return new short[] { this.str, this.dex, this.vit, this.inte, this.agi};
		}

		public void Save()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(" charName = '" + name + "'");
			sb.Append(", charClass = " + cClass);
			sb.Append(", removeState = " + deleteState);
			sb.Append(", invPages = " + invPages);
			sb.Append(", face = " + face);
			sb.Append(", effect = " + effect);
			sb.Append(", faction = " + faction);
			sb.Append(", level = " + level);
			sb.Append(", curHP = " + curHP);
			sb.Append(", curMP = " + curMP);
			sb.Append(", curSP = " + curSP);
			sb.Append(", coin = " + coin);
			sb.Append(", fame = " + fame);
			sb.Append(", locX = '" + position[0]);
			sb.Append("', locY = '" + position[1]);
			sb.Append("', map = " + map);
			sb.Append(", Inte = " + inte);
			sb.Append(", Agi = " + agi);
			sb.Append(", Vit = " + vit);
			sb.Append(", Str = " + str);
			sb.Append(", Dex = " + dex);
			sb.Append(", statPointz = " + statPoints);
			sb.Append(", skillPointz = " + skillPoints);
			sb.Append(", karmaMessagingTimes = " + karmaMessagingTimes);
			sb.Append(", gmShoutMessagingTimes = " + gmShoutMessagingTimes);
			MySQLTool.Save("chars", sb, "charID", this.cID);

			MySQLTool.SaveEquipments(this);
			MySQLTool.SaveInventories(this);
			//MySQLTool.SaveCargo(this);
			MySQLTool.SaveSkills(this);
			MySQLTool.SaveSkillBar(this);
			MySQLTool.SaveCommunities(this);
		}

		public void Delete(byte slot)
		{
			this.account.characters.Remove(slot);
			MySQLTool.Delete("chars", "charID", this.cID);
			MySQLTool.Delete("chars_eq", "charID", this.cID);
			MySQLTool.Delete("chars_inv", "charID", this.cID);
			MySQLTool.Delete("chars_cargo", "charID", this.cID);
			MySQLTool.Delete("chars_skill", "charID", this.cID);
			MySQLTool.Delete("chars_sbar", "charID", this.cID);
		}

		public int getuID() {
			return cID;
		}

		public void setcID(int cID) {
			this.cID = cID;
		}

		public Guild getGuild()
		{
			return this.guild;
		}

		public void setGuild(Guild guild)
		{
			this.guild = guild;
		}

		public Skills getSkills()
		{
			return this.skills;
		}

		public SkillBar getSkillBar()
		{
			return this.skillBar;
		}

		public Account getAccount()
		{
			return this.account;
		}

		public void setAccount(Account account) {
			this.account = account;
		}

		public String getName() {
			return name;
		}

		public void setName(String charName) {
			this.name = charName;
		}

		public byte getcClass() {
			return cClass;
		}

		public void setcClass(byte charClass) {
			this.cClass = charClass;
		}

		public byte getFace() {
			return face;
		}

		public void setFace(byte face) {
			this.face = face;
		}

		public byte getFaction() {
			return faction;
		}

		public void setFaction(byte faction) {
			this.faction = faction;
		}

		public byte getLevel() {
			return level;
		}

		public void setLevel(byte level) {
			this.level = level;
		}

		public int getMaxHP() {
			return maxHP;
		}

		public void setMaxHP(int maxHP) {
			this.maxHP = maxHP;
		}

		public int getCurHP() {
			return curHP;
		}

		public void setCurHP(int curHP) {
			this.curHP = curHP;
		}

		public short getMaxMP() {
			return maxMP;
		}

		public void setMaxMP(short maxMP) {
			this.maxMP = maxMP;
		}

		public short getCurMP() {
			return curMP;
		}

		public void setCurMP(short curMP) {
			this.curMP = curMP;
		}

		public short getMaxSP() {
			return maxSP;
		}

		public void setMaxSP(short maxSP) {
			this.maxSP = maxSP;
		}

		public short getCurSP() {
			return curSP;
		}

		public void setCurSP(short curSP) {
			this.curSP = curSP;
		}

		public short getRegHP()
		{
			return regHP;
		}

		public void setRegHP(short regHP)
		{
			this.regHP = regHP;
		}

		public short getRegMP()
		{
			return regMP;
		}

		public void setRegMP(short regMP)
		{
			this.regMP = regMP;
		}

		public short getRegSP()
		{
			return regSP;
		}

		public void setRegSP(short regSP)
		{
			this.regSP = regSP;
		}

		public int getAtk() {
			return atk;
		}

		public void setAtk(int atk) {
			this.atk = atk;
		}

		public int getDef() {
			return def;
		}

		public void setDef(int def) {
			this.def = def;
		}

		public long getCoin() {
			return coin;
		}

		public Boolean setCoin(long coin)
		{
			if(coin < 0) return false;
			if(coin > long.MaxValue) return false;
			this.coin = coin;
			return true;
		}

		public int getFame() {
			return fame;
		}

		public void setFame(int fame) {
			if(fame > int.MaxValue)
				this.fame = int.MaxValue;
			this.fame = fame;
		}

		public float[] getPosition() {
			return this.position;
		}

		public void setPosition(float[] position) {
			this.position = position;
		}

		public short getMap() {
			return map;
		}

		public void setMap(short map) {
			this.map = map;
		}

		public short getInt() {
			return inte;
		}

		public void setInt(short inte) {
			this.inte = inte;
		}

		public short getAgi() {
			return agi;
		}

		public void setAgi(short agi) {
			this.agi = agi;
		}

		public short getVit() {
			return vit;
		}

		public void setVit(short vit) {
			this.vit = vit;
		}

		public short getDex() {
			return dex;
		}

		public void setDex(short dex) {
			this.dex = dex;
		}

		public short getStr() {
			return str;
		}

		public void setStr(short str) {
			this.str = str;
		}

		public short getStatPoints() {
			return statPoints;
		}

		public void setStatPoints(short statPoints) {
			this.statPoints = statPoints;
		}

		public short getSkillPoints() {
			return skillPoints;
		}

		public void setSkillPoints(short skillPoints) {
			this.skillPoints = skillPoints;
		}

		public byte getVp() {
			return vp;
		}

		public void setVp(byte vp) {
			this.vp = vp;
		}

		/*public void setCargo(Cargo cargo) {
			this.cargo = cargo;
		}

		public Equipment getEquipment() {
			return equipment;
		}

		public void setEquipment(Equipment equipment) {
			this.equipment = equipment;
		}*/

		public long getExp() {
			return exp;
		}

		public void setExp(long exp) {
			this.exp = exp;
		}

		public Boolean getDeleteState() {
			return deleteState;
		}

		public void setDeleteState(Boolean deleteState) {
			this.deleteState = deleteState;
		}

		public Area getArea() {
			return area;
		}

		public void setArea(Area area) {
			this.area = area;
		}

		public byte getEffect() {
			return effect;
		}

		public void setEffect(byte effect) {
			this.effect = effect;
		}

		public List<int> getInnitedAreas()
		{
			return innitedAreas;
		}

		public void setInnitedAreas(List<int> innitedAreas)
		{
			this.innitedAreas = innitedAreas;
		}

		public void addInnitedArea(int areaID)
		{
			this.innitedAreas.Add(areaID);
		}

		public Boolean removeInnitedArea(int innitedArea)
		{
			if (!this.innitedAreas.Contains(innitedArea)) return false;
			return this.innitedAreas.Remove(innitedArea);
		}

		public void removeInnitedAreas()
		{
			this.innitedAreas.Clear();
		}

		public void setEquipment(Equipment equipment) {
			this.equipment = equipment;
		}

		public Equipment getEquipment() {
			return this.equipment;
		}

		public void setInventory(Inventory inventory)
		{
			this.inventory = inventory;
		}

		public Inventory getInventory()
		{
			return this.inventory;
		}

		public void setCargo(Cargo cargo)
		{
			this.cargo = cargo;
		}

		public Cargo getCargo()
		{
			return this.cargo;
		}

		public int getMinDmg()
		{
			return minDmg;
		}

		public void setMinDmg(int minDmg)
		{
			this.minDmg = minDmg;
		}

		public int getMaxDmg()
		{
			return maxDmg;
		}

		public void setMaxDmg(int maxDmg)
		{
			this.maxDmg = maxDmg;
		}

		public int getBasicDefSuc()
		{
			return basicDefSuc;
		}

		public void setBasicDefSuc(int basicDefSuc)
		{
			this.basicDefSuc = basicDefSuc;
		}

		public int getAtkSuc()
		{
			return atkSuc;
		}

		public int getDefSuc()
		{
			return defSuc;
		}

		public int getCritRate()
		{
			return critRate;
		}

		public int getBasicAtkSuc()
		{
			return basicAtkSuc;
		}

		public void setBasicAtkSuc(int basicAtkSuc)
		{
			this.basicAtkSuc = basicAtkSuc;
		}

		public int getAdditionalAtkSuc()
		{
			return additionalAtkSuc;
		}

		public void setAdditionalAtkSuc(int additionalAtkSuc)
		{
			this.additionalAtkSuc = additionalAtkSuc;
		}

		public int getAdditionalDefSuc()
		{
			return additionalDefSuc;
		}

		public void setAdditionalDefSuc(int additionalDefSuc)
		{
			this.additionalDefSuc = additionalDefSuc;
		}

		public int getBasicCritRate()
		{
			return basicCritRate;
		}

		public void setBasicCritRate(int basicCritRate)
		{
			this.basicCritRate = basicCritRate;
		}

		public int getAdditionalCritRate()
		{
			return additionalCritRate;
		}

		public void setAdditionalCritRate(int additionalCritRate)
		{
			this.additionalCritRate = additionalCritRate;
		}

		public void setAtkSuc(int atkSuc)
		{
			this.atkSuc = atkSuc;
		}

		public void setCritRate(int critRate)
		{
			this.critRate = critRate;
		}

		public void setDefSuc(int defSuc)
		{
			this.defSuc = defSuc;
		}

		public int getCritDmg()
		{
			return critDmg;
		}

		public void setCritDmg(int critdmg)
		{
			this.critDmg = critdmg;
		}

		public Boolean isDeath()
		{
			return death;
		}

		public byte getInvPages()
		{
			return invPages;
		}

		public short getKarmaMessagingTimes()
		{
			return this.karmaMessagingTimes;
		}

		public void setKarmaMessagingTimes(short karmaMessagingTimes)
		{
			this.karmaMessagingTimes = karmaMessagingTimes;
		}

		public short getGMShoutMessagingCounts()
		{
			return this.gmShoutMessagingTimes;
		}

		public void setGMShoutMessagingCounts(short gmShoutMessagingTimes)
		{
			this.gmShoutMessagingTimes = gmShoutMessagingTimes;
		}

		public void recDamage(int uid, int dmg)
		{
			this.curHP =- dmg;
			lastHit = uid;
		}

		public Community getCommunity()
		{
			return this.community;
		}

		public void setVending(Vending vending)
		{
			this.vending = vending;
		}

		public Vending getVending()
		{
			return this.vending;
		}
	}
}

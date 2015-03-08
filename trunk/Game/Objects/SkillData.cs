
namespace gameServer.Game.Objects
{
	public class SkillData
	{
		private int dmg;
		private int group;
		private float speed;
		private int needsWepToCast;
		private int ultiSetID;
		private short healCost, manaCost, staminaCost;
		private int targets;

		private bool isCastable;
		private int id;
		private int chClass;
		private int reqSkill1;
		private int reqSkill2;
		private int reqSkill3;
		private int faction;
		private int lvl;
		private int skillPoints;
		private int typeGeneral;
		private int typeSpecific;
		private int stage;
		private short[] effID = new short[3];
		private short[] effDuration = new short[3];
		private short[] effValue = new short[3];

		public SkillData()
		{

		}

		public void setID(int ID)
		{
			this.id = ID;
		}

		public int getID()
		{
			return id;
		}

		public void setGroup(int group)
		{
			this.group = group;
		}

		public int getGroup()
		{
			return this.group;
		}

		public void setIsCastable(bool castable)
		{
			this.isCastable = castable;
		}

		public bool getIsCastable()
		{
			return isCastable;
		}

		public int getChClass()
		{
			return chClass;
		}

		public void setChClass(int chClass)
		{
			this.chClass = chClass;
		}

		public int getReqSkill1()
		{
			return reqSkill1;
		}

		public void setReqSkill1(int reqSkill1)
		{
			this.reqSkill1 = reqSkill1;
		}

		public int getReqSkill2()
		{
			return reqSkill2;
		}

		public void setReqSkill2(int reqSkill2)
		{
			this.reqSkill2 = reqSkill2;
		}

		public int getReqSkill3()
		{
			return reqSkill3;
		}

		public void setReqSkill3(int reqSkill3)
		{
			this.reqSkill3 = reqSkill3;
		}

		public int getFaction()
		{
			return faction;
		}

		public void setFaction(int faction)
		{
			this.faction = faction;
		}

		public int getLvl()
		{
			return lvl;
		}

		public void setLvl(int lvl)
		{
			this.lvl = lvl;
		}

		public int getSkillPoints()
		{
			return skillPoints;
		}

		public void setSkillPoints(int skillPoints)
		{
			this.skillPoints = skillPoints;
		}

		public int getTypeSpecific()
		{
			return typeSpecific;
		}

		public void setTypeSpecific(int typeSpecific)
		{
			this.typeSpecific = typeSpecific;
		}

		public int getTypeGeneral()
		{
			return typeGeneral;
		}

		public void setTypeGeneral(int typeGeneral)
		{
			this.typeGeneral = typeGeneral;
		}

		public int getStage()
		{
			return stage;
		}

		public void setStage(int stage)
		{
			this.stage = stage;
		}

		public void setEffectID(int key, short effId)
		{
			this.effID[key] = effId;
		}

		public short getEffectID(int key)
		{
			return effID[key];
		}

		public void setEffectDuration(int key, short effDuration)
		{
			this.effDuration[key] = effDuration;
		}

		public short getEffectDuration(int key)
		{
			return effDuration[key];
		}

		public void setEffectValue(int key, short effValue)
		{
			this.effValue[key] = effValue;
		}

		public short getEffectValue(int key)
		{
			return effValue[key];
		}

		public int getDmg()
		{
			return dmg;
		}

		public void setDmg(int dmg)
		{
			this.dmg = dmg;
		}

		public int getNeedsWepToCast()
		{
			return needsWepToCast;
		}

		public void setNeedsWepToCast(int needsWepToCast)
		{
			this.needsWepToCast = needsWepToCast;
		}

		public int getUltiSetID()
		{
			return ultiSetID;
		}

		public void setUltiSetID(int ultiSetID)
		{
			this.ultiSetID = ultiSetID;
		}

		public int getHealCost()
		{
			return healCost;
		}

		public void setHealCost(short healCost)
		{
			this.healCost = healCost;
		}

		public short getManaCost()
		{
			return manaCost;
		}

		public void setManaCost(short manaCost)
		{
			this.manaCost = manaCost;
		}

		public short getStaminaCost()
		{
			return staminaCost;
		}

		public void setStaminaCost(short staminaCost)
		{
			this.staminaCost = staminaCost;
		}

		public int getTargets()
		{
			return targets;
		}

		public void setTargets(int targets)
		{
			this.targets = targets;
		}

		public float getSpeed()
		{
			return speed;
		}

		public void setSpeed(float speed)
		{
			this.speed = speed;
		}
	}
}

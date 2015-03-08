
namespace gameServer.Game.World {
	public class MobData
	{
		private short mobID;
		private int level, minAtk, maxAtk, def, maxHP, baseFame, coins, atkSuc, defSuc, critSuc;
		private long baseExp;
		private int[] skills;
		private int deathId;
		private int aggroRange = 30;
		private int followRange = 200;
		private int moveRange = 300;
		private int attackRange = 5;
		private long respawnTime = 10000;
		private int[] grid;
		private int gridID;
		private int waypointCount, waypointHop;
		private int moveSpeed = 50;
		private int waypointDelay = 4;
		private int[] drops;
		private float[] dropchances;

		public MobData(short mobID = 0)
		{
			this.mobID = mobID;
		}
		public int getLevel()
		{
			return level;
		}
		public void setLvl(int lvl)
		{
			this.level = lvl;
		}
		public int getDef()
		{
			return def;
		}
		public void setDef(int defence)
		{
			if(defence > 1000) defence = 1000;
			this.def = defence;
		}
		public long getBaseExp()
		{
			return baseExp;
		}
		public void setBaseExp(long baseExp)
		{
			this.baseExp = baseExp;
		}
		public int getAggroRange()
		{
			return aggroRange;
		}
		public void setAggroRange(int aggroRange)
		{
			this.aggroRange = aggroRange;
		}
		public int getFollowRange()
		{
			return followRange;
		}
		public void setFollowRange(int followRange)
		{
			this.followRange = followRange;
		}
		public int getMoveRange()
		{
			return moveRange;
		}
		public void setMoveRange(int moveRange)
		{
			this.moveRange = moveRange;
		}
		public long getRespawnTime()
		{
			return respawnTime;
		}
		public void setRespawnTime(long respawnTime)
		{
			this.respawnTime = respawnTime;
		}
		public int[] getGrid()
		{
			return grid;
		}
		public void setGrid(int[] grid)
		{
			this.grid = grid;
		}
		public short getmID()
		{
			return mobID;
		}
		public void setmID(short mobID)
		{
			this.mobID = mobID;
		}
		public int getGridID()
		{
			return gridID;
		}
		public void setGridID(int gridID)
		{
			this.gridID = gridID;
		}
		public int getWaypointHop()
		{
			return waypointHop;
		}
		public void setWaypointHop(int waypointHop)
		{
			this.waypointHop = waypointHop;
		}
		public int getWaypointCount()
		{
			return waypointCount;
		}
		public void setWaypointCount(int waypointCount)
		{
			this.waypointCount = waypointCount;
		}
		public int getAttackRange()
		{
			return attackRange;
		}
		public void setAttackRange(int attackRange)
		{
			this.attackRange = attackRange;
		}
		public int getBaseFame()
		{
			return baseFame;
		}
		public void setBaseFame(int baseFame)
		{
			this.baseFame = baseFame;
		}
		public int getMaxHP()
		{
			return maxHP;
		}
		public void setMaxHP(int maxHP)
		{
			this.maxHP = maxHP;
		}
		public int getMoveSpeed()
		{
			return this.moveSpeed;
		}
		public int getWaypointDelay()
		{
			return waypointDelay;
		}
		public void setWaypointDelay(int waypointDelay)
		{
			this.waypointDelay = waypointDelay;
		}
		public int[] getSkills()
		{
			return skills;
		}
		public void setSkills(int[] skills)
		{
			this.skills = skills;
		}
		public int getDeathId()
		{
			return deathId;
		}
		public void setDeathId(int deathId)
		{
			this.deathId = deathId;
		}
		public int getCoins()
		{
			return coins;
		}
		public void setCoins(int coins)
		{
			this.coins = coins;
		}
		public int[] getDrops()
		{
			return drops;
		}
		public void setDrops(int[] drops)
		{
			this.drops = drops;
		}
		public float[] getDropchances()
		{
			return dropchances;
		}
		public void setDropchances(float[] dropchances)
		{
			this.dropchances = dropchances;
		}
		public int getMinAtk()
		{
			return minAtk;
		}
		public void setMinAtk(int minAtk)
		{
			this.minAtk = minAtk;
		}
		public int getMaxAtk()
		{
			return maxAtk;
		}
		public void setMaxAtk(int maxAtk)
		{
			this.maxAtk = maxAtk;
		}
		public int getAtkSuc()
		{
			return atkSuc;
		}
		public void setAtkSuc(int atkSuc)
		{
			this.atkSuc = atkSuc;
		}
		public int getDefSuc()
		{
			return defSuc;
		}
		public void setDefsuc(int defSuc)
		{
			this.defSuc = defSuc;
		}
		public int getCritsuc()
		{
			return critSuc;
		}
		public void setCritsuc(int critSuc)
		{
			this.critSuc = critSuc;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using gameServer.Game.Caches;
using gameServer.Game.World;

namespace gameServer.Game
{
	public class MobController
	{
		private Dictionary<int, Mob> mobs = new Dictionary<int, Mob>();
		private int mobID, mobCount, uidPool;
		private short map;
		private float spawnx, spawny, spawnWidth, spawnHeight;
		private int wpCount = 8, wpHop = 8, respawnTime = 10000;
		private volatile Boolean active;
		private MobData data;
		private BlockingCollection<Mob> activeMobs = new BlockingCollection<Mob>();
		private Boolean isTemp;
		private Boolean onlyStars;
		private float expFactor;

		public MobController(int ID, int Count, int Pool, short Map, float X1, float Y1, float X2, float Y2) {
			this.mobID = ID;
			this.mobCount = Count;
			this.uidPool = Pool;
			this.map = Map;
			this.spawnx = X1;
			this.spawny = Y2;
			this.spawnWidth = X2;
			this.spawnHeight = Y2;
			this.setActive(false);
			this.init();
		}
		private void init() {
			Mob mob = null;
			int uid = uidPool;
			for (int i=0; i < this.mobCount; i++){
				mob = new Mob(this);
				mob.Run();
				mobs.Add(i, mob);
				uid++;
			}
		}
		public Boolean isActive() {
			return active;
		}
		private void setActive(Boolean active) {
			this.active = active;
		}
	
		public void run(){
			
		}
	
		public MobData getData(){
			return this.data;
		}
	
		protected void register(Mob mob) {
			//this.activeMobs.offer(mob);
		}
	
		protected void unregister(Mob mob) {
			//this.activeMobs.Remove(mob);
		}
	
		public void deleteMob(Mob mob) {
			/*this.mobs.remove(mob.getuid());
			unregister(mob);
			this.ticks.deleteMob(mob);
			WMap.Instance.removeMob(mob.getuid());
			MobMaster.deleteTempMob(mob);
			System.out.println("Removed mob: "+mob.getuid());*/
		}
	
		public Boolean getIsTemp() {
			return this.isTemp;
		}
	
		public Boolean getOnlyStars(){
			return this.onlyStars;
		}
	
		public float getExpFactor(){
			return this.expFactor;
		}

		public float getSpawnX()
		{
			return this.spawnx;
		}

		public float getSpawnY()
		{
			return this.spawny;
		}
	
		public float getSpawnWidth()
		{
			return this.spawnWidth;
		}
	
		public float getSpawnHeight()
		{
			return this.spawnHeight;
		}
	
		public int getMap()
		{
			return this.map;
		}
	}
}
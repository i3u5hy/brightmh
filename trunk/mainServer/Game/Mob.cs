using System;
using System.Collections.Generic;
using gameServer.Core.IO;
using gameServer.Tools;

namespace gameServer.Game.World
{
	public class Mob : Fightable
	{
		private MobController mController;
		private Area area;
		private float positionX, positionY;
		private float spawnX, spawnY;
		private int uID;
		private int curHP, maxHP;
		private long letItSnow = 0;
		private Boolean death;
		private int speed = 20, NoMove = 0;
		private int moveRange = 500;
		public long CdInterval, WaypInterval, CdActiveSync, one, CDonWP;
		private Dictionary<int, int> damage = new Dictionary<int,int>();

		public Mob(MobController mController)
		{
			this.mController = mController;
			this.uID = WMap.Instance.mobsCount;
			this.maxHP = mController.getData().getMaxHP();
			this.curHP = maxHP;
			WMap.Instance.mobsCount++;
			this.spawnX = mController.getSpawnX();
			this.spawnY = mController.getSpawnY();
			this.area = WMap.Instance.getGrid(mController.getMap()).getAreaByRound(spawnX, spawnY);
			this.positionX = Randomizer.NextInt(this.area.getGrid().getaSize()) + this.area.getAreaStartingPosition()[0]; // cause I can.
			this.positionY = Randomizer.NextInt(this.area.getGrid().getaSize()) + this.area.getAreaStartingPosition()[1]; // cause I can.
			if(area != null)
				area.addMob(this);
			else
				Console.WriteLine("out of grid kurwa. {0} {1} {2}", mController.getMap(), spawnX, spawnY);
			this.death = false;
		}

		public int getuID()
		{
			return this.uID;
		}

		public void Run()
		{
			/*if(this.death) {			
				if (WMap.distance(this.positionX, this.positionY, this.spawnX, this.spawnY) > this.moveRange){
					//System.out.println(this.uid + " is too far from spawn");
					this.reset(true,false);
				}
				else {
					if (!this.hasNextWaypoint()){
						this.generateNewChain(20);
					}
					Waypoint wp = this.waypoints.pop();
					if(wp != null) {
						this.setLocation(wp);
					}
				}
			}else{
			
				//respawn
				if(MiscFunctions.CurrentTimeMillis()>died+10000){
					death=false;
					reset(true, true);
				}
			}*/
		}

		private void setLocation(Waypoint wp) {
			/*this.setX(wp.getX());
			this.setY(wp.getY());
			this.updateArea();
			this.send(MobPackets.getMovePacket(this.uid, this.location.getX(), this.location.getY()));*/
		}

		private void reset(Boolean sendMove, Boolean resetHp) {
			if(resetHp) {
				this.curHP = mController.getData().getMaxHP();
			}
		}

		public byte[] getInitialPacket()
		{
			OutPacket p = new OutPacket(608);
			p.WriteInt(608);
			p.WriteShort(0x05);
			p.WriteShort(0x03);
			p.WriteInt(0x02);
			p.WriteInt(uID);
			p.WriteZero(48);
			p.WriteInt(this.mController.getData().getmID());
			p.WriteInt();
			p.WriteInt(curHP);
			p.WriteZero(8);
			p.WriteFloat(positionX);
			p.WriteFloat(positionY);
			//Console.WriteLine("mobid {0} mobuid {1} xcord {2} ycord {3} hp {4}", this.mController.getData().getmID(), uID, positionX, positionY, curHP);
			return p.ToArray();
		}

		public static byte[] getMovePacket(int uid, float x, float y)
		{
			byte[] moveBucket = new byte[48];
			byte[] uniqueID = BitTools.intToByteArray(uid);
			byte[] moveX = BitTools.floatToByteArray(x);
			byte[] moveY = BitTools.floatToByteArray(y);

			moveBucket[0] = (byte)moveBucket.Length;
			moveBucket[4] = (byte)0x05;
			moveBucket[6] = (byte)0x0D;
			moveBucket[8] = (byte)0x02;
			moveBucket[9] = (byte)0x10;
			moveBucket[10] = (byte)0xa0;
			moveBucket[11] = (byte)0x36;

			for(int i = 0;i < 4;i++)
			{
				moveBucket[i + 12] = uniqueID[i];
				moveBucket[i + 20] = moveX[i];
				moveBucket[i + 24] = moveY[i];
				moveBucket[i + 28] = moveX[i];
				moveBucket[i + 32] = moveY[i];
			}

			return moveBucket;
		}

		public int getCurHP()
		{
			return this.curHP;
		}

		public byte getLevel()
		{
			return (byte)this.mController.getData().getLevel();
		}

		public int getAtkSuc()
		{
			return this.mController.getData().getAtkSuc();
		}

		public int getDefSuc()
		{
			return this.mController.getData().getDefSuc();
		}

		public int getCritDmg()
		{
			return 0;
		}

		public int getCritRate()
		{
			return this.mController.getData().getCritsuc();
		}

		public int getDef()
		{
			return (byte)this.mController.getData().getDef();
		}

		public string getName()
		{
			return "mob";
		}

		public short getCurMP()
		{
			return (short)0;
		}

		public void recDamage(int uID, int damage)
		{
			curHP -= damage;
			if(!this.damage.ContainsKey(uID))
				this.damage.Add(uID, damage);
			else
				this.damage[uID] = this.damage[uID] + damage;
			if(curHP < 0)
			{
				// kill dat bastard
			}
			else
			{

			}
		}
	}
}

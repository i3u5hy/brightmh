using System;
using System.Collections.Generic;
using gameServer.Game.Caches;

namespace gameServer.Game.Objects
{
	public class Equipment
	{
		private int hp, mana, stamina;
		private short atk, deff, minDmg, maxDmg, critDmg;
		private short[] stats = new short[] { 0, 0, 0, 0, 0 };
		private short[] setStats = new short[] { 0, 0, 0, 0, 0 };
		private float[] typeDmg = new float[] { 0, 0, 0, 0 };
		private float atkSucMul, defSucMul, critRateMul;
		private Dictionary<byte, Item> equipment = new Dictionary<byte, Item>();
		private float speed;
		private Dictionary<int, int> sets;
		private Dictionary<int, int> fullSets;
		private Character owner;

		/*0, cap
		1, neck
		2, cape
		3, jacket
		4, pants
		5, armor
		6, brace
		7, wep 1
		8, wep 2
		9, ring 1
		10, ring 2
		11, shoes
		12, bird
		13, tablet
		14, fame pad
		15, mount
		16, bead*/

		public Equipment(Character chr)
		{
			owner = chr;
			this.setToZero();
		}

		public void addToEquips(byte index, Item item)
		{
			if(equipment.ContainsKey(index))
				equipment.Remove(index);
			equipment.Add(index, item);
		}

		public Item getEquipItem(byte key)
		{
			if(!equipment.ContainsKey(key)) return null;
			return equipment[key];
		}

		public Character getOwner()
		{
			return this.owner;
		}

		public Dictionary<byte, Item> getEquipments()
		{
			return this.equipment;
		}

		public Boolean swapEquips(byte index1, byte index2)
		{
			Item tmp = equipment[index1];
			equipment[index1] = equipment[index2];
			equipment[index2] = tmp;
			return true;
		}

		public void calculateEquipStats()
		{
			setToZero();
			sets = new Dictionary<int, int>();
			fullSets = new Dictionary<int, int>();
		
			for(byte i=0;i<18;i++){
				if(i!=8 && equipment.ContainsKey(i)){
				
					ItemData item = ItemDataCache.Instance.getItemData(equipment[i].getItemID());
					//EQUIPABLE ITEM
					hp+=item.getLife();
					mana+=item.getMana();
					stamina+=item.getStamina();
					minDmg+=item.getMinDMG();
					maxDmg+=item.getMaxDMG();
					atk += Convert.ToInt16(item.getOffPower() + item.getMinDMG());
					deff+=item.getDefPower();
					atkSucMul+=item.getAtkSCS();
					defSucMul+=item.getDefSCS();
					critRateMul+=item.getCritChance();
					short[] itstatbonuses = new short[] { item.getStr(), item.getInte(), item.getVit(), item.getAgi(), item.getDex() };
					for(int j = 0;j < 5;j++) stats[j] += itstatbonuses[j];

					//EQUIPABLE SET ITEM
					int setHash=item.getEffectID();
					if(sets.ContainsKey(setHash)){
						sets[setHash] = sets[setHash]+1;
						//full set
						if(sets[setHash]==item.getSetPieces()) {
							fullSets.Add(setHash, 1);
							for(byte k=0;k<18;k++) {
								if(i!=8 && equipment.ContainsKey(k)) {
									ItemData tmp = ItemDataCache.Instance.getItemData(equipment[k].getItemID());
									if(tmp.getEffectID() == setHash)
									{
										hp += tmp.getBonusLife();
										mana += tmp.getBonusMana();
										// somebody once said.. stamina doesn't exist in set bonuses, topk3k
										atk += tmp.getBonusOffPower();
										deff += tmp.getBonusDefPower();
										atkSucMul += tmp.getBonusAtkSCS();
										defSucMul += tmp.getBonusDefSCS();
										critRateMul += tmp.getBonusCritChance();
										short[] itstatbonuses2 = new short[] { item.getBonusStr(), item.getBonusInt(), item.getBonusVit(), item.getBonusAgi(), item.getBonusDex() };
										for(int j=0;j<5;j++) stats[j]+=itstatbonuses2[j];
									}
								}
							}
						
						}
					}
					else
					{
						sets.Add(setHash, 1);
					}
				}
			}
		}

		public short[] getStats()
		{
			return this.stats;
		}

		private void setToZero()
		{
			this.hp = 0;
			this.mana = 0;
			this.stamina = 0;
			this.atk = 0;
			this.minDmg = 0;
			this.maxDmg = 0;
			this.deff = 0;
			for(int i = 0;i < 5;i++)
				this.stats[i] = 0;
			for(int i = 0;i < 5;i++)
				this.setStats[i] = 0;
			for(int i = 0;i < 4;i++)
			{
				this.typeDmg[i] = 0;
			}
			this.atkSucMul = 0;
			this.defSucMul = 0;
			this.critRateMul = 0;
			this.critDmg = 0;
			this.setSpeed(0);
		}

		public int getHp()
		{
			return hp;
		}

		public int getMana()
		{
			return mana;
		}

		public int getStamina()
		{
			return stamina;
		}

		public short getAtk()
		{
			return atk;
		}

		public short getDeff()
		{
			return deff;
		}

		public short[] getSetStats()
		{
			return setStats;
		}

		public float getAtkSucMul()
		{
			return atkSucMul;
		}

		public float getDefSucMul()
		{
			return defSucMul;
		}

		public float getCritRateMul()
		{
			return critRateMul;
		}
		public float[] getTypeDmg()
		{
			return typeDmg;
		}
		public short getMinDmg()
		{
			return minDmg;
		}
		public void setMinDmg(short minDmg)
		{
			this.minDmg = minDmg;
		}
		public short getMaxDmg()
		{
			return maxDmg;
		}
		public void setMaxDmg(short maxDmg)
		{
			this.maxDmg = maxDmg;
		}
		public Dictionary<int, int> getFullSets()
		{
			return fullSets;
		}
		public short getCritDmg()
		{
			return critDmg;
		}
		public void setCritDmg(short critDmg)
		{
			this.critDmg = critDmg;
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

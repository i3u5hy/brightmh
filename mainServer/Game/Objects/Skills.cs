using System;
using System.Collections.Generic;
using System.Linq;
using gameServer.Game.Caches;

namespace gameServer.Game.Objects
{
	public class Skills
	{
		private List<int> learnedSkills = new List<int>();
		private int basicSkill=0;
		private Character owner;
	
		public Skills(Character owner)
		{
			this.owner = owner;
		}

		public void addToSkills(int ID)
		{
			if(learnedSkills.Contains(ID))
				return;
			learnedSkills.Add(ID);
		}
	
		public bool learnSkill(int ID, bool updateSp)
		{
			if(learnedSkills.Contains(ID))
			{
				Console.WriteLine("character already has dat skill");
				return false;
			}
		
			if(learnedSkills.Count() >= 60)
			{
				Console.WriteLine("so sorz 2many skillz");
				return false;
			}

			SkillData skill = SkillDataCache.Instance.getSkill(ID);
			int[] reqSkill= new int[] { skill.getReqSkill1(), skill.getReqSkill2(), skill.getReqSkill3() };
			for(int i = 0;i < 3;i++) if(reqSkill[i] != 0 && !learnedSkills.Contains(reqSkill[i])) return false;
			if(reqSkill[0] != 0 && (skill.getTypeGeneral() == 27 || skill.getTypeSpecific() == 0 || skill.getTypeSpecific() == 7 || skill.getTypeSpecific() == 11) && learnedSkills.Contains(reqSkill[0]))
			{
				for(int i = 0;i < learnedSkills.Count;i++)
				{
					if(learnedSkills.ElementAtOrDefault(i) == reqSkill[0])
					{
						learnedSkills[i] = ID;
					}
				}
			}
			else
			{
				learnedSkills.Add(ID);
			}


			if(skill.getTypeSpecific()==5)
			{
				basicSkill=ID;
			}

			owner.setSkillPoints((short)(owner.getSkillPoints()-skill.getSkillPoints()));
			return true;
		}
	
		public int getBasicSkill()
		{
			if(basicSkill!=0)
			{
				return basicSkill;
			}
			else
			{
				ItemData it = null;
				if(owner.getEquipment().getEquipments().ContainsKey(7))
					it = ItemDataCache.Instance.getItemData(owner.getEquipment().getEquipments()[7].getItemID());
				if(it != null && it.getCategory() < 13)
				{
					return SkillDataCache.Instance.getStandardBasicSkillId(it.getCategory());
				} else {
					return SkillDataCache.Instance.getWoodenSkillID(owner.getcClass());
				}
			}
		}

		public Character getOwner() {
			return owner;
		}

		public void setOwner(Character owner) {
			this.owner = owner;
		}

		public int getUpgradeSkillStage()
		{
			if(learnedSkills.Contains(100324) || learnedSkills.Contains(200354) || learnedSkills.Contains(300134) || learnedSkills.Contains(400234))
				return 6;
			if(learnedSkills.Contains(100323) || learnedSkills.Contains(200353) || learnedSkills.Contains(300133) || learnedSkills.Contains(400233))
				return 3;
			if(learnedSkills.Contains(100322) || learnedSkills.Contains(200352) || learnedSkills.Contains(300132) || learnedSkills.Contains(400232))
				return 2;
			if(learnedSkills.Contains(100321) || learnedSkills.Contains(200351) || learnedSkills.Contains(300131) || learnedSkills.Contains(400231))
				return 1;
			return 0;
		}

		public List<int> getLearnedSkills()
		{
			if(learnedSkills == null)
				return null;
			return this.learnedSkills;
		}

		public void resetAll()
		{
			this.learnedSkills.Clear();
		}
	}
}

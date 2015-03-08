using System;
using System.Collections.Generic;
using gameServer.Game.Objects;
using gameServer.Tools;
using gameServer.Packets;
using gameServer.Game.World;

namespace gameServer.Game.Caches
{
	public class SkillDataCache
	{
		private static readonly SkillDataCache instance = new SkillDataCache();
		private SkillDataCache() { }

		public static SkillDataCache Instance { get { return instance; } }

		private Dictionary<int, SkillData> skills = new Dictionary<int, SkillData>();
		private List<int> woodenSkills = new List<int> { 0, 121103060, 122206060, 121309060, 121413050 };
		private List<int> knockSkills = new List<int> { 0, 121100050, 122200050, 121300050, 121400050 };
		private List<int> standardBasicSkills = new List<int> { 0, 131101011, 131102011, 131103011, 132204011, 132205011, 132206011, 131307011, 131308011, 131309011, 131411011, 131411011, 131411011 };

		public void addToSkills(int key, SkillData sData)
		{
			skills.Add(key, sData);
		}

		public SkillData getSkill(int key)
		{
			if(!skills.ContainsKey(key))
				return null;
			return skills[key];
		}

		public int getWoodenSkillID(int chClass)
		{
			if(chClass > woodenSkills.Count)
				return 0;
			return this.woodenSkills[chClass];
		}

		public int getStandardBasicSkillId(int wep)
		{
			if(wep > standardBasicSkills.Count)
				return 0;
			return standardBasicSkills[wep];
		}

		public Boolean canLearnSkill(Character chr, int ID)
		{
			if(!this.skills.ContainsKey(ID))
			{
				Console.WriteLine("playa already has dat skill");
				return false;
			}

			SkillData skill = skills[ID];
		
			if(skill.getChClass() != chr.getcClass() && skill.getChClass()!=0){
				Console.WriteLine("Cannot learn skill [wrong character class]");
				return false;
			}
			if(skill.getLvl()>chr.getLevel()){
				Console.WriteLine("Cannot learn skill [lvl too low]");
				return false;
			}
			if(skill.getFaction()!=0 && skill.getFaction()!=chr.getFaction()){
				Console.WriteLine("Cannot learn skill [wrong faction]");
				return false;
			}
			if(skill.getSkillPoints()>chr.getSkillPoints()){
				Console.WriteLine("Cannot learn skill [not enough skillpoints]");
				return false;
			}
			if((!chr.getSkills().getLearnedSkills().Contains(skill.getReqSkill1()) && skill.getReqSkill1() != 0) || (!chr.getSkills().getLearnedSkills().Contains(skill.getReqSkill2()) && skill.getReqSkill2() != 0) || (!chr.getSkills().getLearnedSkills().Contains(skill.getReqSkill3()) && skill.getReqSkill3() != 0))
			{
				Console.WriteLine("Cannot learn skill [you do not have the req skills]");
				return false;
			}
			return true;
		}

		public int getSkillIDFromCast(Character ch, byte decrypted)
		{
			int skillIDInt = 0;
			int key=(int) decrypted;
			if(key <= ch.getSkills().getLearnedSkills().Count){
				skillIDInt = ch.getSkills().getLearnedSkills()[key];
			}else{
				if(decrypted==(byte)0xFF){
					if(ch.getEquipment().getEquipments().ContainsKey(7))
						skillIDInt = woodenSkills[ch.getcClass()];
					else
						skillIDInt = knockSkills[ch.getcClass()];
				}else{
					Console.WriteLine("Cannot cast skill [skill not learned]");
				}
			}
	
			return skillIDInt;
		}

		public float getDmgFactorByClass(Character chr)
		{
			switch(chr.getcClass())
			{
				case 1:
				{
					return 1.2f;
				}
				case 2:
				{
					return 1.1f;
				}
				case 3:
				{
					return 1;
				}
				case 4:
				{
					return 1;
				}
				default:
				{
					return 1;
				}
			}
		}

		public float getDmgFactorByType(int dmgType)
		{
			//GREEN CRIT evil
			if(dmgType == 5)
				return 12;
			//WHITE CRIT
			if(dmgType == 2)
				return 1.5f;
			//MISS
			if(dmgType == 0)
				return 0;

			return 1;
		}

		public byte skillCastDmgTypeCalculations(Fightable chr, Fightable target, bool canCrit)
		{
			byte dmgType;

			//successrate to hit
			if(chr.getAtkSuc()>=target.getDefSuc() || (int)(Randomizer.NextInt(2)/(Math.Pow(2,(float)-(target.getDefSuc()-chr.getAtkSuc())/400f)))==0) {
				if(chr.getCritRate()>=target.getDefSuc() || (int)(Randomizer.NextInt(2)/(Math.Pow(2,(float)-(target.getDefSuc()-chr.getCritRate())/200f)))==0) {
					if(canCrit && (int)(Randomizer.NextInt(2)*50)==0) {
						dmgType=5;
					} else {
						dmgType=2;
					}
				}else{
					dmgType=1;
				}
			}else{
				dmgType=0;
			}
    		return dmgType;
		}

		public int skillCastDmgCalculations(Character chr, int skillId)
		{
			Console.WriteLine("skillDMG {0} | chr.atk {1} | chr.minDMG {2} | chr.maxDMG {3}", getSkill(skillId).getDmg(), chr.getAtk(), chr.getMinDmg(), chr.getMaxDmg());
			int dmgInt = getSkill(skillId).getDmg() + chr.getAtk() + (chr.getMinDmg() + (int)(Randomizer.NextInt(2) * (chr.getMaxDmg() - chr.getMinDmg() + 1)));
			return dmgInt;
		}

		public bool canCastSkill(Character ch, int id) {
			if(!this.skills.ContainsKey(id))
			{
				Console.WriteLine("no key");
				return false;
			}

			if(!this.skills[id].getIsCastable())
			{
				Console.WriteLine("Cannot cast skill [not castable]");
				return false;
			}
		
			SkillData skill = skills[id];
		
			if(skill.getLvl()>ch.getLevel()){
				Console.WriteLine("Cannot cast skill [lvl too low]");
				return false;
			}
			if(skill.getFaction()!=0 && skill.getFaction()!=ch.getFaction()){
				Console.WriteLine("Cannot cast skill [wrong faction]");
				return false;
			}
			if(skill.getNeedsWepToCast()!=46 && skill.getNeedsWepToCast()!=0 && 
			(!ch.getEquipment().getEquipments().ContainsKey(7) || 
			(skill.getNeedsWepToCast()!=55 && skill.getNeedsWepToCast()!=45 && ItemDataCache.Instance.getItemData(ch.getEquipment().getEquipments()[7].getItemID()).getCategory()!=skill.getNeedsWepToCast()))){
				Console.WriteLine("Cannot cast skill [you do not have the req wep]");
				return false;
			}
			if(skill.getManaCost()>ch.getCurMP()){
				Console.WriteLine("Cannot cast skill [not enough mana]");
				return false;
			}
			if(skill.getStaminaCost()>ch.getCurSP()){
				Console.WriteLine("Cannot cast skill [not enough stamina]");
				return false;
			}
			if(skill.getUltiSetID()!=0 && !ch.getEquipment().getFullSets().ContainsKey(skill.getUltiSetID())){
				Console.WriteLine("Cannot cast skill [you do not have the req set]");
				return false;
			}
			return true;
		}
	}
}

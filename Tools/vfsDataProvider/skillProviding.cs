using System;
using System.IO;
using gameServer.Game.Caches;
using gameServer.Game.Objects;

namespace gameServer.Tools.vfsDataProvider
{
	public class skillProviding
	{
		public static Boolean loadSkills()
		{
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\script\\skills.scr", "data\\skills.scr");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/skills.scr"))
				return false;

			int skills_count = 0;

			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/skills.scr");

			byte meh = 0;
			int i = 0, position = 0, hop = 0, real_length = 0;
			while(position < data.Length)
			{
				hop = 0;
				for(i = 0;i < 4;i++)
				{
					meh = data[position + 1496 + i];
					if(meh != 0x00)
					{
						hop = ((i + 1) * (8 * meh));
					}
				}
				real_length = 1500 + hop;

				SkillData sData = new SkillData();

				//ID
				sData.setID(BitConverter.ToInt32(data, position + 0));
				//SKILLGROUP
				sData.setGroup(BitConverter.ToInt32(data, position + 4));
				//CLASS
				sData.setChClass(BitConverter.ToInt16(data, position + 518));
				//STAGE
				sData.setStage(data[position + 520]);
				//REQSKILL1
				sData.setReqSkill1(BitConverter.ToInt32(data, position + 1280));
				//REQSKILL2
				sData.setReqSkill2(BitConverter.ToInt32(data, position + 1284));
				//REQSKILL3
				sData.setReqSkill3(BitConverter.ToInt32(data, position + 1288));
				//SKILLPOINTS
				sData.setSkillPoints(BitConverter.ToInt32(data, position + 1292));
				//LVL
				sData.setLvl(BitConverter.ToInt16(data, position + 1304));
				//SPECIFICTYPE
				sData.setTypeSpecific(BitConverter.ToInt16(data, position + 1306));
				//TARGETS
				sData.setTargets(BitConverter.ToInt16(data, position + 1330));
				//GENERALTYPE
				sData.setTypeGeneral(BitConverter.ToInt16(data, position + 1332));
				//FACTION
				sData.setFaction(BitConverter.ToInt32(data, position + 1340));
				//NEEDSWEPTOCAST
				sData.setNeedsWepToCast(BitConverter.ToInt32(data, position + 1344));
				//ULTISETID
				sData.setUltiSetID(BitConverter.ToInt32(data, position + 1348));
				//ISCASTABLE
				sData.setIsCastable(Convert.ToBoolean(data[position + 1352]));
				//HEALCOST -signed
				sData.setHealCost(BitConverter.ToInt16(data, position + 1368));
				//MANACOST -signed
				sData.setManaCost(BitConverter.ToInt16(data, position + 1370));
				//STAMINACOST -signed
				sData.setStaminaCost(BitConverter.ToInt16(data, position + 1372));
				//DMG
				sData.setDmg(BitConverter.ToInt32(data, position + 1390));
				//SPEED
				sData.setSpeed(BitConverter.ToSingle(data, position + 1408));
				if(real_length >= 1508)
				{
					//EFFID
					sData.setEffectID(0, data[position + 1500]);
					//EFFDURATION
					sData.setEffectDuration(0, data[position + 1502]);
					//EFFVALUE -signed
					sData.setEffectValue(0, data[position + 1504]);
					if(real_length >= 1516)
					{
						//EFFID
						sData.setEffectID(1, data[position + 1508]);
						//EFFDURATION
						sData.setEffectDuration(1, data[position + 1510]);
						//EFFVALUE -signed
						sData.setEffectValue(1, data[position + 1512]);
						if(real_length >= 1524)
						{
							//EFFID
							sData.setEffectID(2, data[position + 1516]);
							//EFFDURATION
							sData.setEffectDuration(2, data[position + 1518]);
							//EFFVALUE -signed
							sData.setEffectValue(2, data[position + 1520]);
						}
					}
				}

				SkillDataCache.Instance.addToSkills(sData.getID(), sData);

				skills_count++;
				position += real_length;
			}

			Logger.WriteLog(Logger.LogTypes.Info, "Loaded data for {0} skills", skills_count);
			return true;
		}
	}
}

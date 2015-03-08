using gameServer.Core.IO;
using gameServer.Game;
using gameServer.Game.Caches;
using gameServer.Game.Objects;
using gameServer.Game.World;
using gameServer.Tools;

namespace gameServer.Packets.Handlers
{
	public class SkillManagement
	{
		public static void ShortcutBar(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook bar skill while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte barIndex = p.ReadByte();
			byte actionID = p.ReadByte();
			p.Skip(2);
			int thingID = p.ReadInt();

			OutPacket op = new OutPacket(24);
			op.WriteInt(24);
			op.WriteShort(4);
			op.WriteShort(17);
			op.WriteInt(135595521);
			op.WriteInt(chr.getuID());
			op.WriteShort(1);
			op.WriteByte(barIndex);
			op.WriteByte(actionID);
			op.WriteInt(thingID);

			if(actionID >= 1 && actionID <= 4)
			{
				chr.getSkillBar().addToSkillBar(barIndex, thingID);
			}
			else if(actionID == 6)
			{
				chr.getSkillBar().addToSkillBar(barIndex, thingID + 256);
			}
			else if(actionID == 0)
			{
				chr.getSkillBar().removeFromSkillBar(barIndex);
			}
			else if(actionID == 5)
			{
				chr.getSkillBar().addToSkillBar(barIndex, thingID + 512);
			}

			c.WriteRawPacket(op.ToArray());
		}

		public static void LearnSkill(MartialClient c, InPacket p)
		{
			Character chr = c.getAccount().activeCharacter;
			byte[] skillNumber = p.ReadBytes(4);
			byte[] skillId = p.ReadBytes(4);

			int skillNumberInt = BitTools.byteArrayToInt(skillNumber);
			int skillIdInt = BitTools.byteArrayToInt(skillId);

			if(!SkillDataCache.Instance.canLearnSkill(chr, skillIdInt))
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "You can't learn this skill! [" + skillIdInt + "]");
				return;
			}
			chr.getSkills().learnSkill(skillIdInt, true);

			byte[] learnskill = SkillPackets.getLearnSkillPacket(chr, skillIdInt, skillNumberInt);
			c.WriteRawPacket(learnskill);
		}

		public static void CastSkill(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Attempted to hook cast skill while not being ingame.");
				c.Close();
				return;
			}

			Character chr = c.getAccount().activeCharacter;

			byte skillBarNumber = p.ReadByte();
			byte skillActivationType = p.ReadByte();
			p.Skip(14);
			byte chartargets = p.ReadByte();
			p.Skip(1);
			byte mobtargets = p.ReadByte();
			if((chartargets + mobtargets) > 8)
				return;
			p.Skip(1);
			int[] targetIds = new int[chartargets + mobtargets];
			for(int i = 0;i < targetIds.Length;i++)
			{
				targetIds[i] = p.ReadInt();
			}
			System.Console.WriteLine("{0} | {1} | {2} | {3} | {4}", skillBarNumber, skillActivationType, chartargets, mobtargets, string.Join(",", targetIds));

			//skillpckt1 is a packet of skilleffects e.g. buffs
			byte[] skillpckt1 = SkillPackets.getSkillEffectOnCharPacket(chr);
			//skillpckt2 is a packet of skill activation, different IDs and DMG
			byte[] skillpckt2 = new byte[52];
			//just for medi and turbo
			byte[] skillpckt3 = new byte[28];

			//SkillID
			byte[] skillid;
			int skillIDInt = SkillDataCache.Instance.getSkillIDFromCast(chr, (byte)(skillBarNumber));
			int skillidNoFake = skillIDInt;
			skillid = BitTools.intToByteArray(skillIDInt);

			//SkillMaster.canCastSkill(cur, skillidInt);
			SkillData skill = SkillDataCache.Instance.getSkill(skillIDInt);

			if(skill == null)
			{
				System.Console.WriteLine("Skill was null");
				return;
			}

			if(skill.getTargets() < chartargets + mobtargets)
			{
				System.Console.WriteLine("tried to hit moar than poss");
				return;
			}

			//TURBO AND MEDI
			if(skill.getTypeSpecific() == 6 || skill.getTypeSpecific() == 7)
			{
				if(skill.getTypeSpecific() == 6)
				{
					skillpckt3 = SkillPackets.getMediPacket(chr, skillIDInt, skillActivationType);
				}
				else
				{
					skillpckt3 = SkillPackets.getTurboPacket(chr, skillIDInt, skillActivationType == (byte)0xc8);
					//if(skillActivationType == (byte)0xc8) chr.setTurboSpeed(((CastableSkill)skill).getSpeed());
					//else chr.setTurboSpeed(0);
				}

				c.WriteRawPacket(skillpckt3);
				WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), skillpckt3);
				return;
			}

			//TARGETS
			int targets = chartargets + mobtargets;

			//COSTS
			chr.setCurHP(chr.getCurHP() - skill.getHealCost());
			chr.setCurMP((short)(chr.getCurMP() - skill.getManaCost()));
			chr.setCurSP((short)(chr.getCurSP() - skill.getStaminaCost()));

			Fightable target;

			//GET MAXIMUM CASTERS DMG
			int dmgInt = SkillDataCache.Instance.skillCastDmgCalculations(chr, skillIDInt);
			int totalDmg;
			int dmgType;

			//GET MAIN SKILL PACKET
			skillpckt2 = SkillPackets.getCastSkillPacket(chr, targets, skillidNoFake, skillActivationType);

			//ADD TARGET STUFF TO THE PACKET
			for(int aoe = 0;aoe < targets;aoe++)
			{
				//GET TARGET
				byte[] targetByteB = BitTools.intToByteArray(targetIds[aoe]);

				target = WMap.Instance.getGrid(chr.getMap()).getFightableNear(chr.getArea(), BitTools.byteArrayToInt(targetByteB));
				if(target == null)
					continue;

				//check for distance
				//canCastToTarget((Location)chr, (Location)target);

				totalDmg = dmgInt;

				//DECREASE DMG BY DEF
				dmgType = SkillDataCache.Instance.skillCastDmgTypeCalculations(chr, target, skill.getTypeSpecific() == 2);
				totalDmg -= target.getDef();

				//CRIT
				if(dmgType == 2 || dmgType == 5)
					totalDmg += chr.getCritDmg();

				//DMG TYPE FACTOR
				totalDmg *= (int)(SkillDataCache.Instance.getDmgFactorByType(dmgType) * SkillDataCache.Instance.getDmgFactorByClass(chr));

				if(totalDmg < 0) totalDmg = 0;

				//ATK THE TARGET FINALLY
				target.recDamage(chr.getuID(), totalDmg);

				int targetId = BitTools.byteArrayToInt(targetByteB);

				//COMPLETE THE TARGET IN THE PACKAGE
				skillpckt2 = SkillPackets.completeCastSkillPacket(skillpckt2, aoe, targetId, target.getCurHP(), target.getCurMP(), -totalDmg, chartargets, dmgType);

				if(chartargets > 0)
					chartargets--;
			}

			//send skill packet to other players
			WMap.Instance.getGrid(chr.getMap()).sendTo3x3Area(chr, chr.getArea(), skillpckt2);

			//send skill packet to client
			c.WriteRawPacket(skillpckt2);

			//effects on char
			c.WriteRawPacket(skillpckt1);
			return;
		}
	}
}

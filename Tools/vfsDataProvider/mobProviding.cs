using System;
using System.IO;
using gameServer.Game.Caches;
using gameServer.Game.World;

namespace gameServer.Tools.vfsDataProvider
{
	class mobProviding
	{
		public static void loadMobs()
		{
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\script\\mobs.scr", "data\\mobs\\mobs.scr");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/mobs/mobs.scr"))
				return;

			int mob_counts = 0;

			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/mobs/mobs.scr");

			int position = 0;
			while(position < data.Length)
			{
				MobData mobData = new MobData(BitConverter.ToInt16(data, position));
				mobData.setMinAtk(BitConverter.ToInt16(data, position + 220));
				mobData.setMaxAtk(BitConverter.ToInt16(data, position + 222));
				mobData.setDef(BitConverter.ToInt16(data, position + 226));
				mobData.setMaxHP(BitConverter.ToInt32(data, position + 228)+10);
				mobData.setAtkSuc(BitConverter.ToInt16(data, position + 232));
				mobData.setDefsuc(BitConverter.ToInt16(data, position + 234));
				mobData.setCoins(BitConverter.ToInt32(data, position + 356));
				mobData.setSkills(new int[] {BitConverter.ToInt32(data, position + 372), BitConverter.ToInt32(data, position + 380), BitConverter.ToInt32(data, position + 388)});
				long exp = (long)(Math.Pow(1.09, mobData.getLevel())/10*mobData.getMaxHP());
				if(mobData.getLevel() > 20) exp += Convert.ToInt64((mobData.getLevel() - 20) * 0.1 * mobData.getMaxHP());
				if(mobData.getLevel() > 30) exp += Convert.ToInt64((mobData.getLevel() - 30) * 0.8 * mobData.getMaxHP());
				if(mobData.getLevel() > 40) exp += (mobData.getLevel() - 40) * 4 * mobData.getMaxHP();
				if(mobData.getLevel() > 55) exp += (mobData.getLevel() - 55) * 5 * mobData.getMaxHP();
				if(mobData.getLevel() > 70) exp += (mobData.getLevel() - 70) * 5 * mobData.getMaxHP();
				if(mobData.getLevel() > 85) exp += (mobData.getLevel() - 85) * 5 * mobData.getMaxHP();
				if(mobData.getLevel() > 100) exp += (mobData.getLevel() - 100) * 5 * mobData.getMaxHP();
				if(exp<1) exp=1;
				mobData.setBaseExp(exp);
				mobData.setBaseFame(mobData.getLevel() * 5);

				MobDataCache.Instance.addMobData(mobData);
				mob_counts++;

				position += 456;
			}

			Logger.WriteLog(Logger.LogTypes.Info, "Loaded data for {0} Mobs", mob_counts);
		}
	}
}

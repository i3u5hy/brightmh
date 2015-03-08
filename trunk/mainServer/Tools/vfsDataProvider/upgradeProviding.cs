using System;
using System.IO;
using gameServer.Game.Misc;
using gameServer.Game.Objects;

namespace gameServer.Tools.vfsDataProvider
{
	class upgradeProviding
	{
		public static Boolean loadUprades()
		{
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\script\\upgradeitems.scr", "data\\upgradeitems.scr");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/upgradeitems.scr"))
				return false;

			int upgrades_count = 0;

			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/upgradeitems.scr");

			int position = 0;
			while(position < data.Length)
			{
				//"ID        , OLDITEM   , UPGRADER  , NEWITEM   , ITEMSTAGE , UPGRADELVL, FAILRATE  , DUNNO     , BREAKOPTN , UPGRSKILL , DUNNO     , ");
				Upgrades.Instance.addItemUpgrading(new Upgrade(BitConverter.ToInt32(data, position+0),
						BitConverter.ToInt32(data, position+4),
							BitConverter.ToInt32(data, position+8),
								BitConverter.ToInt32(data, position+12),
									BitConverter.ToSingle(data, position+16),
										BitConverter.ToSingle(data, position+20),
											BitConverter.ToSingle(data, position+28),
												BitConverter.ToSingle(data, position+32),
													BitConverter.ToInt32(data, position+40)));
				upgrades_count++;
				position += 44;
			}

			Logger.WriteLog(Logger.LogTypes.Info, "Loaded data for {0} upgrades", upgrades_count);
			return true;
		}
	}
}

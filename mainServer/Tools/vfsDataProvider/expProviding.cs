using System;
using System.IO;
using gameServer.Game.Caches;

namespace gameServer.Tools.vfsDataProvider
{
	public class expProviding
	{
		public static Boolean loadExpRelated()
		{
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\script\\users.scr", "data\\users.scr");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/users.scr"))
				return false;

			int expLevels_count = 0;

			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/users.scr");

			long[] lvlCaps = new long[167];
			long[] multips = new long[167];
			for(byte i = 0;i < 255;i++)
			{
				ExpTableCache.Instance.addExpRequirements(i, BitConverter.ToInt32(data, 18520 + (i * 4)), BitConverter.ToInt32(data, 19544 + (i * 4)));
				expLevels_count++;
			}

			Logger.WriteLog(Logger.LogTypes.Info, "Loaded data for {0} level exp requirements", expLevels_count);
			return true;
		}
	}
}

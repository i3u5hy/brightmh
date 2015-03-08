using System;
using System.IO;
using gameServer.Game.Misc;

namespace gameServer.Tools.vfsDataProvider
{
	class fameProviding
	{
		public static Boolean loadFameNicknames()
		{
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\script\\nicktofame.scr", "data\\nicktofame.scr");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/nicktofame.scr"))
				return false;

			int fameName_count = 0;

			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/nicktofame.scr");

			int position = 0;
			while(position < data.Length)
			{
				FameNickNames.Instance.addFameRequirements(data[position], BitConverter.ToInt32(data, position + 8));
				fameName_count++;
				position += 48;
			}

			Logger.WriteLog(Logger.LogTypes.Info, "Loaded data for {0} fame nicknames", fameName_count);
			return true;
		}
	}
}

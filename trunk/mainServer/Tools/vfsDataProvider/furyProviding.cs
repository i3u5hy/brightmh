using System;
using System.IO;
using System.Text.RegularExpressions;
using gameServer.Game.Misc;

namespace gameServer.Tools.vfsDataProvider {
	public class furyProviding {
		public static Boolean loadFury() {
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\script\\angerlevel.txt", "data\\angerlevel.txt");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/angerlevel.txt"))
				return false;

			int fury_count = 0;

			Boolean after_first_line = false;
			StreamReader file = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "data/angerlevel.txt");
			string line;
			while((line = file.ReadLine()) != null) {
				if(!after_first_line) {
					after_first_line = true;
					continue;
				}
				string[] param = Regex.Split(line, "\t");
				AngerLevel.Instance.addAnger(Convert.ToByte(param[0]), Convert.ToInt16(param[1]), Convert.ToInt16(param[2]));
				fury_count++;
			}
			file.Close();

			Logger.WriteLog(Logger.LogTypes.Info, "Loaded data for {0} fury levels", fury_count);
			return true;
		}
	}
}

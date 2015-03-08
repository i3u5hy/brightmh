using System;
using System.IO;
using System.Text.RegularExpressions;
using gameServer.Game.Misc;

namespace gameServer.Tools.externalDataProvider
{
	public class newCharInventories
	{
		public static bool loadNewCharacterInventories()
		{
			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/createNewCharInventory.txt"))
				return false;

			int newInventory_count = 0;

			StreamReader file = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "data/createNewCharInventory.txt");
			string line;
			while((line = file.ReadLine()) != null)
			{
				string[] param = Regex.Split(line, "\t");
				NewInventory.Instance.addItemTonInventory(Convert.ToInt32(param[0]), Convert.ToInt32(param[1]), Convert.ToInt32(param[2]), Convert.ToInt32(param[3]));
				newInventory_count++;
			}
			file.Close();

			Logger.WriteLog(Logger.LogTypes.Info, "Loaded {0} data for new inventory", newInventory_count);
			return true;
		}
	}
}

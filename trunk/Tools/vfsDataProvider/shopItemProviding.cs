using System;
using System.Collections.Generic;
using System.IO;
using gameServer.Game.Objects;

namespace gameServer.Tools.vfsDataProvider
{
	class shopItemProviding
	{
		public static Boolean loadItemShop()
		{
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\script\\citems.scr", "data\\citems.scr");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/citems.scr"))
				return false;

			int itemShop_count = 0;

			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/citems.scr");

			int position = 0;
			Dictionary<int, int> items = null;
			while(position < data.Length)
			{
				items = new Dictionary<int, int>();
				for(int i = 0;i < 10;i++)
				{
					if(BitConverter.ToInt32(data, position + 64 + i * 8) < 200000000 || BitConverter.ToInt16(data, position + 68 + i * 8) == 0)
						continue;
					if(items.ContainsKey(BitConverter.ToInt32(data, position + 64 + i * 8)))
						continue;
					items.Add(BitConverter.ToInt32(data, position + 64 + i * 8), data[position + 68 + i * 8]);
				}
				ItemShop.Instance.addItemData(BitConverter.ToInt32(data, position), new ShopItem(BitConverter.ToInt32(data, position+56), items));
				itemShop_count++;
				position += 1008;
			}

			Logger.WriteLog(Logger.LogTypes.Info, "Loaded data for {0} itemShop items", itemShop_count);
			return true;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using gameServer.Game.Caches;
using gameServer.Game.Objects;

namespace gameServer.Tools.vfsDataProvider
{
	class manualProviding
	{
		public static Boolean loadManuals()
		{
			if(!Constants.VFSSkip)
				vfsDataProvider.Instance.unpackFromVFS("data\\script\\products.scr", "data\\products.scr");

			if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data/products.scr"))
				return false;

			int manuals_count = 0;

			byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "data/products.scr");

			int position = 0;
			while(position < data.Length)
			{
				List<int> materials = new List<int>();
				List<int> quantities = new List<int>();
				List<int> products = new List<int>();
				
				for(int i = 0;i < 8;i++)
				{
					int material = BitConverter.ToInt32(data, position + 40 + (i * 4));
					int quantity = BitConverter.ToInt32(data, position + 72 + (i * 4));
					if(material == 0) break;
					materials.Add(material);
					quantities.Add(quantity);
				}
				for(int i = 0;i < 8;i++)
				{
					int product = BitConverter.ToInt32(data, position + 104 + (i * 4));
					if(product == 0) break;
					products.Add(product);
				}

				if(materials.Count == 0 || products.Count == 0)
				{
					Console.WriteLine("error for: {0}", position);
					position += 212;
					continue;
				}

				ManualDataCache.Instance.addManualData(BitConverter.ToInt32(data, position), new ManualData(materials, quantities, products));
				manuals_count++;

				position += 212;
			}

			Logger.WriteLog(Logger.LogTypes.Info, "Loaded data for {0} manuals", manuals_count);
			return true;
		}
	}
}

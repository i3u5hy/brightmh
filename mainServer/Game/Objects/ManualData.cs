using System.Collections.Generic;
using gameServer.Tools;

namespace gameServer.Game.Objects
{
	public class ManualData
	{
		private List<int> materials;
		private List<int> quantities;
		private List<int> products;

		public ManualData(List<int> materials, List<int> quantities, List<int> products)
		{
			this.materials = new List<int>(materials);
			this.quantities = new List<int>(quantities);
			this.products = new List<int>(products);
		}

		public List<int> getRequiredMaterials()
		{
			return new List<int>(this.materials);
		}

		public List<int> getRequiredQuantities()
		{
			return new List<int>(this.quantities);
		}

		public int getProducedItemID()
		{
			int index = Randomizer.NextInt(products.Count);
			return products[index];
		}
	}
}

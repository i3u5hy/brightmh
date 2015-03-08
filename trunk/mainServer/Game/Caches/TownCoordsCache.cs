using System.Collections.Generic;
using System.Linq;
using gameServer.Game.World;

namespace gameServer.Game.Caches
{
	public class TownCoordsCache
	{
		private static readonly TownCoordsCache instance = new TownCoordsCache();
		private TownCoordsCache() { }
		public static TownCoordsCache Instance { get { return instance; } }

		Dictionary<short, List<Waypoint>> towns = new Dictionary<short, List<Waypoint>>();

		public void addTowns(short mapID, List<Waypoint> towns)
		{
			this.towns.Add(mapID, towns);
		}

		public Waypoint getWaypointAtIndexForMap(short mapID, int index)
		{
			return towns.Values.ElementAtOrDefault(mapID).ElementAtOrDefault(index);
		}

		public Waypoint getClosestWaypointForMap(short mapID, Waypoint location)
		{
			if(!towns.ContainsKey(mapID)) return null;

			List<Waypoint> townsForCertainMap = new List<Waypoint>(towns[mapID]);
			Dictionary<float, Waypoint> distances = new Dictionary<float, Waypoint>();
			for(int i = 0;i < townsForCertainMap.Count;i++)
			{
				distances.Add(WMap.distance(townsForCertainMap[i].getX(), townsForCertainMap[i].getY(), location.getX(), location.getY()), townsForCertainMap[i]);
			}

			distances = distances.OrderBy(k => k.Key).ToDictionary(k => k.Key, k => k.Value);
			if(distances.Count == 0)
				return null;
			return distances.ElementAt(0).Value;
		}
	}
}

using System;
using System.Collections.Generic;

namespace gameServer.Game.World {
	public sealed class NPCData
	{
		private int mID;
		private byte module;
		private string name = "null";
		private List<int> tradeItems = new List<int>();

		public NPCData(int mID) {
			this.mID = mID;
		}

		public int getmID() {
			return this.mID;
		}

		public void setmID(int mID) {
			this.mID = mID;
		}

		public String getName() {
			return name;
		}

		public void setName(String name) {
			this.name = name;
		}

		public void addToItems(int itemID) {
			this.tradeItems.Add(itemID);
		}

		public int getModule() {
			return module;
		}

		public void setModule(byte module) {
			this.module = module;
		}

		public int getItemFromSlot(int slot) {
			if(tradeItems.Count > slot)
				return tradeItems[slot];
			return -1;
		}
	}
}

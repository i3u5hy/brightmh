using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gameServer.Game.Objects
{
	public class Community
	{
		private List<string> friends = new List<string>(30);
		private List<string> ignores = new List<string>(10);

		public Community()
		{
			for(int i = 0;i < friends.Capacity;i++) friends.Add(null);
			for(int i = 0;i < ignores.Capacity;i++) ignores.Add(null);
		}

		public List<string> getFriendsList()
		{
			return this.friends;
		}

		public List<string> getIgnoresList()
		{
			return this.ignores;
		}

		public void relistCommunities()
		{
			List<string> temporary = new List<string>();
			foreach(string i in this.friends)
			{
				if(i == null) continue;
				temporary.Add(i);
			} friends.Clear();
			for(int i = 0;i < friends.Capacity;i++)
				friends.Add(null);
			int x = 0;
			foreach(string i in temporary)
			{
				friends[x] = i;
				x++;
			}

			temporary.Clear();
			foreach(string i in this.ignores)
			{
				if(i == null) continue;
				temporary.Add(i);
			} ignores.Clear();
			for(int i = 0;i < ignores.Capacity;i++)
				ignores.Add(null);
			x = 0;
			foreach(string i in temporary)
			{
				ignores[x] = i;
				x++;
			}
		}

		public void addPersona(byte type, string name)
		{
			if(type == 0 && friends.Count < friends.Capacity && !friends.Contains(name))
				for(int i=0;i<friends.Capacity;i++) if(friends[i] == null) friends[i] = name;
			else if(type == 1 && ignores.Count < ignores.Capacity && !ignores.Contains(name))
				for(i = 0;i < ignores.Capacity;i++) if(ignores[i] == null) ignores[i] = name;
		}

		public Boolean addPersona(byte type, byte index, string name)
		{
			if(type == 0 && index < friends.Capacity && !friends.Contains(name))
				friends[index] = name;
			else if(type == 1 && index < ignores.Capacity && !ignores.Contains(name))
				ignores[index] = name;
			else return false;
			return true;
		}

		public Boolean removePersona(byte type, byte index)
		{
			if(type == 0 && index >= 0 && index < friends.Capacity && friends.ElementAtOrDefault(index) != null)
				friends[index] = null;
			else if(type == 1 && index >= 0 && index < ignores.Capacity && ignores.ElementAtOrDefault(index) != null)
				ignores[index] = null;
			else return false;
			return true;
		}
	}
}

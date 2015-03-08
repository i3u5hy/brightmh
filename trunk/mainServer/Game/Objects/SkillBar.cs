using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gameServer.Game.Objects
{
	public class SkillBar
	{
		private Dictionary<byte, int> skillBarValues = new Dictionary<byte, int>(21);

		public SkillBar()
		{

		}

		public void addToSkillBar(byte key, int value)
		{
			if(skillBarValues.ContainsKey(key))
				return;
			this.skillBarValues.Add(key, value);
		}

		public Dictionary<byte, int> getSkillBar()
		{
			return skillBarValues;
		}

		public int getSkillBarValue(byte key)
		{
			if(!skillBarValues.ContainsKey(key)) return 0;
			return skillBarValues[key];
		}

		public void removeFromSkillBar(byte key)
		{
			if(!skillBarValues.ContainsKey(key))
				return;
			skillBarValues.Remove(key);
		}
	}
}

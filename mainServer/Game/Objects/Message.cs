using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gameServer.Game.Objects
{
	public class Message
	{
		private int sender, receiver;
		private string message;
		private DateTime time;

		public Message(int sender, int receiver, string message)
		{
			this.sender = sender;
			this.receiver = receiver;
			this.message = message;
			this.time = DateTime.Now;
		}

		public int getSender()
		{
			return this.sender;
		}

		public int getReceiver()
		{
			return this.receiver;
		}

		public string getMessage()
		{
			return this.message;
		}

		public DateTime getDateTime()
		{
			return this.time;
		}

		public string getDateTimeString()
		{
			return this.time.ToString("yyyy-MM-dd HH:mm");
		}
	}
}

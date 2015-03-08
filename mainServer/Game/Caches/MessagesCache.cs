using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gameServer.Game.Objects;

namespace gameServer.Game.Caches
{
	public class MessagesCache
	{
		private static readonly MessagesCache instance = new MessagesCache();
		private MessagesCache() { }

		public static MessagesCache Instance { get { return instance; } }

		public List<Message> messages = new List<Message>();

		public void addMessage(Message message)
		{
			messages.Add(message);
		}

		public Message findMessageBySender(int suID)
		{
			return messages.Find(x => x.getSender() == suID);
		}

		public List<Message> findMessagesBySender(int suID)
		{
			return messages.FindAll(x => x.getSender() == suID);
		}

		public Message findMessageByReceiver(int ruID)
		{
			return messages.Find(x => x.getReceiver() == ruID);
		}

		public List<Message> findMessagesByReceiver(int ruID)
		{
			return messages.FindAll(x => x.getReceiver() == ruID);
		}
	}
}
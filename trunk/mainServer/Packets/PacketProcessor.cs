using gameServer.Core.IO;
using gameServer.Tools;

namespace gameServer.Packets
{
	public delegate void PacketHandler(MartialClient c, InPacket p);

	public sealed class PacketProcessor
	{
		public string Label { get; private set; }
		private PacketHandler[] m_handlers;
		private int m_count;

		public int Count { get { return m_count; } }

		public PacketProcessor(string label)
		{
			Label = label;
			m_handlers = new PacketHandler[0xFFFF + 1];
		}

		public void AppendHandler(int opcode, PacketHandler handler)
		{
			m_handlers[opcode] = handler;
			m_count++;
		}

		public PacketHandler this[int opcode]
		{
			get
			{
				Logger.WriteLog(Logger.LogTypes.Info, "Packet received: " + opcode + ".");
				try
				{
					return m_handlers[opcode];
				}
				catch
				{
					Logger.WriteLog(Logger.LogTypes.Warning, "Opcode " + opcode + " has no handler.");
					return null;
				}
			}
		}
	}
}

using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using gameServer.Core.IO;
using gameServer.Core.Network;
using gameServer.Game;
using gameServer.Game.Misc;
using gameServer.Packets;
using gameServer.Packets.Handlers.Commands;
using gameServer.Tools;

namespace gameServer
{
	public sealed class MartialClient : MartialSession
	{
		private PacketProcessor m_processor;
		private CommandProcessor c_processor;
		private Func<MartialClient, bool> m_deathAction;

		private Account account = new Account();
		private long sessionID;

		public void setSessionID(long sessionID)
		{
			this.sessionID = sessionID;
		}

		public long getSessionID()
		{
			return this.sessionID;
		}

		public void setAccount(Account account)
		{
			this.account = account;
		}

		public Account getAccount()
		{
			return this.account;
		}

		public MartialClient(Socket client, PacketProcessor processor, Func<MartialClient, bool> death) : base(client)
		{
			m_processor = processor;
			m_deathAction = death;
		}

		protected override void OnPacket(byte[] packet)
		{
			Console.WriteLine(BitConverter.ToString(packet));
			using (InPacket p = new InPacket(packet))
			{
				short opcode = 0;

				if(packet.Length >= 4) {
					opcode = (short)((int)((packet[0] & 0xFF) * 666) + (int)(packet[2] & 0xFF));
					if(packet[3] != 0x00)
						opcode = 1;
				}

				if(m_processor == null)
					return;

				PacketHandler handler = null;
				handler = m_processor[opcode];

				if (handler != null)
				{
					p.Skip(4);
					handler(this, p);
				}
				else
				{
					Logger.WriteLog(Logger.LogTypes.Warning, "[{0}] Unhandled packet from {1}: {2}.", m_processor.Label, Label, opcode);
				}
			}
		}

		protected override void OnDisconnected()
		{
			if(account != null) {
				if(account.characters != null)
				{
					CharacterFunctions.quitGameWorld(this);
					foreach(Character character in account.characters.Values) {
						character.Save();
					}
					account.clearCharacters();
				}
			}

			OutPacket wayToHell = new OutPacket(9);
			wayToHell.WriteInt(9);
			wayToHell.WriteShort(3);
			wayToHell.WriteRepeatedByte(6, 3); // just for lulz
			WriteRawPacket(wayToHell.ToArray());

			if(m_processor != null)
			{
				Logger.WriteLog(Logger.LogTypes.Disconnect, "[{0}] Client {1} disconnected.", m_processor.Label, Label);
				m_deathAction(this);
			}
		}
	}
}

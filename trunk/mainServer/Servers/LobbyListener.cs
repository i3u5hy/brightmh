using System.Collections.Generic;
using System.Net.Sockets;
using gameServer.Core.Network;
using gameServer.Tools;

namespace gameServer.Servers
{
	class LobbyListener
	{
		private Acceptor m_acceptor;

		public LobbyListener(short port)
		{
			m_acceptor = new Acceptor(port);
			m_acceptor.OnClientAccepted = OnClientAccepted;
		}

		private void OnClientAccepted(Socket client)
		{
			MartialClient mc = new MartialClient(client, null, null);
			byte[] pckt = new byte[16];
			pckt[0] = (byte)pckt.Length; //packet length
			pckt[4] = 0x01; //amount of servers
			pckt[8] = 0x02; //server names are hard coded in client itself. they are distinctable by this byte 
			pckt[12] = 0x01; //server status
			mc.WriteRawPacket(pckt);
			Logger.WriteLog(Logger.LogTypes.LList, "Client {0} handled - end connection.", mc.Label);
			mc.PoorDispose();
		}

		public void Run()
		{
			m_acceptor.Start();
			Logger.WriteLog(Logger.LogTypes.Info, "LListener		   - {0}.", m_acceptor.Port);
		}

		public void Shutdown()
		{
			m_acceptor.Stop();
		}
	}
}

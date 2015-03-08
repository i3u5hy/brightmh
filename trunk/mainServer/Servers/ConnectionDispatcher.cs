using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using gameServer.Core.Network;
using gameServer.Tools;

namespace gameServer.Servers
{
	class ConnectionDispatcher
	{
		private Acceptor m_acceptor;

		public ConnectionDispatcher(short port)
		{
			m_acceptor = new Acceptor(port);
			m_acceptor.OnClientAccepted = OnClientAccepted;
		}

		private void OnClientAccepted(Socket client)
		{
			MartialClient mc = new MartialClient(client, null, null);
			byte[] ip = Encoding.ASCII.GetBytes((mc.Label == "127.0.0.1") ? ("127.0.0.1") : (mc.Label == Constants.RouterIP) ? (Constants.RouterIP) : (Constants.ExternalIP));
			byte[] port = Encoding.ASCII.GetBytes(Convert.ToString(Constants.HellPort));
			byte[] pckt = new byte[ip.Length + port.Length + 13];

			pckt[0] = (byte)pckt.Length; //packet length
			pckt[4] = (byte)0x17; //packet header

			for(int i = 0;i < ip.Length;i++) {
				pckt[i + 8] = ip[i]; //add ip in the packet
			}

			pckt[(8 + ip.Length)] = (byte)0x20; //add space between ip and port

			for(int i = 0;i < port.Length;i++) {
				pckt[(i + 8 + ip.Length + 1)] = port[i]; //add port in the packet
			}

			mc.WriteRawPacket(pckt);
			Logger.WriteLog(Logger.LogTypes.CDisp, "Client {0} handled - end connection.", mc.Label);
			mc.PoorDispose();
		}

		public void Run()
		{
			m_acceptor.Start();
			Logger.WriteLog(Logger.LogTypes.Info, "CDispatcher		 - {0}.", m_acceptor.Port);
		}

		public void Shutdown()
		{
			m_acceptor.Stop();
		}
	}
}

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using gameServer.Core.Cryptography;

namespace gameServer.Core.Network {
	public abstract class MartialSession {
		public string Label {
			get;
			private set;
		}

		private readonly Socket m_socket;

		private MartialCipher m_sendCipher;
		private MartialCipher m_recvCipher;

		private bool m_header;
		private int m_offset;
		private byte[] m_buffer;

		private object m_locker;
		private bool m_disposed;

		protected abstract void OnPacket(byte[] packet);
		protected abstract void OnDisconnected();

		public bool Connected {
			get {
				return !m_disposed;
			}
		}

		public MartialSession(Socket socket) {
			m_socket = socket;
			m_socket.NoDelay = false;
			m_socket.ReceiveBufferSize = 0xFFFF;
			m_socket.SendBufferSize = 0xFFFF;

			m_locker = new object();
			m_disposed = false;

			Label = ((IPEndPoint)(m_socket.RemoteEndPoint)).Address.ToString();

			m_sendCipher = new MartialCipher(MartialCipher.TransformDirection.Encrypt);
			m_recvCipher = new MartialCipher(MartialCipher.TransformDirection.Decrypt);

			WaitForData(true, 4); // cause of Martial Packet Header size
		}

		private void WaitForData(bool header, int size) {
			if(m_disposed) {
				return;
			}

			m_header = header;
			m_offset = 0;
			m_buffer = new byte[size];

			BeginRead(m_buffer.Length);
		}

		private void BeginRead(int size) {
			SocketError outError = SocketError.Success;

			m_socket.BeginReceive(m_buffer, m_offset, size, SocketFlags.None, out outError, ReadCallback, null);

			if(outError != SocketError.Success) {
				Close();
			}
		}

		private void ReadCallback(IAsyncResult iar) {
			if(m_disposed) {
				return;
			}

			SocketError error;
			int received = m_socket.EndReceive(iar, out error);

			if(received == 0 || error != SocketError.Success) {
				Close();
				return;
			}

			m_offset += received;

			if(m_offset == m_buffer.Length) {
				HandleStream();
			}
			else {
				BeginRead(m_buffer.Length - m_offset);
			}
		}

		private void HandleStream() {
			if(m_header) {
				int size = MartialCipher.GetPacketLength(m_buffer);

				if(size > m_socket.ReceiveBufferSize) {
					Close();
					return;
				}

				WaitForData(false, size-4);
			}
			else {
				byte[] mh_buffer = new byte[4];
				Buffer.BlockCopy(m_buffer, 0, mh_buffer, 0, 4);
				for(int i = 0;i < m_buffer.Length-4;i++) {
					m_buffer[i] = m_buffer[i + 4];
				}
				Array.Resize(ref m_buffer, m_buffer.Length - 4);
				m_recvCipher.Transform(m_buffer);
				OnPacket(mh_buffer.Concat(m_buffer).ToArray());
				WaitForData(true, 4);
			}
		}

		public void WriteRawPacket(byte[] packet)
		{
			if(m_disposed)
			{
				return;
			}

			int offset = 0;

			while(offset < packet.Length)
			{
				SocketError outError = SocketError.Success;
				int sent = m_socket.Send(packet, offset, packet.Length - offset, SocketFlags.None, out outError);

				if(sent == 0 || outError != SocketError.Success)
				{
					Close();
					return;
				}

				offset += sent;
			}
		}

		public void Close()
		{
			Dispose();
		}

		public void PoorDispose()
		{
			if(!m_disposed)
			{
				lock(m_locker)
				{
					if(!m_disposed)
					{
						m_disposed = true;

						m_socket.Shutdown(SocketShutdown.Both);
						m_socket.Close();

						m_offset = 0;
						m_buffer = null;
					}
				}
			}
		}

		public void Dispose()
		{
			if(!m_disposed)
			{
				lock(m_locker)
				{
					if(!m_disposed)
					{
						m_disposed = true;

						try
						{
							m_socket.Shutdown(SocketShutdown.Both);
						}
						catch(SocketException)
						{

						}
						m_socket.Close();

						if(m_recvCipher != null)
							m_recvCipher.Dispose();

						m_offset = 0;
						m_buffer = null;

						m_sendCipher = null;
						m_recvCipher = null;

						OnDisconnected();
					}
				}
			}
		}
	}
}
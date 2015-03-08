using System;
using gameServer.Tools;

namespace gameServer.Core.Cryptography
{
	public sealed class MartialCipher : IDisposable
	{
		public enum TransformDirection : byte
		{
			Encrypt,
			Decrypt
		}

		private TransformDirection m_direction;

		private Action<byte[]> m_transformer;

		public TransformDirection TransformationDirection
		{
			get { return m_direction; }
		}

		public MartialCipher(TransformDirection transformDirection)
		{
			m_direction = transformDirection;

			m_transformer = new Action<byte[]>(DecryptTransform);
		}

		public void Transform(byte[] data)
		{
			m_transformer(data);
		}

		private void EncryptTransform(byte[] data)
		{
			Cryptography.Encryptor.Encrypt(data);
		}

		public void DecryptTransform(byte[] data)
		{
			Cryptography.Decryptor.Decrypt(data);
		}

		public static int GetPacketLength(byte[] packetHeader)
		{
			return BitConverter.ToInt32(packetHeader, 0);
		}

		public void Dispose()
		{
			m_transformer = null;
		}
	}
}

using System;
using System.Text;

namespace gameServer.Tools
{
	class BitTools
	{
		public static int byteToInt(byte b) 
		{
			return (int) b & 0xFF;
		}
		
		public static float byteArrayToFloat(byte[] data) 
		{
            return BitConverter.ToSingle(data, 0);
		}
		
		public static int byteArrayToInt(byte[] b)
		{
			return BitConverter.ToInt32(b, 0);
		}

		public static byte[] intToByteArray(int var) {
			return BitConverter.GetBytes(var);
		}
	
		public static byte[] shortToByteArray(short sval) 
		{
			return BitConverter.GetBytes(sval);
		}

		public static byte[] floatToByteArray(float f) {
			byte[] bytes = BitConverter.GetBytes(f);
			int result = BitConverter.ToInt32(bytes, 0);
			return intToByteArray(result);
		}
		
		public static byte[] stringToByteArray(String s)
		{
			if(s == null)
				return new byte[16];
			return Encoding.ASCII.GetBytes(s);
		}
	  
		public static String byteArrayToString(byte[] baww) 
		{
			return Encoding.ASCII.GetString(baww);
		}

		public static byte[] longToByteArray(long var) {
			return BitConverter.GetBytes(var);
		}
	}
}

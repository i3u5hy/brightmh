using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using gameServer.Tools;

namespace gameServer
{
	class Constants
	{
		public static readonly string ConfigName		= "Config.ini";
		public static readonly string gameDirectory		= m_ini.GetString(ConfigName, "Client", "Directory");
		public static readonly string username			= m_ini.GetString(ConfigName, "Database", "Username");
		public static readonly string database			= m_ini.GetString(ConfigName, "Database", "Database");
		public static readonly string password			= m_ini.GetString(ConfigName, "Database", "Password");
		public static readonly string host				= m_ini.GetString(ConfigName, "Database", "Host");
		public static readonly string WelcomeMessage	= m_ini.GetString(ConfigName, "ServerConfiguration", "Welcome");
		public static readonly string LogWriting		= m_ini.GetString(ConfigName, "ServerConfiguration", "Logs");
		public static readonly string CheatWriting		= m_ini.GetString(ConfigName, "ServerConfiguration", "Cheaters");
		public static readonly string ExternalIP		= m_ini.GetString(ConfigName, "ServerConfiguration", "External") != "-1" ?
			m_ini.GetString(ConfigName, "ServerConfiguration", "External") : IP.ExternalIPAddress();
		public static readonly string RouterIP			= initialiseRouterIP();
		//public static readonly int LoginPort			= 667;
		public static readonly int LLPort				= 10000;
		public static readonly int CDPPort				= 10002;
		public static readonly int HellPort				= 666;
		public static readonly bool VFSSkip				= m_ini.GetBool(ConfigName, "VFS", "SkipIfPossible");
		public static readonly bool clearCache			= m_ini.GetBool(ConfigName, "VFS", "ClearCacheAfterLoad");

		public static string initialiseRouterIP() {
			IPHostEntry host;
			string localIP = "";
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach(IPAddress ip in host.AddressList) {
				localIP = ip.ToString();

				String[] temp = localIP.Split('.');
				if(ip.AddressFamily == AddressFamily.InterNetwork && temp[0] == "192") {
					break;
				}
				else
					localIP = null;
			}
			return localIP;
		}

		public static bool initialiseConfig()
		{
			if(!File.Exists(ConfigName)) {
				File.WriteAllText(ConfigName, "[Database]\r\nUsername = \"root\"\r\nPassword = \"----\"\r\nDatabase = \"brightmh\"\r\nHost = \"127.0.0.1\"\r\n[ServerPorts]\r\nLobbyListener = 10000\r\nConnectionDispatcher = 10002\r\nHellEmissary = 666\r\n[ServerConfiguration]\r\nWelcome = \"Welcome to BrightMH!\"\r\nLogs = \"logs.txt\"\r\n[Client]\r\nDirectory = \"C:\\Users\\gawrprom\\Games\\doonline_20141024\\\"");
				return false;
			}

			if(!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "data"))
				Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "data");
			if(!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "data\\npcs"))
				Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "data\\npcs");
			if(!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "data\\regions"))
				Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "data\\regions");
			if(!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "data\\maps"))
				Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "data\\maps");
			if(!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "data\\mobs"))
				Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "data\\mobs");

			if(!File.Exists(LogWriting))
			{
				File.Create(LogWriting).Close();
			}

			if(!File.Exists(CheatWriting))
			{
				File.Create(CheatWriting).Close();
			}
			return true;
		}

		//tell client the authentication was a huge success - just a fucking crypto::Key -
		public static readonly byte[] authSuccess = new byte[] {
			(byte)0x46, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0xd9, (byte)0x00, (byte)0x1c, (byte)0x00, (byte)0x1c, (byte)0x00, 
			(byte)0x00, (byte)0x00, (byte)0x01, (byte)0x45, (byte)0xbb, (byte)0x1f, (byte)0x36, (byte)0xcb, (byte)0xfc, (byte)0x63, (byte)0x3f, (byte)0x11, (byte)0x50, (byte)0xaa, (byte)0xea, (byte)0x3a, 
			(byte)0x94, (byte)0x60, (byte)0x8e, (byte)0xed, (byte)0x0f, (byte)0x86, (byte)0xd5, (byte)0xb9, (byte)0xf1, (byte)0xd5, (byte)0x62, (byte)0xcf, (byte)0x90, (byte)0x7f, (byte)0x0e, (byte)0x00, 
			(byte)0x00, (byte)0x00, (byte)0x0d, (byte)0x87, (byte)0xc0, (byte)0x59, (byte)0x6c, (byte)0xe7, (byte)0xe8, (byte)0xd7, (byte)0xa8, (byte)0xbb, (byte)0xd8, (byte)0xce, (byte)0x15, (byte)0x6d, 
			(byte)0xac, (byte)0x83, (byte)0x21, (byte)0x4e, (byte)0x84, (byte)0x84, (byte)0x0a, (byte)0x00
		};

		//tell client the authentication was a huge fail
		public static readonly byte[] authFail = new byte[] { (byte)0x09, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x03, (byte)0x00, (byte)0x64, (byte)0x00, (byte)0xc9 };

		public static readonly byte[] accFail = new byte[] { (byte)0x09, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x03, (byte)0x00, (byte)0x64, (byte)0x00, (byte)0xCA, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00 };

		public static readonly byte[] accDoesntExist = new byte[] { 0x09, 0x00, 0x00, 0x00, 0x03, 0x00, 0x64, 0x00, 0xca };

		//empty account - no characters
		public static readonly byte[] emptyAccount = new byte[] {
			(byte)0x44, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x03, (byte)0x00, (byte)0x05, (byte)0x00, (byte)0x62, (byte)0x61, (byte)0x75, 
			(byte)0x6b, (byte)0x32, (byte)0x33, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00,
			(byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x6d,
			(byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, 
			(byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, 
			(byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, 
			(byte)0x00, (byte)0x00
		};

		//tell client the create character was a huge success
		public static readonly byte[] createNewCharacter = new byte[] {
			(byte)0x14, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x03, (byte)0x00, (byte)0x06, (byte)0x00, (byte)0x01, (byte)0x01, 
			(byte)0x12, (byte)0x2b, (byte)0x00, (byte)0xc0, (byte)0xb7, (byte)0xc4, (byte)0x00, (byte)0xe0, (byte)0x21, (byte)0x45
		};
		//14 00 00 00 03 00 06 00 00 CE 9F 2A 00 00 00 00 00 00 00 00 - nickname already taken
		public static readonly byte[] createNCharNameTaken = new byte[20] {
			(byte)0x14, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x03, (byte)0x00, (byte)0x06, (byte)0x00, (byte)0x00, (byte)0xCE,
			(byte)0x9F, (byte)0x2A, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00
		};

		public static readonly byte[] quit = new byte[] { (byte)0x09, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x03, (byte)0x00, (byte)0x64, (byte)0x00, (byte)0x00 };
	}
}
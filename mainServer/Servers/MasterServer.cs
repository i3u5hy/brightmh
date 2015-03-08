using System;
using System.IO;
using gameServer.Tools;

namespace gameServer.Servers
{
	class MasterServer
	{
		public static MasterServer Instance { get; set; }

		public bool Running { get; private set; }

		public HellEmissary HellEmissary { get; private set; }
		public ConnectionDispatcher ConnectionDispatcher { get; private set; }
		public LobbyListener LobbyListener { get; private set; }
		public MySQL_Connection SqlConnection { get; set; }

		public MasterServer()
		{
			SqlConnection = new MySQL_Connection();

			HellEmissary = new HellEmissary((Int16)Constants.HellPort);
			ConnectionDispatcher = new ConnectionDispatcher((Int16)Constants.CDPPort);
			LobbyListener = new LobbyListener((Int16)Constants.LLPort);
		}

		public void Run()
		{
			if(!HellEmissary.Run())
			{
				Logger.WriteLog(Logger.LogTypes.Error, "Server couldn't be initialised! Zero maps found.");
				MasterServer.Instance.Shutdown();
				return;
			}
			ConnectionDispatcher.Run();
			LobbyListener.Run();

			Running = true;

			Logger.WriteLog(Logger.LogTypes.Debug, "External IP address: {0}", Constants.ExternalIP);
			Logger.WriteLog(Logger.LogTypes.Debug, "Router IP address:   {0}", Constants.RouterIP);
			Logger.WriteLog(Logger.LogTypes.Info, "gameServer has been started on {0}", DateTime.Now.ToString());
		}

		public void Shutdown()
		{
			Logger.WriteLog(Logger.LogTypes.Info, "Shutting down MasterServer");

			if(LobbyListener != null) LobbyListener.Shutdown();
			if(ConnectionDispatcher != null) ConnectionDispatcher.Shutdown();
			if(HellEmissary != null) HellEmissary.Shutdown();
			SqlConnection.Disconnect();

			Running = false;

			Logger.WriteLog(Logger.LogTypes.Info, "gameServer went offline on {0}", DateTime.Now.ToString("dd-mm-yyyy HH:mm:ss"));
			Environment.Exit(0);
		}
	}
}

using System;
using MySql.Data.MySqlClient;

namespace gameServer.Tools
{
	public class MySQL_Connection
	{
		private MySqlConnection Connection { get; set; }
		private MySqlCommand Command { get; set; }
		public MySqlDataReader Reader { get; set; }
		public string mConnectionString { get; set; }
		public bool mShuttingDown { get; set; }
		private static readonly object locker = new object();

		public MySQL_Connection()
		{
			mShuttingDown = false;
			mConnectionString = "Server=" + Constants.host + "; Port=" + 3306 + "; Database=" + Constants.database + "; Uid=" + Constants.username + "; Pwd=" + Constants.password;
		}

		public MySQL_Connection(string username, string password, string database, string server, ushort port = 3306)
		{
			mShuttingDown = false;
			mConnectionString = "Server=" + Constants.host + "; Port=" + 3306 + "; Database=" + Constants.database + "; Uid=" + Constants.username + "; Pwd=" + Constants.password;
		}

		public void Disconnect()
		{
			try
			{
				mShuttingDown = true;
				if(Connection != null) Connection.Close();
			}
			catch(MySqlException ex)
			{
				//FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::Connect() : {1}", DateTime.Now.ToString(), ex.ToString()), true);
				Console.WriteLine(ex.ToString());
				throw new Exception(string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::Disconnect() : {1}", DateTime.Now.ToString(), ex.ToString()));
			}
		}

		public void Connect()
		{
			try
			{
				//FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Connecting...", DateTime.Now.ToString()), true);
				Connection = new MySqlConnection(mConnectionString);
				//FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] State Change...", DateTime.Now.ToString()), true);
				//Connection.StateChange += new System.Data.StateChangeEventHandler(Connection_StateChange);
				//FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Opening Connection", DateTime.Now.ToString()), true);
				
				Connection.Open();
				//FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Connected with MySQL server with version info: {1} and uses {2}compression", DateTime.Now.ToString(), Connection.ServerVersion, Connection.UseCompression ? "" : "no "), true);
				//Console.WriteLine("Connection setted up");
			}
			catch (MySqlException ex)
			{
				//FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::Connect() : {1}", DateTime.Now.ToString(), ex.ToString()), true);
				Console.WriteLine(ex.ToString());
				throw new Exception(string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::Connect() : {1}", DateTime.Now.ToString(), ex.ToString()));
				//Connect();
			}
			catch (Exception ex)
			{
				//FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::Connect() : {1}", DateTime.Now.ToString(), ex.ToString()), true);
				throw new Exception(string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::Connect() : {1}", DateTime.Now.ToString(), ex.ToString()));
			}
		}

		void Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
		{
			if (e.CurrentState == System.Data.ConnectionState.Closed && !mShuttingDown)
			{
				Logger.WriteLog(Logger.LogTypes.Info, "MySQL Connection lost, reconnecting...");
				Connection.StateChange -= Connection_StateChange;
				Connect();
			}
			else if (e.CurrentState == System.Data.ConnectionState.Open)
			{
				//empty lines pzl
				Console.WriteLine();
			}
		}

		public MySqlDataReader RunQuery(string query)
		{
			System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
			st.Start();
			try
			{
				this.Connect();
				Command = new MySqlCommand(query, Connection);
				if(query.StartsWith("SELECT"))
				{
					MySqlDataReader _reader = Command.ExecuteReader();
					st.Stop();
					Logger.WriteLog(Logger.LogTypes.Disconnect, "select took {0}", st.ElapsedMilliseconds);
					return _reader;
				}
				else if(query.StartsWith("DELETE") || query.StartsWith("UPDATE") || query.StartsWith("INSERT"))
				{
					int execution = Command.ExecuteNonQuery();
					st.Stop();
					Logger.WriteLog(Logger.LogTypes.Disconnect, "dek took {0}", st.ElapsedMilliseconds);
					return null;
				}
			}
			catch(InvalidOperationException)
			{
				Console.WriteLine("Lost connection to DB... Trying to reconnect and wait a second before retrying to run query.");
				//Connect();
				System.Threading.Thread.Sleep(1000);
				//RunQuery(query);
				return null;
			}
			catch(MySqlException ex)
			{
				if(ex.Number == 2055)
				{
					Console.WriteLine("Lost connection to DB... Trying to reconnect and wait a second before retrying to run query.");
					//Connect();
					System.Threading.Thread.Sleep(1000);
					RunQuery(query);
					return null;
				}
				else
				{
					Console.WriteLine(ex.ToString());
					Console.WriteLine(query);
					//FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::RunQuery({1}) : {2}", DateTime.Now.ToString(), query, ex.ToString()), true);
					throw new Exception(string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::RunQuery({1}) : {2}", DateTime.Now.ToString(), query, ex.ToString()));
				}
			}
			return null;
		}

		public int GetLastInsertId()
		{
			return (int)Command.LastInsertedId;
		}

		public bool Ping()
		{
			if (Reader != null && !Reader.IsClosed)
				return false;
			return Connection.Ping();
		}
	}
}

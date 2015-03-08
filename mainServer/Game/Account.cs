using System;
using System.Collections.Generic;
using System.Linq;
using gameServer.Servers;
using gameServer.Tools;
using gameServer.Game.Misc;
using MySql.Data.MySqlClient;

namespace gameServer.Game
{
	public sealed class Account
	{
		public MartialClient mClient { get; set; }
		public int aID { get; set; }
		public string name { get; set; }
		public int gmLvl { get; set; }
		public string ip { get; set; }
		public Character activeCharacter = null;
		public Dictionary<byte, Character> characters { get; set; }
		public int MHPoints { get; set; }

		public enum AccountLoadErrors
		{
			UndefinedAccount,
			SomebodyLoggedIn,
			Success
		}

		public AccountLoadErrors Load(string username, string password, string pin)
		{
			this.characters = new Dictionary<byte, Character>();

			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "SELECT * FROM users WHERE username='" + MySqlHelper.EscapeString(username) + "' AND password='" + MySqlHelper.EscapeString(password) + "' AND pin='" + MySqlHelper.EscapeString(pin) + "'";
				using(var reader = cmd.ExecuteReader())
				{
					reader.Read();

					if(!reader.HasRows)
					{
						return AccountLoadErrors.UndefinedAccount;
					}
					else
					{
						aID = reader.GetInt32("uid");
						if(MasterServer.Instance.HellEmissary.checkIfClientExists(aID))
						{
							return AccountLoadErrors.SomebodyLoggedIn;
						}
						name = username;
						gmLvl = reader.GetInt32("alvl");
						MHPoints = reader.GetInt32("mhpoints");

						return AccountLoadErrors.Success;
					}
				}
			}
		}

		public void LoadCharacters()
		{
			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "SELECT * FROM chars WHERE ownerID = " + aID + " LIMIT 5";
				using(var reader = cmd.ExecuteReader())
				{
					List<int> ids = new List<int>();

					for(int i = 0;i < 5;i++)
					{
						if(!reader.Read())
							break;

						ids.Add(reader.GetInt32(0));
					}

					foreach(int id in ids)
					{
						Character chr = new Character();
						chr.setcID(id);

						if(chr.Load(this) == 0)
						{
							Logger.WriteLog(Logger.LogTypes.Error, "Could not load character with ID {0}.", chr.getuID());
							continue;
						}

						chr.setAccount(this);
						MySQLTool.loadEq(chr);
						MySQLTool.loadInv(chr);
						MySQLTool.loadCargo(chr);
						MySQLTool.loadSkills(chr);
						MySQLTool.loadSkillBar(chr);
						MySQLTool.loadCommunities(chr);
						CharacterFunctions.calculateCharacterStatistics(chr);
						this.appendToCharacters(chr);
					}
				}
			}
		}

		public void Save()
		{
			//TODO
		}

		public void relistCharacters()
		{
			List<Character> temporary = new List<Character>();
			foreach(Character i in this.characters.Values)
			{
				temporary.Add(i);
			}
			this.characters.Clear();
			int x = 0;
			foreach(Character i in temporary)
			{
				this.characters.Add((byte)x, i);
				x++;
			}
		}

		public void appendToCharacters(Character character)
		{
			if(this.characters.Count() >= 5) return;
			for(byte i = 0;i < 5;i++)
			{
				if(!characters.ContainsKey(i))
				{
					characters.Add((byte)i, character);
					return;
				}
			}
		}

		public void clearCharacters() {
			characters.Clear();
		}
	}
}

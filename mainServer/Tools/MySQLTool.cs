using System.Collections.Generic;
using System.Text;
using System.Linq;
using gameServer.Game;
using gameServer.Game.Objects;
using gameServer.Servers;
using MySql.Data.MySqlClient;

namespace gameServer.Tools
{
	class MySQLTool
	{
		public static bool Save(string db, StringBuilder data, string inc, int incVal)
		{
			if(data.Length > 0 && data[0] == ',')
				data[0] = ' ';
			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "UPDATE " + db + " SET " + data.ToString() + " WHERE " + inc + " = " + incVal;
				cmd.ExecuteNonQuery();
			}
			return true;
		}

		public static int Create(string db, StringBuilder data)
		{
			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "INSERT INTO " + db + " " + data.ToString();
				cmd.ExecuteNonQuery();
			}
			return MasterServer.Instance.SqlConnection.GetLastInsertId();
		}

		public static int Delete(string db, string value, int valueEq)
		{
			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "DELETE FROM " + db + " WHERE " + value + " = " + valueEq;
				cmd.ExecuteNonQuery();
			}
			return 1;
		}

		public static void SaveEquipments(Character chr)
		{
			Equipment eq = chr.getEquipment();
			StringBuilder sb = new StringBuilder();
			for(byte i = 0;i<17;i++)
			{
				Item item = new Item();
				if(eq.getEquipments().ContainsKey(i))
					item = eq.getEquipItem(i);
				sb.Append(",s" + i + "=" + item.getItemID());
				sb.Append(",e" + i + "=" + item.getEnding());
			}

			Save("chars_eq", sb, "charID", chr.getuID());
			return;
		}

		public static void SaveInventories(Character chr)
		{
			Inventory inv = chr.getInventory();
			StringBuilder sb = new StringBuilder();
			for(int i = 0;i < inv.getPages() * 40;i++) {
				Item item = new Item();
				if(inv.getSeqSaved()[i] != -1)
					item = inv.getInvSaved()[inv.getSeqSaved()[i]];
				sb.Append(",i" + i + "=" + item.getItemID());
				sb.Append(",h" + i + "=" + inv.getSeqSaved()[i]);
				sb.Append(",q" + i + "=" + item.getQuantity());
				sb.Append(",e" + i + "=" + item.getEnding());
			}

			Save("chars_inv", sb, "charID", chr.getuID());
			return;
		}

		public static void SaveCargo(Character chr)
		{
			Cargo cargo = chr.getCargo();
			StringBuilder sb = new StringBuilder();
			for(int i = 0;i < 120;i++)
			{
				Item item = new Item();
				if(cargo.getSeqSaved()[i] != -1)
					item = cargo.getCargoSaved()[cargo.getSeqSaved()[i]];
				sb.Append(",i" + i + "=" + item.getItemID());
				sb.Append(",h" + i + "=" + cargo.getSeqSaved()[i]);
				sb.Append(",q" + i + "=" + item.getQuantity());
				sb.Append(",e" + i + "=" + item.getEnding());
			}

			Save("chars_cargo", sb, "charID", chr.getuID());
			return;
		}

		public static void SaveSkills(Character chr)
		{
			Skills skillz = chr.getSkills();
			StringBuilder sb = new StringBuilder();
			if(skillz.getLearnedSkills().Count == 0)
				return;
			for(int i = 0;i < skillz.getLearnedSkills().Count;i++) {
				sb.Append(",s" + i + "=" + skillz.getLearnedSkills()[i]);
			}

			Save("chars_skill", sb, "charID", chr.getuID());
			return;
		}

		public static void SaveSkillBar(Character chr)
		{
			StringBuilder sb = new StringBuilder();
			for(byte i = 0;i<21;i++) {
				sb.Append(",s" + i + "=" + chr.getSkillBar().getSkillBarValue(i));
			}

			Save("chars_sbar", sb, "charID", chr.getuID());
			return;
		}

		public static void SaveCommunities(Character chr)
		{
			int i = 0;
			StringBuilder sb = new StringBuilder();
			foreach(string s in chr.getCommunity().getFriendsList()) {
				sb.Append(",t" + i + "=0,n" + i + "=" + s == null ? "null" : "'" + s + "'");
				i++;
			}
			foreach(string s in chr.getCommunity().getIgnoresList()) {
				sb.Append(",t" + i + "=1,n" + i + "=" + s == null ? "null" : "'" + s + "'");
				i++;
			}

			Save("chars_com", sb, "charID", chr.getuID());
			return;
		}

		public static void loadEq(Character chr)
		{
			MySqlDataReader reader = MasterServer.Instance.SqlConnection.RunQuery("SELECT * FROM chars_eq WHERE charID=" + chr.getuID());
			Equipment eq = chr.getEquipment();

			if(reader.HasRows)
			{
				if(reader.Read())
				{
					int val = 0;
					long ending;
					for(byte i=0;i<17;i++)
					{
						val = reader.GetInt32(i * 2 + 1);
						ending = reader.GetInt64(i * 2 + 2);
						if(val != 0)
						{
							eq.addToEquips(i, new Item(val, 1, ending));
						}
					}
				}
			}
			reader.Close(); MasterServer.Instance.SqlConnection.Disconnect();
		}

		public static void loadInv(Character chr)
		{
			Inventory inv = chr.getInventory();
			inv.setPages(chr.getInvPages());

			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "SELECT * FROM chars_inv WHERE charID=" + chr.getuID();
				using(var reader = cmd.ExecuteReader())
				{
					reader.Read();

					if(!reader.HasRows)
					{
						return;
					}
					else
					{
						List<int> seqhash = new List<int>();

						int val = 0;
						long ending;
						short amount;
						Item it;
						for(int i = 0;i < 240;i++)
						{
							val = reader.GetInt32(i * 4 + 1);
							amount = reader.GetInt16(i * 4 + 3);
							it = null;
							if(val != 0 && amount != 0)
							{
								it = new Item(val);
							}
							val = reader.GetInt32(i * 4 + 2);
							ending = reader.GetInt64(i * 4 + 4);
							seqhash.Add(val);
							if(it != null)
							{
								inv.putIntoInv(val % 100, val / 100, it);
								it.setQuantity(amount);
							}
						}

						inv.saveInv();
						inv.setSeqSaved(seqhash);
					}
				}
			}
		}

		public static void loadCargo(Character chr)
		{
			Cargo cargo = chr.getCargo();

			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "SELECT * FROM chars_inv WHERE charID=" + chr.getuID();
				using(var reader = cmd.ExecuteReader())
				{
					reader.Read();

					if(!reader.HasRows)
					{
						return;
					}
					else
					{
						List<int> seqhash = new List<int>();

						int val = 0;
						long ending;
						short amount;
						Item it;
						for(int i = 0;i < 240;i++)
						{
							val = reader.GetInt32(i * 4 + 1);
							amount = reader.GetInt16(i * 4 + 3);
							it = null;
							if(val != 0 && amount != 0)
							{
								it = new Item(val, amount);
							}
							val = reader.GetInt32(i * 4 + 2);
							ending = reader.GetInt64(i * 4 + 4);
							seqhash.Add(val);
							if(it != null)
							{
								cargo.putIntoCargo(val % 100, val / 100, it);
							}
						}

						cargo.saveCargo();
						cargo.setSeqSaved(seqhash);
					}
				}
			}
		}

		public static void loadSkills(Character chr)
		{
			Skills skillz = chr.getSkills();

			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "SELECT * FROM chars_skill WHERE charID=" + chr.getuID();
				using(var reader = cmd.ExecuteReader())
				{
					reader.Read();

					if(!reader.HasRows)
					{
						return;
					}
					else
					{
						for(int i = 0;i < 60;i++)
						{
							int skillID = reader.GetInt32(i + 1);
							if(skillID != 0)
							{
								skillz.addToSkills(skillID);
							}
						}
					}
				}
			}
		}

		public static void loadSkillBar(Character chr)
		{
			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "SELECT * FROM chars_sbar WHERE charID=" + chr.getuID();
				using(var reader = cmd.ExecuteReader())
				{
					reader.Read();

					if(!reader.HasRows)
					{
						return;
					}
					else
					{
						SkillBar sBar = chr.getSkillBar();

						for(byte i = 0;i < 21;i++)
						{
							int skillID = reader.GetInt32(i + 1);
							if(skillID != -1) sBar.addToSkillBar(i, skillID);
						}
					}
				}
			}
		}

		public static void loadCommunities(Character chr)
		{
			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "SELECT * FROM chars_com WHERE charID=" + chr.getuID();
				using(var reader = cmd.ExecuteReader())
				{
					reader.Read();

					if(!reader.HasRows)
					{
						return;
					}
					else
					{
						Community com = chr.getCommunity();
						for(int i = 0;i < 40;i++)
						{
							byte type = reader.GetByte(i * 2 + 1);
							string person = reader.IsDBNull(i * 2 + 2) == true ? null : reader.GetString(i * 2 + 2);
							if(person != null) com.addPersona(type, person);
						}
					}
				}
			}
		}

		public static bool NameTaken(string name)
		{
			using(var con = new MySqlConnection(MasterServer.Instance.SqlConnection.mConnectionString))
			using(var cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = "SELECT * FROM chars WHERE charName = '" + MySqlHelper.EscapeString(name) + "'";
				using(var reader = cmd.ExecuteReader())
				{
					reader.Read();

					if(!reader.HasRows)
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
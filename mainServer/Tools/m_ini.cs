using System;
using System.Collections.Generic;
using System.IO;

namespace gameServer
{
	class m_ini
	{
		public static bool Exist(string filename)
		{
			return File.Exists(AppDomain.CurrentDomain.BaseDirectory + filename);
		}

		public static bool Delete(string filename)
		{
			if (!Exist(filename)) return false;
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + filename);
			return true;
		}

		public static bool Create(string filename, string insider=null)
		{
			if (!Exist(filename)) return false;
			File.Create(AppDomain.CurrentDomain.BaseDirectory + filename);
			if (insider != null) WriteText(AppDomain.CurrentDomain.BaseDirectory + filename, insider);
			return true;
		}

		public static bool WriteText(string filename, string insider)
		{
			if (!Exist(filename)) return false;
			File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + filename, insider);
			return true;
		}

		public static object Get(string filename, string section, string item)
		{
			if (!Exist(filename)) return -1;

			string _insta = null;
			bool sectionfound = false;
			System.IO.StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + filename);
			while ((_insta = file.ReadLine()) != null)
			{
				if (_insta == null) continue;
				if (!sectionfound)
				{
					if (_insta.IndexOf("[" + section + "]") != -1)
					{
						sectionfound = true;
					}
				}
				else
				{
					if (_insta[0] == '[')
					{
						file.Close();
						return -1;
					}

					if (_insta.IndexOf(item + " = \"") != -1)
					{
						_insta = _insta.Substring(item.Length + 4, _insta.Length - (item.Length + 5));
						break;
					}
					else if (_insta.IndexOf(item + " = ") != -1)
					{
						_insta = _insta.Substring(item.Length+3, _insta.Length - (item.Length+3));
						break;
					}
					else if (_insta.IndexOf(item + "=") != -1)
					{
						_insta = _insta.Substring(item.Length + 1, _insta.Length - (item.Length + 1));
						break;
					}
				}
			}
			file.Close();
			return _insta;
		}

		public static string GetString(string filename, string section, string item)
		{
			return Convert.ToString(Get(filename, section, item));
		}

		public static int GetInt(string filename, string section, string item)
		{
			return Convert.ToInt32(Get(filename, section, item));
		}

		public static bool GetBool(string filename, string section, string item)
		{
			return Convert.ToBoolean(GetInt(filename, section, item));
		}

		public static void Set(string filename, string section, string item, object value)
		{
			if (!Exist(filename)) return;

			string _insta = "\0";
			int sectionfound = -1, itemfound = -1, line = 0;
			string itemvalue = "\0";

			System.IO.StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + filename);
			while ((_insta = file.ReadLine()) != null)
			{
				line++;
				if (_insta == null) continue;
				if (sectionfound == -1)
				{
					if (_insta.IndexOf("[" + section + "]") != -1)
					{
						sectionfound = line;
					}
				}
				else
				{
					if (_insta[0] == '[')
					{
						return;
					}

					if (_insta.IndexOf(item) != -1)
					{
						itemfound = line;
						itemvalue = _insta;
						break;
					}
				}
			}
			file.Close();
			
			if (sectionfound == -1)
			{
				List<string> m_oEnum = new List<string>() { "\n\n[" + section + "]", item + " = " + value };
				File.AppendAllLines(AppDomain.CurrentDomain.BaseDirectory + filename, m_oEnum);
				return;
			}
			string[] full_file = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + filename);
			List<string> l = new List<string>();
			l.AddRange(full_file);
			if (itemfound == -1)
			{
				l.Insert(sectionfound+1, item + " = " + value);
			}
			else
			{
				l.Remove(itemvalue);
				l.Insert(itemfound-1, item + " = " + value);
			}
			File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + filename, l.ToArray());
			return;
		}
	}
}

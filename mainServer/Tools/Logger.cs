using System;
using System.Collections.Generic;

namespace gameServer.Tools
{
	class Logger
	{
		public static Dictionary<HackTypes, MartialClient> Cheaters = new Dictionary<HackTypes, MartialClient>();

		public enum LogTypes
		{
			Debug,
			Info,
			LList,
			CDisp,
			HEmi,
			Warning,
			Error,
			Connect,
			Disconnect
		}

		public enum HackTypes
		{
			CreateCharacter,
			CharacterSelection,
			NullActive,
			NPC,
			Equip,
			Speed,
			Dupe,
			Warp,
			Login,
			Chat,
			Guild,
			Items
		}

		public static bool mCommandEnabled = false;
		public static string mCommandBuffer = "";

		public static void WriteHeader()
		{
			Console.Clear();
			ConsoleColor tmpColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine("\n   C# Martial Heroes Emulator");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("					 ");
			Console.ForegroundColor = tmpColor;
		}

		public static void LogCheat(HackTypes type, MartialClient c, string msg = "", params object[] pObjects)
		{
			//Cheaters.Add(type, c);
			FileWriter.Write(Constants.CheatWriting, "[" + c.getAccount().name + "|" + c.Label + "]" + string.Format(msg + "\n", pObjects));
			//TODO:
			//Log msg and such to file.
		}

		public static void WriteLog(LogTypes type, string msg, params object[] pObjects)
		{
			FileWriter.Write(Constants.LogWriting, string.Format(msg + "\n", pObjects));
			string tab = "";
			for (int i = 1; i > (type.ToString().Length / 8); i--)
			{
				tab += "\t";
			}
			if (type == LogTypes.Debug)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("    Debug" + tab + "| ");
			}
			else if (type == LogTypes.Info)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write("    Info" + tab + "| ");
			}
			else if (type == LogTypes.LList)
			{
				Console.ForegroundColor = ConsoleColor.DarkMagenta;
				Console.Write("    Lobby" + tab + "| ");
			}
			else if (type == LogTypes.CDisp)
			{
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.Write("    CDisp" + tab + "| ");
			}
			else if (type == LogTypes.HEmi)
			{
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.Write("    Hell" + tab + "| ");
			}
			else if (type == LogTypes.Warning)
			{
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.Write("    Warning" + tab + "| ");
			}
			else if (type == LogTypes.Error)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("    Error" + tab + "| ");
			}
			else if (type == LogTypes.Connect)
			{
				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.Write("    Connect" + tab + "| ");
			}
			else if (type == LogTypes.Disconnect)
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.Write("   Disconnect" + tab + "| ");
			}

			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(msg, pObjects);
			if (mCommandEnabled)
				Console.Write("> {0}", mCommandBuffer);
		}

		public static void Read()
		{
			Console.Read();
		}
	}
}

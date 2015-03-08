using System.Linq;
using gameServer.Game;
using gameServer.Tools;
using gameServer.Core.IO;

namespace gameServer.Packets.Handlers
{
	static class LoginHandler
	{
		public static void Ping(MartialClient c, InPacket p)
		{
			c.WriteRawPacket(new byte[0]);
		}

		public static void LauncherValidate(MartialClient c, InPacket p)
		{
			byte pinLength = p.ReadByte();
			byte uNameLength = p.ReadByte();
			byte passWLength = p.ReadByte();
			p.ReadByte();
			string pin = MiscFunctions.obscureString(p.ReadString(4));
			string uN = MiscFunctions.obscureString(p.ReadString(16));
			string pW = MiscFunctions.obscureString(p.ReadString(12));

			Account account = new Account();
			if(account.Load(uN, pW, pin) != Account.AccountLoadErrors.Success)
			{
				c.WriteRawPacket(Constants.accDoesntExist);
				Logger.WriteLog(Logger.LogTypes.HEmi, "Authorization error for [{0} | {1} | {2}]", uN, pW, pin);
				c.Close();
				return;
			}
			Logger.WriteLog(Logger.LogTypes.HEmi, "User passed authorization [{0} | {1} | {2}]", uN, pW, pin);
			account.mClient = c;
			c.setAccount(account);
			account.LoadCharacters();

			if(c.getAccount().characters.Count() > 0)
			{
				c.WriteRawPacket(LoginPacketCreator.initCharacters(c.getAccount(), false).Concat(Constants.emptyAccount).ToArray());
			}
			c.WriteRawPacket(Constants.emptyAccount);
			c.WriteRawPacket(LoginPacketCreator.initAccount(c.getAccount()));
		}

		public static void InvalidVersion(MartialClient c, InPacket p)
		{
			c.WriteRawPacket(Constants.authFail);
			c.Close();
			return;
		}

		/*public static void HandleCheckUserLimit(MartialClient c, InPacket p)
		{
			byte world = p.ReadByte();

			if (!MasterServer.Instance.Worlds.InRange(world))
			{
				return;
			}

			int current = MasterServer.Instance.Worlds[world].CurrentLoad;

			if (current >= Constants.MaxPlayers) //full
			{
				c.WritePacket(LoginPacketCreator.CheckUserLimit(2));
			}
			else if (current * 2 >= Constants.MaxPlayers) //half full
			{
				c.WritePacket(LoginPacketCreator.CheckUserLimit(1));
			}
			else //under half full
			{
				c.WritePacket(LoginPacketCreator.CheckUserLimit(0));
			}
		}*/
	}
}

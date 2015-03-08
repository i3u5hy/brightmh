using System.Linq;
using System.Text.RegularExpressions;
using gameServer.Core.IO;
using gameServer.Game;
using gameServer.Game.Misc;
using gameServer.Game.World;
using gameServer.Servers;
using gameServer.Tools;

namespace gameServer.Packets.Handlers
{
	static class Selection
	{
		public static void CreateNewCharacter(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter != null)
			{
				Logger.LogCheat(Logger.HackTypes.CreateCharacter, c, "Attempted to create a character while being ingame.");
				c.Close();
				return;
			}

			if(c.getAccount().characters.Count() == 5)
			{
				Logger.LogCheat(Logger.HackTypes.CreateCharacter, c, "Attempted to create a character while characters count is 5.");
				c.Close();
				return;
			}

			string charName = MiscFunctions.obscureString(p.ReadString(18));
			if(charName == null)
			{
				c.WriteRawPacket(Constants.createNCharNameTaken);
				return;
			}
			if(charName.Length < 3 || Regex.Replace(charName, "[^A-Za-z0-9]+", "") != charName || MySQLTool.NameTaken(charName))
			{
				c.WriteRawPacket(Constants.createNCharNameTaken);
				return;
			}

			byte face = (byte)p.ReadShort();
			if(face < 1 || face > 7)
			{
				Logger.LogCheat(Logger.HackTypes.CreateCharacter, c, "Attempted to create a character with face no {0}", face);
				c.WriteRawPacket(Constants.createNCharNameTaken);
				return;
			}

			short unknownShit = p.ReadShort(); // but let's check it
			if(unknownShit > 0) Logger.WriteLog(Logger.LogTypes.Debug, "Create character's shit: {0}", unknownShit);

			short unknownShit2 = p.ReadShort();
			if(unknownShit2 > 0)
				Logger.WriteLog(Logger.LogTypes.Debug, "Create character's shit: {0}", unknownShit2);

			byte cClass = (byte)p.ReadShort();
			if(cClass < 1 || cClass > 4)
			{
				Logger.LogCheat(Logger.HackTypes.CreateCharacter, c, "Attempted to create a character with class no {0}", cClass);
				c.WriteRawPacket(Constants.createNCharNameTaken);
				return;
			}

			byte[] stats = { (byte)p.ReadShort(), (byte)p.ReadShort(), (byte)p.ReadShort(), (byte)p.ReadShort(), (byte)p.ReadShort() };
			byte statPoints = (byte)p.ReadShort();

			if(stats[0] + stats[1] + stats[2] + stats[3] + stats[4] + statPoints > 55)
			{
				Logger.LogCheat(Logger.HackTypes.CreateCharacter, c, "Attempted to create a character with weird amount of attributes.");
				c.WriteRawPacket(Constants.createNCharNameTaken);
				return;
			}

			Character newChr = new Character();
			newChr.setName(charName);
			newChr.setFace(face);
			newChr.setcClass(cClass);
			newChr.setStr(stats[0]);
			newChr.setDex(stats[1]);
			newChr.setVit(stats[2]);
			newChr.setAgi(stats[3]);
			newChr.setInt(stats[4]);
			newChr.setStatPoints(statPoints);

			newChr.setAccount(c.getAccount());
			if (newChr.Create() == true)
			{
				CharacterFunctions.createEquipments(newChr);
				CharacterFunctions.createInventories(newChr);
				CharacterFunctions.calculateCharacterStatistics(newChr);
				newChr.setCurHP(newChr.getMaxHP());
				newChr.setCurMP(newChr.getMaxMP());
				newChr.setCurSP(newChr.getMaxSP());
				c.getAccount().appendToCharacters(newChr);
				c.WriteRawPacket(Constants.createNewCharacter);
				return;
			}

			c.WriteRawPacket(Constants.createNCharNameTaken);
			return;
		}

		public static void RequestSpawn(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter != null)
			{
				Logger.LogCheat(Logger.HackTypes.CreateCharacter, c, "Attempted to spawn a character while being ingame.");
				c.Close();
				return;
			}

			byte selected_character = p.ReadByte();

			if(!c.getAccount().characters.ContainsKey(selected_character))
			{
				Logger.LogCheat(Logger.HackTypes.CharacterSelection, c, "Wrong target '{0}' has been selected by selection packet", selected_character);
				c.Close();
				return;
			}

			Character target = c.getAccount().characters[selected_character];

			c.getAccount().activeCharacter = target;

			WMap.Instance.addToCharacters(target);
			CharacterFunctions.setPlayerPosition(target, target.getPosition()[0], target.getPosition()[1], target.getMap());
			CharacterFunctions.calculateCharacterStatistics(target);
			StaticPackets.sendSystemMessageToClient(c, 1, Constants.WelcomeMessage);
		}

		public static void MoveToVV(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter != null)
			{
				Logger.LogCheat(Logger.HackTypes.CreateCharacter, c, "Attempted to create a character while being ingame.");
				c.Close();
				return;
			}

			byte selected_character = p.ReadByte();

			if(!c.getAccount().characters.ContainsKey(selected_character))
			{
				Logger.LogCheat(Logger.HackTypes.CharacterSelection, c, "Wrong target '{0}' has been selected by packet", selected_character);
				c.Close();
				return;
			}

			Character target = c.getAccount().characters[selected_character];

			target.setPosition(new float[] { -1660, 2344 });
			target.setMap(1);

			OutPacket op = new OutPacket(24);
			op.WriteInt(24);
			op.WriteShort(3);
			op.WriteShort(14);
			op.WriteByte(1);
			op.WriteByte(selected_character);
			op.WriteByte(6);
			op.WriteByte(8);
			op.WriteInt(1); // map
			op.WriteFloat(-1660); // X
			op.WriteFloat(2344); // Y
			c.WriteRawPacket(op.ToArray());
		}

		public static void ReturnToSelection(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.CharacterSelection, c, "Hooked returnToSelection with null of activeCharacter");
				c.Close();
				return;
			}

			c.getAccount().activeCharacter.getInventory().updateInv();
			c.getAccount().activeCharacter.getInventory().saveInv();
			c.getAccount().activeCharacter.getCommunity().relistCommunities();
			CharacterFunctions.quitGameWorld(c);
			c.getAccount().relistCharacters();
			c.WriteRawPacket(LoginPacketCreator.initCharacters(c.getAccount(), true));
		}

		public static void RemoveCharacter(MartialClient c, InPacket p)
		{
			if(c.getAccount().activeCharacter != null)
			{
				Logger.LogCheat(Logger.HackTypes.CreateCharacter, c, "Attempted to remove a character while being ingame.");
				c.Close();
				return;
			}			

			byte selected_character = p.ReadByte();

			if(c.getAccount().characters.Count < selected_character)
			{
				Logger.LogCheat(Logger.HackTypes.CharacterSelection, c, "Characters count is smaller than selected character {0}", selected_character);
				c.Close();
				return;
			}

			if(!c.getAccount().characters.ContainsKey(selected_character))
			{
				Logger.LogCheat(Logger.HackTypes.CharacterSelection, c, "Wrong target '{0}' has been selected by packet", selected_character);
				c.Close();
				return;
			}

			Character target = c.getAccount().characters[selected_character];

			int removement_mode = p.ReadByte();

			OutPacket op = new OutPacket(11);
			op.WriteInt(11);
			op.WriteShort(0x03);
			op.WriteShort(0x07);
			op.WriteByte(0x01);
			op.WriteByte(selected_character);

			if(target.getDeleteState() == true)
			{ // state 
				if(removement_mode == 1)
				{ // recover
					op.WriteByte(0x00);
					target.setDeleteState(false);
				}
				else
				{ // Perm Delete
					op.WriteByte(0x02);
					target.Delete(selected_character);
				}
			}
			else
			{ // state 0
				if(removement_mode == 0)
				{
					op.WriteByte(0x01);
				} // turn your back
				target.setDeleteState(true);
			}

			c.WriteRawPacket(op.ToArray());
		}

		public static void Quit(MartialClient c, InPacket p)
		{
			CharacterFunctions.quitGameWorld(c);
			c.WriteRawPacket(new byte[] { (byte)0x09, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x03, (byte)0x00, (byte)0x64, (byte)0x00, (byte)0x00 });
			c.Close();
		}
	}
}

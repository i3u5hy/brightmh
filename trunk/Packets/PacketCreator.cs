using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using gameServer.Core.IO;
using gameServer.Game;
using gameServer.Game.Caches;
using gameServer.Game.Misc;
using gameServer.Game.Objects;
using gameServer.Game.World;
using gameServer.Tools;

namespace gameServer.Packets
{
	static class GuildPackets
	{
		public static byte[] refreshGuild(Character chr)
		{
			Guild guild = chr.getGuild();

			OutPacket op = new OutPacket(1644);
			op.WriteInt		(32);
			op.WriteShort	(4);
			op.WriteShort	(97);
			op.WriteInt		(1);
			op.WriteInt		(chr.getuID());
			op.Skip			(16);

			op.WriteInt		(1612);
			op.WriteShort	(4);
			op.WriteShort	(65);
			op.WriteInt		(1);
			op.WriteInt		(chr.getuID());
			op.WriteShort	(1);
			op.WritePaddedString(guild.guildName, 18);
			op.WriteShort	(guild.guildIcon);
			op.WriteShort	(guild.guildType);
			op.WriteInt		((guild.guildFame / 100) + (guild.guildGold / 10));
			op.WriteInt		(guild.guildFame);
			op.WriteLong	(guild.guildGold);
			op.WriteInt		(guild.guildHat);

			for(int i = 0;i < 50;i++)
			{
				op.WriteInt(guild.guildMembers.ElementAt(i) == null ? 0 : guild.guildMembers.ElementAt(i).getuID());
			} // 60 - 259

			for(int i = 0;i < 50;i++)
			{
				op.WriteByte(guild.guildMembers.ElementAt(i) == null ? (byte)0 : guild.guildMembers.ElementAt(i).getGuildRank());
			} // 260 - 309

			for(int i = 0;i < 50;i++)
			{
				if(guild.guildMembers.ElementAt(i) == null)
				{
					op.Skip(17);
					continue;
				}
				if(guild.guildMembers.ElementAt(i).getOnlineCharacter() == null)
				{
					op.Skip(17);
					continue;
				}
				Character tmp = guild.guildMembers.ElementAt(i).getOnlineCharacter();
				op.WritePaddedString(tmp.getName(), 17);
			} // 310 - 1159

			for(int i = 0;i < 50;i++)
			{
				if(guild.guildMembers.ElementAt(i) == null)
				{
					op.Skip(1);
					continue;
				}
				if(guild.guildMembers.ElementAt(i).getOnlineCharacter() == null)
				{
					op.Skip(1);
					continue;
				}
				Character tmp = guild.guildMembers.ElementAt(i).getOnlineCharacter();
				op.WriteByte(tmp.getcClass());
			}
			return op.ToArray();
		}

		public static byte[] createGuildResponse(Character chr, byte managementType, byte managementArg = 0, string guildName = null)
		{
			OutPacket op = new OutPacket(88); // 32 & 56
			op.WriteInt		(32);
			op.WriteShort	(4);
			op.WriteShort	(97);
			op.WriteInt		(1);
			op.WriteInt		(chr.getuID()); // 12-15
			op.Skip			(16); // 16-31

			op.WriteInt		(56);
			op.WriteShort	(4);
			op.WriteShort	(61);
			op.WriteInt		(1);
			op.WriteInt		(chr.getuID());
			op.WriteShort	(1);
			op.WriteByte	(managementType);
			op.WriteByte	(managementArg);
			op.WritePaddedString(guildName ?? "", 20);
			op.WriteInt		(chr.getFame());
			op.WriteInt		(/* 344 ?? */);
			op.WriteLong	(chr.getCoin());
			return op.ToArray();
		}

		public static byte[] extCharGuild(Character chr)
		{
			OutPacket op = new OutPacket(40);
			op.WriteInt		(40);
			op.WriteShort	(5);
			op.WriteShort	(65);
			op.WriteInt		(1);
			op.WriteInt		(chr.getuID());
			op.WriteByte	(chr.getGuild().guildType);
			op.WritePaddedString(chr.getGuild().guildName, 17);
			op.WriteShort	(chr.getGuild().guildIcon);
			return op.ToArray();
		}

		public static byte[] quitGuildForInternal(Character chr)
		{
			/* This packet shows teh laziness of koreanz, they use one fucking shit probably for everything - 3 packets just in one case ;
			 * to show teh playa, that he has a guild or na ;
			 * that he lives / joinz guild etc ;
			 * oh lulz at u koreanz, I'd not like to work on Karma too ;
			 * I already see on my eyez how much0 CRSpace team was butthurted to work on this shit XD*/

			OutPacket op = new OutPacket(1748);
			op.WriteInt		(32);
			op.WriteShort	(4);
			op.WriteShort	(97);
			op.WriteInt		(1);
			op.WriteInt		(chr.getuID()); // 12-15
			op.Skip			(16); // 16-31

			op.WriteInt		(72); // yeah - new packet & fuck rzaah 32-35
			op.WriteShort	(4);  // 36 - 37
			op.WriteShort	(63); // 38 - 39
			op.WriteInt		(1); // 40 - 43
			op.WriteInt		(chr.getuID()); // 44 - 47
			op.WriteInt		(1); // 48 - 51
			op.WriteInt		(chr.getuID()); // 52 - 55
			op.WriteInt		(3487029); // 555 - but wut for? // 56 - 59
			op.Skip			(12); // 60 - 71
			op.WriteInt		(135570688); // lukz f4mili4r <: // 72 - 75
			op.WriteInt		(chr.getuID()); // 76 - 79
			op.WriteInt		(3487029); // 555 - n again, wut for???? // 80 - 83
			op.Skip			(12); // 84 - 95
			op.WriteInt		(781505280); // damn koreanz, trying to stop us by their hash identificator shit! but we g0n4 fight to da last sold13r!! // 96 - 99
			op.WriteInt		(); // first skipped integer? :0 wutwut // 100 - 103

			op.WriteInt		(32); // another p4ck3t ; n yeah, it was already here ^ // 104 - 107
			op.WriteShort	(4); // 108 - 109
			op.WriteShort	(97); // 110 - 111
			op.WriteInt		(1); // 112 - 115
			op.WriteInt		(chr.getuID()); // 116 - 119
			op.Skip			(16); // 120 - 135

			op.WriteInt		(1612); // 4nd y34h, gi4nt sh1t upc0min qq & nother pckt & another dick in rzaah's ass // 136 - 139
			op.WriteShort	(4); // 140 - 141
			op.WriteShort	(65); // 142 - 143
			op.WriteInt		(1); // 144 - 147
			op.WriteInt		(chr.getuID()); // 148 - 152
			return op.ToArray();
		}

		public static byte[] quitGuildForExternals(Character chr)
		{
			OutPacket op = new OutPacket(40);
			op.WriteInt		(40);
			op.WriteShort	(0x05);
			op.WriteShort	(0x41);
			op.WriteInt		(1);
			op.WriteInt		(chr.getuID());
			return op.ToArray();
		}

		public static byte[] getRefreshNewsGuildPacket(string message)
		{
			OutPacket op = new OutPacket(212);
			op.WriteInt		(212);
			op.WriteShort	(4);
			op.WriteShort	(103);
			op.WriteInt		(1);
			op.WritePaddedString(message, 195);
			return op.ToArray();
		}
	}

	static class CommunityPackets
	{
		public static byte[] getIncomingMessagePacket(Message msg, Character chr)
		{
			string message = msg.getMessage();
			int messageLength = message.Length;

			OutPacket op = new OutPacket(messageLength + 91);
			op.WriteInt		();
			op.WriteShort	(1);
			op.WriteShort	(20);
			op.WriteInt		(358585);
			op.WritePaddedString(msg.getDateTimeString(), 20);
			op.WriteInt		(12);
			op.WriteInt		(4);
			op.WriteLong	(4479951538479293);
			op.WritePaddedString(chr.getName(), 17);
			op.WritePaddedString("BrightMH", 17);
			op.WriteShort	(2080);
			op.WriteInt		(messageLength);
			op.WriteString	(message);
			return op.ToArray();
		}
	}

	static class VendingPackets
	{
		public static byte[] getFameVendingPacket(Character chr)
		{
			OutPacket op = new OutPacket(32);
			op.WriteInt		(32);
			op.WriteShort	(0x05);
			op.WriteShort	(0x43);
			op.WriteInt		(137560065);
			op.WriteInt		(chr.getuID());
			op.WriteInt		();
			op.WriteByte	(FameNickNames.Instance.getFameNickID(chr.getFame()));
			op.WriteByte	(chr.getVp());
			op.WriteInt		(chr.getAccount().gmLvl);
			op.WriteByte	(0x02);
			op.WriteByte	(0x03);
			op.WriteInt		(chr.getFame());
			return op.ToArray();
		}

		public static byte[] createVendorFrame(Character chr, int state, String shopname = null)
		{
			byte[] nameByte = BitTools.stringToByteArray(shopname);
			byte[] chid = BitTools.intToByteArray(chr.getuID());
			byte[] venState = new byte[52];
		
			venState[0] = (byte)venState.Length;
			venState[4] = (byte)0x04;
			venState[6] = (byte)0x37;
		
			venState[8] = (byte)0x01;
			venState[9] = (byte)0x9d;
			venState[10] = (byte)0x0f;
			venState[11] = (byte)0xbf;
		
			for(int i=0;i<4;i++) {
				venState[12+i] = chid[i];
			}
		
			venState[16] = (byte)0x01;
		
			//1 = open, 0 = close
			venState[18] = (byte)state;
		
			if(state == 0)
			{
				Console.WriteLine("Closing vendorshop: " + shopname);
				venState[50] = (byte)0xdb;
				venState[51] = (byte)0x2a;
			}
			else
			{
				Console.WriteLine("Creating vendorshop: " + shopname);
				for(int i=0;i<30;i++)
				{
					if(nameByte[i] != (byte)0x00)
					{
						//shop name
						venState[19+i] = nameByte[i];
					}
					else
					{
						break;
					}
				}
			
				venState[50] = (byte)0x0f;
				venState[51] = (byte)0xbf;
			}
		
			//cur.sendToMap(venState);
			return venState;
		}

		public static byte[] addItemToVendor(ItemVendor item, int state, Character cur, int x, int y) {
			Console.WriteLine("Adding item " + item.getItemID() + " of index " + item.getInvIndex() + " on position(x, y) (" + x + ", " + y +")");
			byte[] price = BitTools.longToByteArray(item.getPrice());
			byte[] chid = BitTools.intToByteArray(cur.getuID());
			byte[] venItem = new byte[36];
		
			venItem[0] = (byte)venItem.Length;
			venItem[4] = (byte)0x04;
			venItem[6] = (byte)0x39;

			venItem[8] = (byte)0x01;
			venItem[9] = (byte)0x04; //f5 need when remove?
			venItem[10] = (byte)0x67; //10 need when remove?
			venItem[11] = (byte)0x28; //29 need when remove?
	
			for(int i=0;i<4;i++) {
				venItem[12+i] = chid[i];
			}

			venItem[16] = (byte)0x01;

			//Status: 1 = add, 0 = remove
			venItem[18] = (byte)state;
		
			//item inventory id
			venItem[19] = (byte)item.getInvIndex();
			//item number (Unique id for vendor UI)
			venItem[20] = (byte)item.getItemID();
		
			//y
			venItem[21] = (byte)y;
			//x
			venItem[22] = (byte)x;
		
			//amount
			venItem[24] = (byte)item.getQuantity();
		
			venItem[26] = (byte)0x0f;
			venItem[27] = (byte)0xbf;

			//price 6 bytes long
			//first byte can be 0x00 when high number
			venItem[28] = price[0];
			for(int i=0;i<price.Length-1;i++)
			{
				venItem[29+i] = price[1+i];
			}
			return venItem;
		}

		public static byte[] buyItemFromVendor(Character buy, Character sold, int index, int invSlot, int x, int y, short amount)
		{
			Console.WriteLine("Handling Buying vendor item");

			byte[] chid = BitTools.intToByteArray(buy.getuID());
			byte[] chid1 = BitTools.intToByteArray(sold.getuID());
			byte[] newMoneyCur = BitTools.intToByteArray((int)buy.getCoin());

			byte[] venBuy = new byte[40];
			venBuy[0] = (byte)venBuy.Length;

			venBuy[4] = (byte)0x04;
			venBuy[6] = (byte)0x3a;

			venBuy[8] = (byte)0x01;
			venBuy[9] = (byte)0x6b;
			venBuy[10] = (byte)0x15;
			venBuy[11] = (byte)0x08;

			for(int i = 0;i < 4;i++)
			{
				venBuy[12 + i] = chid[i];
				venBuy[20 + i] = chid1[i];
				venBuy[32 + i] = newMoneyCur[i];
			}

			venBuy[16] = (byte)0x01;

			venBuy[18] = (byte)0xDB;
			venBuy[19] = (byte)0x2A;

			venBuy[24] = (byte)index;
			venBuy[25] = (byte)invSlot;
			venBuy[26] = (byte)x;
			venBuy[27] = (byte)y;
			venBuy[28] = (byte)amount;
			return venBuy;
		}

		public static byte[] buyItemFromVendorSecondSite(Character buy, Character sold, int index, int invSlot, int x, int y, int amount)
		{
			Console.WriteLine("Handling Buying vendor item");

			byte[] chid = BitTools.intToByteArray(buy.getuID());
			byte[] chid1 = BitTools.intToByteArray(sold.getuID());
			byte[] newMoneySold = BitTools.intToByteArray((int)sold.getCoin());

			byte[] venBuy = new byte[40];
			venBuy[0] = (byte)venBuy.Length;

			venBuy[4] = (byte)0x04;
			venBuy[6] = (byte)0x3a;

			venBuy[8] = (byte)0x01;
			venBuy[9] = (byte)0x6b;
			venBuy[10] = (byte)0x15;
			venBuy[11] = (byte)0x08;

			for(int i = 0;i < 4;i++)
			{
				venBuy[12 + i] = chid1[i];
				venBuy[20 + i] = chid1[i];
				venBuy[32 + i] = newMoneySold[i];
			}

			venBuy[16] = (byte)0x01;

			venBuy[18] = (byte)0xDB;
			venBuy[19] = (byte)0x2A;

			venBuy[24] = (byte)index;
			venBuy[25] = (byte)invSlot;
			venBuy[26] = (byte)x;
			venBuy[27] = (byte)y;
			venBuy[28] = (byte)amount;
			return venBuy;
		}

		public static byte[] openVendorFrame(Character chr, Dictionary<int, ItemVendor> itemsMap, Dictionary<int, Point> coords, int other, int vendorId)
		{
			byte[] chid = BitTools.intToByteArray(chr.getuID());
		
			byte[] venOpen = new byte[1304];
			byte[] cdatalength = BitTools.intToByteArray(venOpen.Length);
		
			venOpen[0] = (byte)cdatalength[0];
			venOpen[1] = (byte)cdatalength[1];
		
			venOpen[4] = (byte)0x04;
			venOpen[6] = (byte)0x38;
		
			venOpen[8] = (byte)0x01;
			//venOpen[9] = (byte)0x31;
		
			for(int x=0;x<4;x++) {
				venOpen[12+x] = chid[x];
			}
		
			venOpen[16] = (byte)0x01;

			venOpen[20] = (byte)other;
			venOpen[21] = (byte)vendorId;
			int i = 0;
			foreach(KeyValuePair<int, ItemVendor> index in itemsMap)
			{
				foreach(KeyValuePair<int, Point> index1 in coords)
				{
					if(index.Key == index1.Key)
					{
						byte[] itemId = BitTools.intToByteArray(index.Value.getItemFrame().getItemID());
						byte[] price = BitTools.longToByteArray(index.Value.getPrice());
					
						//coords
						venOpen[26+i] = (byte)index1.Value.X;
						venOpen[27+i] = (byte)index1.Value.Y;
						//item ID
						for(int j=0;j<4;j++) {
							venOpen[28+i+j] = itemId[j];
						}
						//Amount
						venOpen[32 + i] = (byte)index.Value.getQuantity();
					
						Console.WriteLine("item " + index.Key + " has price " + index.Value.getPrice() + " \n");

						//Price packets are 8 bytes long
						for(int j=0;j<price.Length;j++) {
							venOpen[792+j+ (i/12)*8] = price[j];
						}
						break;
					}
				}
				i+=12; //size of one item packet
			}	
			return venOpen;
		}

		public static byte[] getVendorListPacket(Character chr)
		{
			List<Character> vendorList = WMap.Instance.getVendingList();
			int vendorSize = vendorList.Count() * 36;
			byte[] chid = BitTools.intToByteArray(chr.getuID());
			byte[] vendorlist = new byte[vendorSize + 14];
			byte[] size = BitTools.intToByteArray(vendorlist.Length);

			for(int i = 0;i < 4;i++)
			{
				vendorlist[i] = size[i];
				vendorlist[9 + i] = chid[i];
			}

			vendorlist[4] = (byte)0x04;
			vendorlist[6] = (byte)0x4a;
			vendorlist[8] = (byte)0x01;

			vendorlist[13] = (byte)0x01;

			if(vendorList.Count() > 0)
			{
				for(int i = 0;i < vendorList.Count();i++)
				{
					Character ch = vendorList[i];
					byte[] bCh = BitTools.intToByteArray(ch.getuID());
					byte[] vendorname = BitTools.stringToByteArray(ch.getVending().getShopName());
					//36 is length of shop
					for(int j = 0;j < 4;j++)
					{
						vendorlist[14 + i * 36 + j] = bCh[j];
					}

					for(int j = 0;j < vendorname.Length;j++)
					{
						vendorlist[18 + i * 36 + j] = vendorname[j];
					}
					vendorlist[49 + i * 36] = (byte)0x08;
				}
			}

			return vendorlist;
		}

		public static byte[] getExtVending(Character chr)
		{
			OutPacket op = new OutPacket(48);
			op.WriteInt(48);
			op.WriteShort(0x05);
			op.WriteShort(0x37);
			op.WriteInt(1);
			op.WriteInt(chr.getuID());
			op.WriteByte(chr.getVending() != null ? (byte)1 : (byte)0);
			if(chr.getVending() != null) op.WriteString(chr.getVending().getShopName());
			return op.ToArray();
		}
	}

	static class ItemPackets
	{
		public static byte[] createDroppedItem(int itemuID, float posX, float posY, int itemID, int itemQuantity)
		{
			OutPacket op = new OutPacket(56);
			op.WriteInt(56);
			op.WriteShort(5);
			op.WriteShort(14);
			op.Skip(12);
			op.WriteInt(itemID);
			op.Skip(4);
			op.WriteInt(itemQuantity);
			op.WriteInt(itemuID);
			op.WriteFloat(posX);
			op.WriteFloat(posY);
			return op.ToArray();
		}

		public static byte[] removeDroppedItemForCharacter(Character chr, int uID)
		{
			OutPacket op = new OutPacket(20);
			op.WriteInt(20);
			op.WriteShort(0x05);
			op.WriteShort(0x0f);
			op.WriteInt(0x01);
			op.WriteInt(chr.getuID());
			op.WriteInt(uID);
			return op.ToArray();
		}
	}

	static class CharacterPackets
	{
		public static byte[] getExtEquipPacket(Character chr, byte slot, int itemID)
		{
			OutPacket op = new OutPacket(24);
			op.WriteInt(24);
			op.WriteShort(0x05);
			op.WriteShort(0x0c);
			op.WriteInt(0x01);
			op.WriteInt(chr.getuID());
			op.WriteInt(itemID);
			op.WriteByte(slot);
			op.WriteByte(0x9e);
			op.WriteByte(0x0f);
			op.WriteByte(0xbf);
			return op.ToArray();
		}

		public static byte[] extCharPacket(Character chr) {
			byte[] cedata = new byte[616];
			short length = (short)cedata.Length;
			byte[] lengthbytes = BitTools.shortToByteArray(length);
			byte[] chID = BitTools.intToByteArray(chr.getuID());
			byte[] chName = Encoding.ASCII.GetBytes(chr.getName());
			byte[] xCoords = BitTools.floatToByteArray(chr.getPosition()[0]);
			byte[] yCoords = BitTools.floatToByteArray(chr.getPosition()[1]);

			cedata[0] = lengthbytes[0];
			cedata[1] = lengthbytes[1];
			cedata[4] = (byte)0x05;
			cedata[6] = (byte)0x01;
			cedata[8] = (byte)0x01;

			for(int i = 0;i < 4;i++) {
				cedata[i + 12] = chID[i]; //character ID
				cedata[i + 88] = xCoords[i]; //location x
				cedata[i + 92] = yCoords[i]; //location y
			}

			for(int i = 0;i < chName.Length;i++) {
				cedata[i + 20] = chName[i]; //characters Name
			}

			for(int i = 0;i < 16;i++) {
				cedata[37 + i] = (byte)0x30; //character packets have 16 times 30(0 in ASCII) in row. Mysteries of CRS.
			}

			if(chr.getcClass() == 2) {
				cedata[60] = (byte)0x02; //gender byte
				cedata[68] = (byte)0x02; //class byte
			}
			else {
				cedata[60] = (byte)0x01; //gender byte
				cedata[68] = (byte)chr.getcClass(); //class byte
			}

			cedata[54] = (byte)chr.getFaction(); //faction

			cedata[62] = (byte)chr.getFace();  //face

			cedata[74] = (byte)chr.getLevel();   //level

			//equip
			byte[] bytes;
			for(byte i = 0;i < 17;i++) {
				if(chr.getEquipment().getEquipments().ContainsKey(i)) {
					bytes = BitTools.intToByteArray(chr.getEquipment().getEquipments()[i].getItemID());
					for(int j = 0;j < 4;j++) {
						cedata[100 + i * 12 + j] = bytes[j];
					}
				}
			}

			//if(ch.getPt() != null && receiver.getPt() != null && ch.getPt().getPartyDuel() != null && ch.getPt().getPartyDuel() == receiver.getPt().getPartyDuel() && ch.isInPtDuel() && receiver.isInPtDuel()) cedata[480] = (byte)2;          //fakekao
			//else cedata[480] = (byte)ch.getKao();      //kao

			cedata[482] = (byte)chr.getEffect();   //size

			cedata[484] = (byte)FameNickNames.Instance.getFameNickID(chr.getFame()); //Fame title

			if(chr.getAccount().gmLvl > 0)
				cedata[486] = (byte)0x01; // gm name

			cedata[610] = (byte)0x50;
			cedata[611] = (byte)0x2a;

			return cedata;
		}

		public static byte[] vanCharPacket(Character chr) {
			OutPacket vanCharData = new OutPacket(20);
			vanCharData.WriteInt(20);
			vanCharData.WriteInt((byte)0x05);
			vanCharData.WriteByte((byte)0x01);
			vanCharData.WriteByte((byte)0x10);
			vanCharData.WriteByte((byte)0xa0);
			vanCharData.WriteByte((byte)0x36);
			vanCharData.WriteInt(chr.getuID());
			vanCharData.WriteByte();
			vanCharData.WriteByte((byte)0xee);
			vanCharData.WriteByte((byte)0x5f);
			vanCharData.WriteByte((byte)0xbf);
			return vanCharData.ToArray();
		}

		public static byte[] initCharPacket(Character chr)
		{
			OutPacket op = new OutPacket(653);
			op.WritePaddedString(chr.getName(), 17); // 0-16
			op.WriteRepeatedByte(0x30, 16); // 17-32
			op.WriteByte();
			op.WriteShort(chr.getFaction()); // 34-35
			op.WriteInt(chr.getFame()); // 36-39
			op.WriteShort(chr.getcClass() == 2 ? (byte)0x02 : (byte)0x01); // 40-41
			op.WriteShort(chr.getFace()); // 42-43
			op.WriteInt(1);
			op.WriteShort(chr.getcClass()); // 48-49
			op.WriteShort(157); // 50-51 ; 1 -> hide player nick above the head & toggles guild on [1] / off [0] | 157 for random 154 warrior [probably guildType or guildIcon!! (important!!)]
			op.WriteShort(2); // 52-53 ; 7 -> 137 monk // your guild pos master/member etc.
			op.WriteShort(chr.getLevel()); // 54-55
			op.WriteInt(chr.getCurHP()); // 56-59
			op.WriteInt(chr.getCurMP()); // 60-63 but wtf.. mana is short o.o
			op.WriteInt(chr.getMap()); // 64-67 let's guess.. a map?
			op.WriteFloat(chr.getPosition()[0]); // 68-71
			op.WriteFloat(chr.getPosition()[1]); // 72-75
			for(byte i = 0;i < 17;i++)
			{
				if(chr.getEquipment().getEquipments().ContainsKey(i))
				{
					op.WriteByte();
					op.WriteByte(chr.getEquipment().getEquipments()[i].getEnding() > 0 ? (byte)0xff : (byte)0);
					op.WriteShort();
					op.WriteInt(chr.getEquipment().getEquipments()[i].getItemID());
					op.WriteInt(chr.getEquipment().getEquipments()[i].getEnding() > 0 ? (int)chr.getEquipment().getEquipments()[i].getEnding() / 1000 : 1);
				} else op.WriteZero(12);
			}

			op.Position = 465;
			op.WriteByte(70);

			op.Position = 314;
			for(byte i = 0;i < 16;i++)
			{
				/*if(character.getBuffs().getBuffs().ContainsKey(i)) {
					op.WriteShort(character.getBuffs().getBuffs()[i].getBuffSlot());
					op.WriteShort(character.getBuffs().getBuffs()[i].getBuffID());
					op.WriteShort(character.getBuffs().getBuffs()[i].getBuffTime());
					op.WriteShort(character.getBuffs().getBuffs()[i].getBuffValue());
				} else*/
				op.Skip(8);
			}

			op.Skip(16);

			op.WriteByte(0); // no explanation
			op.WriteByte(0); // no explanation
			op.WriteByte(/*character.getKao()*/);
			op.WriteByte(/*character.getPenance()*/);
			op.WriteByte(chr.getEffect());
			op.WriteByte(0); // no explanation
			op.WriteByte(FameNickNames.Instance.getFameNickID(chr.getFame()));
			op.WriteByte(chr.getVp()); // 465
			op.WriteByte((byte)chr.getAccount().gmLvl); // 466
			op.WriteByte(0); // no explanation
			op.WriteByte(0); // -> 1 -> tells the client that UI mutation effect should be toggled off | 468
			op.WriteByte((byte)(chr.getInvPages() - 3)); // inventory pages | 469
			op.WriteByte(0); // no explanation
			op.WriteByte(0); // no explanation
			op.WriteShort(0); /*weird PK thing*/ // 472 - 473 but in fact.. it has some date time etc.
			op.WriteLong(0); // 474 - 481

			op.Position = 560;
			op.WriteInt(1); // ok.. so this is kinda weird - last place where you've leveled / died with your character?
			op.WriteFloat(-2558); // unknown posX
			op.WriteFloat(8950); // unknown posY
			op.WriteByte(0); // no explanation
			op.WriteByte(0); // no explanation
			op.WriteByte(0); // no explanation
			op.WriteByte(0); // no explanation
			for(int i = 0;i < chr.getCStats().Length;i++) op.WriteShort(chr.getCStats()[i]); // 576-577 / 578-579 / 580-581 / 582-583 / 584-585
			op.WriteShort(chr.getCurSP());
			op.WriteInt();
			op.WriteInt();
			
			op.WriteLong();
			op.WriteShort(chr.getStatPoints());
			op.WriteShort(chr.getSkillPoints()); // 606-607
			op.Position = 648;
			op.WriteByte(Convert.ToByte(chr.getDeleteState()));
			return op.ToArray();
		}
	}

	static class SkillPackets
	{
		public static byte[] completeCastSkillPacket(byte[] skillpckt, int aoe, int targetId, int hpInt, int manaInt, int dmgInt, int chartargets, int dmgType)
		{
			byte[] hp = BitTools.intToByteArray(hpInt);
			byte[] mana = BitTools.intToByteArray(manaInt);
			byte[] dmg = BitTools.intToByteArray(dmgInt);
			byte[] targetByte = BitTools.intToByteArray(targetId);

			if(chartargets > 0)
				skillpckt[28 + aoe * 24] = (byte)0x01;
			else
				skillpckt[28 + aoe * 24] = (byte)0x02;

			//CharID
			for(int i = 0;i < 4;i++)
			{
				//target
				skillpckt[32 + i + aoe * 24] = targetByte[i];
				skillpckt[40 + i + aoe * 24] = hp[i];
				skillpckt[44 + i + aoe * 24] = dmg[i];
				skillpckt[48 + i + aoe * 24] = mana[i];
			}

			//0=miss,1=normal,2=whitecrit,3/4=buff/nothing,5=greencrit
			skillpckt[36 + aoe * 24] = (byte)dmgType;
			skillpckt[37 + aoe * 24] = (byte)0x00;
			skillpckt[38 + aoe * 24] = (byte)0x00;
			skillpckt[39 + aoe * 24] = (byte)0x00;

			return skillpckt;
		}

		public static byte[] getLearnSkillPacket(Character chr, int skillIdInt, int skillNumberInt)
		{
			OutPacket op = new OutPacket(32);
			op.WriteInt(32);
			op.WriteShort(0x04);
			op.WriteShort(0x29);
			op.WriteInt(0x01);
			op.WriteInt(chr.getuID());
			op.WriteShort(0x01);
			op.WriteByte(0x06);
			op.WriteByte(0x08);
			op.WriteInt(skillNumberInt);
			op.WriteInt(skillIdInt);
			op.WriteShort(chr.getSkillPoints());
			op.WriteByte(0x5f);
			op.WriteByte(0x08);
			return op.ToArray();
		}

		public static byte[] getSkillEffectOnCharPacket(Character chr)
		{
			byte[] cid = BitTools.intToByteArray(chr.getuID());
			byte[] skillpckt = new byte[44];

			skillpckt[0] = (byte)skillpckt.Length;
			skillpckt[4] = (byte)0x05;
			skillpckt[6] = (byte)0x1F;
			skillpckt[8] = (byte)0x01;
			skillpckt[9] = (byte)0x99;
			skillpckt[10] = (byte)0x0F;
			skillpckt[11] = (byte)0xBF;

			//CharID
			for(int i = 0;i < 4;i++)
			{
				skillpckt[12 + i] = cid[i];
			}

			skillpckt[16] = (byte)0x0E;

			//Effid
			skillpckt[20] = (byte)0x00;  //e.g.32
			skillpckt[21] = (byte)0x00;

			//Effduration
			skillpckt[22] = (byte)0x00; //e.g.6e
			skillpckt[23] = (byte)0x00;

			//Effvalue
			skillpckt[24] = (byte)0x00; //e.g.21
			skillpckt[25] = (byte)0x00;

			skillpckt[26] = (byte)0x01;
			skillpckt[28] = (byte)0x87;
			skillpckt[29] = (byte)0x01;
			skillpckt[32] = (byte)0x87;
			skillpckt[33] = (byte)0x01;
			skillpckt[36] = (byte)0x45;
			skillpckt[37] = (byte)0x01;
			skillpckt[38] = (byte)0x45;
			skillpckt[39] = (byte)0x01;
			skillpckt[40] = (byte)0xF2;
			skillpckt[42] = (byte)0xF2;
			return skillpckt;
		}

		public static byte[] getMediPacket(Character chr, int skillID, byte activationID)
		{
			OutPacket op = new OutPacket(28);
			op.WriteInt(28);
			op.WriteShort(5);
			op.WriteShort(0x34);
			op.WriteInt(134521345);
			op.WriteInt(chr.getuID());
			op.WriteInt(135528449);
			op.WriteInt(skillID);
			op.WriteByte(activationID);
			op.WriteShort(3566);
			return op.ToArray();
		}

		public static byte[] getTurboPacket(Character chr, int skillID, bool activate)
		{
			OutPacket op = new OutPacket(28);
			op.WriteInt(28);
			op.WriteShort(5);
			op.WriteShort(0x34);
			op.WriteInt(activate ? 135632385 : 134521345);
			op.WriteInt(chr.getuID());
			op.WriteInt(activate ? -1089535999 : 135528449);
			op.WriteInt(skillID);
			op.WriteInt(activate ? 303818 : 913099);
			return op.ToArray();
		}

		public static byte[] getCastSkillPacket(Character chr, int targets, int skillID, byte activationID)
		{
			OutPacket op = new OutPacket(28 + targets * 24);
			op.WriteInt(28 + targets * 24);
			op.WriteShort(5);
			op.WriteShort(0x34);
			op.WriteInt(1);
			op.WriteInt(chr.getuID());
			op.WriteInt(1);
			op.WriteInt(skillID);
			op.WriteByte(activationID);
			op.WriteByte(7);
			op.WriteByte();
			op.WriteByte((byte)targets);
			return op.ToArray();
		}
	}

	static class StaticPackets
	{
		public static byte[] getUpgradePacket(Character chr, Item newItem, byte newItemIndex, int newItemHash)
		{
			byte x, y, suc;
			if(newItemHash != -1)
			{
				x = (byte)(newItemHash % 100);
				y = (byte)(newItemHash / 100);
				suc = 1;
			}
			else
			{
				x = (byte)0xFF;
				y = (byte)0xFF;
				suc = 0;
			}

			OutPacket op = new OutPacket(36);
			op.WriteInt(36);
			op.WriteShort(0x04);
			op.WriteShort(0x32);
			op.WriteInt(0x01);
			op.WriteInt(chr.getuID());
			op.WriteShort(suc);
			op.WriteByte();
			op.WriteByte(newItemIndex);
			op.WriteInt(134612548);
			op.WriteShort();
			op.WriteByte(y);
			op.WriteByte(x);
			op.WriteInt(newItem.getItemID());
			op.WriteInt(newItem.getQuantity());
			return op.ToArray();
		}

		public static byte[] getInventoryDeletePacket(Character chr, byte invIndex, byte amount)
		{
			OutPacket op = new OutPacket(20);
			op.WriteInt(20);
			op.WriteShort(0x04);
			op.WriteShort(0x15);
			op.WriteInt();
			op.WriteInt(chr.getuID());
			op.WriteShort(0x01);
			op.WriteByte(amount);
			op.WriteByte(invIndex);
			return op.ToArray();
		}

		public static byte[] getPickItemPacket(Character chr, Item item, int amount, int uidInt, byte line, byte row, byte stuff)
		{
			OutPacket op = new OutPacket(40);
			op.WriteInt(40);
			op.WriteShort(0x04);
			op.WriteShort(0x0F);
			op.WriteInt(134519809);
			op.WriteInt(chr.getuID());
			op.WriteShort(0x01);
			op.WriteInt(208335);
			op.WriteShort();
			op.WriteByte(line);
			op.WriteByte(row);
			op.WriteShort(stuff);
			op.WriteShort((short)uidInt);
			op.WriteByte(row);
			op.WriteByte(stuff);
			op.WriteInt(item.getItemID());
			op.WriteInt(amount > 0 ? amount : 1);
			return op.ToArray();
		}

		public static void setMHPoints(MartialClient c, int amount)
		{
			OutPacket p = new OutPacket(12);
			p.WriteInt(12);
			p.WriteShort(3);
			p.WriteShort(8);
			p.WriteInt(amount);
			c.WriteRawPacket(p.ToArray());
		}

		public static void releaseGeneralQuestPacket(Character chr, byte karma = 0x0, byte penance = 0x0, int fame = 0x0, long exp = 0x0, long money = 0x0, byte guildPos = 0x0, byte faction = 0x0)
		{
			OutPacket op = new OutPacket(64);
			op.WriteInt(64);
			op.WriteShort(0x05);
			op.WriteShort(0x3b);
			op.WriteInt(715218689);
			op.WriteInt(chr.getuID());
			op.WriteByte(0x0); // karma
			op.WriteByte(0x0); // penance
			op.WriteShort(); // ?
			op.WriteByte(FameNickNames.Instance.getFameNickID(fame != 0x0 ? fame : chr.getFame()) > 0 ? (byte)1 : (byte)0); // appear player name above the head
			op.WriteByte(FameNickNames.Instance.getFameNickID(fame != 0x0 ? fame : chr.getFame())); // fame nickname
			op.WriteByte(chr.getAccount().gmLvl > 0 ? (byte)1 : (byte)0); // blue nickname??
			op.WriteByte(4); // ?
			op.WriteByte(4); // ?
			op.WriteByte(4); // ?
			op.WriteByte(4); // ?
			op.WriteByte(4); // ?
			op.WriteInt(fame != 0x0 ? fame : chr.getFame()); // fame
			op.WriteReversedLong(exp != 0x0 ? exp : chr.getExp()); // exp
			op.WriteLong(money != 0x0 ? money : chr.getCoin()); // money
			op.WriteByte(guildPos != 0x0 ? guildPos : (byte)0x0); // Guild Pos | TODO
			op.WriteByte(faction != 0x0 ? faction : chr.getFaction()); // Faction
			op.WriteByte(2); // 0 - gives an yellow message (?) | 1 - hides inventory
			Console.WriteLine(BitConverter.ToString(op.ToArray()));
			chr.getAccount().mClient.WriteRawPacket(op.ToArray());
		}

		public static void setCharacterLevel(Character chr, byte level)
		{
			OutPacket p = new OutPacket(40);
			p.WriteInt(40);
			p.WriteShort(0x05);
			p.WriteShort(0x20);
			p.WriteByte(0x01);
			p.WriteByte(0x39);
			p.WriteByte(0x07);
			p.WriteByte(0x08);
			p.WriteInt(chr.getuID());
			p.WriteShort(level);
			p.WriteShort(chr.getStatPoints());
			p.WriteInt(chr.getSkillPoints());
			p.WriteInt(chr.getMaxHP());
			p.WriteShort((short)chr.getMaxMP());
			p.WriteShort(chr.getMaxSP());
			chr.getAccount().mClient.WriteRawPacket(p.ToArray());

			chr.setLevel(level);
		}

		public static void updateStatsAttributes(Character chr, short[] attributes = null, short sp = -1)
		{
			if(attributes != null) {
				chr.setStr(attributes[0]);
				chr.setDex(attributes[1]);
				chr.setVit(attributes[2]);
				chr.setInt(attributes[3]);
				chr.setAgi(attributes[4]);
			}
			if(sp != -1) {
				chr.setStatPoints(sp);
			}

			CharacterFunctions.calculateCharacterStatistics(chr);

			OutPacket p = new OutPacket(32);
			p.WriteInt(32);
			p.WriteShort(0x04);
			p.WriteShort(0x1d);
			p.WriteInt(0x01);
			p.WriteInt(chr.getuID());
			p.WriteShort(0x01);
			p.WriteShort(chr.getStr()); // Strength
			p.WriteShort(chr.getDex()); // Dextery
			p.WriteShort(chr.getVit()); // Vitality
			p.WriteShort(chr.getInt()); // Intelligence
			p.WriteShort(chr.getAgi()); // Agility
			p.WriteShort(chr.getStatPoints()); // statusPoints
			p.WriteByte(0x40);
			p.WriteByte(0x2a);
			chr.getAccount().mClient.WriteRawPacket(p.ToArray());
		}

		public static void releaseHealPacket(Character chr, int hpParam = 1000, short mpParam = 1000, short spParam = 1000)
		{
			if(hpParam > chr.getMaxHP())
			{
				hpParam = chr.getMaxHP();
				chr.setCurHP(chr.getMaxHP());
			}
			else
			{
				chr.setCurHP(hpParam);
			}

			if(mpParam > chr.getMaxMP())
			{
				mpParam = chr.getMaxMP();
				chr.setCurMP(chr.getMaxMP());
			}
			else
			{
				chr.setCurMP(mpParam);
			}

			if(spParam > chr.getMaxSP())
			{
				spParam = chr.getMaxSP();
				chr.setCurSP(chr.getMaxSP());
			}
			else
			{
				chr.setCurSP(spParam);
			}

			OutPacket p = new OutPacket(32);
			p.WriteInt(32);
			p.WriteShort(0x05);
			p.WriteShort(0x35);
			p.WriteInt(1158393864);
			p.WriteInt(chr.getuID());
			p.WriteInt(131076);
			p.WriteInt();
			p.WriteInt(hpParam);
			p.WriteShort(mpParam);
			p.WriteShort(spParam);
			chr.getAccount().mClient.WriteRawPacket(p.ToArray());
		}

		public static byte[] playerIsntConnected(Character chr)
		{
			OutPacket p = new OutPacket(48);
			p.WriteInt(48);
			p.WriteShort(0x05);
			p.WriteShort(0x07);
			p.WriteInt(0x01);
			p.WriteInt(chr.getuID());
			p.WriteZero(2);
			p.WriteShort(0x01);
			p.WriteZero(20);
			p.WriteInt(0x04);
			p.WriteByte(0x34);
			p.WriteByte(0x33);
			p.WriteByte(0x34);
			p.WriteByte(0x33);
			return p.ToArray();
		}

		public static byte[] chatRelay(Character chr, byte messageType, string message)
		{
			OutPacket p = new OutPacket(message.Length + 44);
			p.WriteInt(message.Length + 44);
			p.WriteShort(0x05);
			p.WriteShort(0x07);
			p.WriteInt(0x01);
			p.WriteInt(chr.getuID());
			p.WriteZero(1);
			p.WriteByte(0x01);
			p.WriteShort(messageType);
			p.WritePaddedString(chr.getName(), 20);
			p.WriteInt(message.Length);
			p.WriteString(message);
			return p.ToArray();
		}

		public static void setCharacterFame(Character chr, int fameAmount) {
			OutPacket p = new OutPacket(192);
			p.WriteInt(192);
			p.WriteShort(0x04);
			p.WriteShort(0x64);
			p.WriteInt(1);
			p.WriteInt(chr.getuID());
			p.WriteShort(0x05);
			p.WriteShort(0x05);
			p.WriteInt(0x0F);
			p.WriteInt(fameAmount);

			chr.setFame(fameAmount);
			chr.getAccount().mClient.WriteRawPacket(p.ToArray());
		}

		public static void sendSystemMessageToClient(MartialClient c, byte messageType, string message, string sender = null) {
			OutPacket p = new OutPacket(45 + message.Length);
			p.WriteInt(45 + message.Length);
			p.WriteShort(0x05);
			p.WriteShort(0x07);
			p.WriteShort(0x01);
			p.WriteZero(7);
			p.WriteByte(0x01);
			p.WriteShort(messageType);
			if(sender == null)
				p.WritePaddedString("*", 20);
			else
				p.WritePaddedString(sender, 20);
			p.WriteInt(0x3e);
			p.WriteString(message);
			c.WriteRawPacket(p.ToArray());
		}

		public static void sendWorldAnnounce(string message) {
			OutPacket p = new OutPacket(message.Length + 14);
			p.WriteInt(message.Length + 14);
			p.WriteShort(0x03);
			p.WriteByte(0x50);
			p.WriteShort(0xc3);
			p.WriteInt(message.Length);
			p.WriteString(message);

			foreach(Character chara in WMap.Instance.getWorldCharacters()) {
				chara.getAccount().mClient.WriteRawPacket(p.ToArray());
			}
		}
	}

	static class MoveCharacterPacket {
		public static void HandleMovement(Character chr, byte[] tx, byte[] ty, byte mMode) {
			if(chr == null) {
				Logger.LogCheat(Logger.HackTypes.NullActive, chr.getAccount().mClient, "Wrong target has been selected by moving packet");
				chr.getAccount().mClient.Close();
				return;
			}

			Area lastArea = chr.getArea();
			Boolean nullify = false;
			if(lastArea == null)
				nullify = true;

			Area newArea = WMap.Instance.getGrid(chr.getMap()).getAreaByRound(chr.getPosition()[0], chr.getPosition()[1]);

			if(newArea == null)
			{
				CharacterFunctions.warpToNearestTown(chr);
				return;
			}

			if(!nullify) {
				if(lastArea != newArea) {
					lastArea.removeCharacter(chr);
					newArea.addCharacter(chr);
					chr.setArea(newArea);
				}
			}
			else if(nullify) {
				newArea.addCharacter(chr);
				chr.setArea(newArea);
			}

			OutPacket p = new OutPacket(56);
			p.WriteInt(56);
			p.WriteShort(0x04);
			p.WriteShort(0x0d);
			p.WriteInt	();
			p.WriteInt	(chr.getuID());
			p.WriteFloat(chr.getPosition()[0]);
			p.WriteFloat(chr.getPosition()[1]);
			p.WriteBytes(tx); //2nd set 
			p.WriteBytes(ty);
			p.WriteInt	(newArea.getaID());
			p.WriteShort();
			p.WriteByte	((byte)0x80);
			p.WriteByte	((byte)0x3f);
			p.WriteByte	(mMode);
			p.WriteByte	((byte)0x03);
			p.WriteByte	((byte)0x05);
			p.WriteByte	((byte)0x08);
			p.WriteInt	(chr.getCurHP());
			p.WriteShort(chr.getCurMP());
			p.WriteShort(chr.getCurSP());
			p.WriteInt(newArea.getRegionID());
			chr.getAccount().mClient.WriteRawPacket(p.ToArray());

			OutPacket externalMovement = new OutPacket(48);
			externalMovement.WriteInt(48);
			externalMovement.WriteShort(0x05);
			externalMovement.WriteShort(0x0d);
			externalMovement.WriteInt((newArea != lastArea) ? (0x01) : (-1084232447));
			externalMovement.WriteInt(chr.getuID());
			externalMovement.WriteInt(1078117293);
			externalMovement.WriteFloat(chr.getPosition()[0]);
			externalMovement.WriteFloat(chr.getPosition()[1]);
			externalMovement.WriteBytes(tx);
			externalMovement.WriteBytes(ty);
			externalMovement.WriteShort(mMode); // who knows? | 36
			externalMovement.WriteShort((short)newArea.getaID());
			externalMovement.WriteShort();
			externalMovement.WriteByte(0x80);
			externalMovement.WriteByte(0x3f);
			externalMovement.WriteInt((newArea != lastArea) ? (0x05) : (0x03));

			WMap.Instance.getGrid(chr.getMap()).sendTo3x3AreaMovement(chr, newArea, externalMovement.ToArray());

			bool areaTriggered = false;
			for(int i = 0;i < 3;i++)
			{
				for(int u = 0;u < 3;u++)
				{
					Area nearCentral = WMap.Instance.getGrid(chr.getMap()).getArea(new int[] { newArea.getAreaPosition()[0] - 1 + i, newArea.getAreaPosition()[1] - 1 + u });
					if(nearCentral == null) continue;
					if(nearCentral.getAreaTriggers().Count() == 0) continue;
					{
						foreach(AreaTrigger areaTrigger in nearCentral.getAreaTriggers())
						{
							if((WMap.distance(chr.getPosition()[0], chr.getPosition()[1], areaTrigger.getFromPosition()[0], areaTrigger.getFromPosition()[1]) > 35)) continue;
							if(!CharacterFunctions.isCharacterWearingItem(chr, areaTrigger.getRequiredItem())) continue;
							try
							{
								areaTriggered = true;
								CharacterFunctions.setPlayerPosition(chr, areaTrigger.getToPosition()[0], areaTrigger.getToPosition()[1], areaTrigger.gettMap());
							}
							catch(Exception e)
							{
								Console.WriteLine(e);
							}
						}
					}
				}
			}
			if(!areaTriggered) chr.setPosition(new float[] { (float)BitTools.byteArrayToFloat(tx), (float)BitTools.byteArrayToFloat(ty) });
		}
	}

	static class NPCSpawnChain
	{
		public static void initNPCsForCharacter(Character chr, List<NPC> npcs)
		{
			if(npcs.Count == 0)
				return;

			OutPacket all = new OutPacket((npcs.Count() * 589) + 30);
			all.WriteInt((npcs.Count() * 589) + 30);
			all.WriteShort(4);
			all.WriteShort(4);
			all.WriteByte(1);
			all.WriteInt(chr.getuID());
			all.WriteInt(chr.getArea().getaID());
			all.WriteFloat(chr.getPosition()[0]);
			all.WriteFloat(chr.getPosition()[1]);
			foreach(NPC npc in npcs)
			{
				all.WriteBytes(npc.npcSpawnChained(chr));
			}
			all.WriteByte();
			all.WriteInt(3560);
			chr.getAccount().mClient.WriteRawPacket(all.ToArray());

			Console.WriteLine("npc init package length: {0}", all.ToArray().Length);
		}
	}

	static class LoginPacketCreator
	{
		public static byte[] initAccount(Account acc)
		{
			OutPacket op = new OutPacket(52);
			op.WriteInt(52);
			op.WriteShort(3);
			op.WriteShort(5);
			op.WritePaddedString(acc.name, 24);
			op.WriteInt(109);
			op.WriteInt(acc.MHPoints);
			op.WriteLong();
			op.WriteInt(acc.characters != null ? acc.characters.Count : 0);
			return op.ToArray();
		}

		public static byte[] initCharacters(Account acc, bool backSpawn = false) {
			OutPacket all = new OutPacket((acc.characters.Count() * 653) + 8 + 3);
			all.WriteInt((acc.characters.Count() * 653) + 8 + 3);
			all.WriteShort(0x03);
			all.WriteShort((!backSpawn) ? ((byte)0x04) : ((byte)0x01));
			all.WriteBytes(new byte[] { (byte)0x01, (byte)0x01, (byte)0x01 });

			int multipier_keeper = 0x01;
			Boolean multiplied = false;

			foreach(Character chara in acc.characters.Values) {
				all.WriteBytes(CharacterPackets.initCharPacket(chara));

				if(multiplied)
					multipier_keeper = (multipier_keeper * 2) + 1;
				if(!multiplied)
					multiplied = true;
			}

			all.Position = 10;
			all.WriteByte((byte)multipier_keeper);
			return all.ToArray();
		}
	}
}
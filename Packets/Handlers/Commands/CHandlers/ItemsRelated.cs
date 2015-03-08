using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gameServer.Game;
using gameServer.Tools;
using gameServer.Game.Misc;
using gameServer.Game.World;
using gameServer.Game.Caches;
using gameServer.Game.Objects;

namespace gameServer.Packets.Handlers.Commands.CHandlers
{
	public class ItemsRelated
	{
		public static void ItemCreate(MartialClient c, InCommand cmd)
		{
			if(cmd.commandArgs == null)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "/item [itemid] [*amount]");
				return;
			}

			if(c.getAccount().activeCharacter == null)
			{
				Logger.LogCheat(Logger.HackTypes.NullActive, c, "Null activity in command handler");
				c.Close();
				return;
			}
			Character chr = c.getAccount().activeCharacter;

			int itemID = 0;
			Int32.TryParse(cmd.commandArgs[0], out itemID);
			if(itemID < 200000000 || itemID > 299999999)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Item ID range goes from 200.000.000 - 299.999.999.");
				return;
			}
			if(ItemDataCache.Instance.getItemData(itemID).getID() == 0)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Selected item wasn't found.");
				return;
			}

			short itemQuantity = 1;
			Int16.TryParse(cmd.commandArgs[1], out itemQuantity);
			if(itemQuantity > short.MaxValue)
			{
				StaticPackets.sendSystemMessageToClient(c, 1, "Items amount, can't be bigger than 100!");
				return;
			}

			c.WriteRawPacket(ItemPackets.createDroppedItem(WMap.Instance.items.Count, chr.getPosition()[0], chr.getPosition()[1], itemID, itemQuantity));
			StaticPackets.sendSystemMessageToClient(c, 1, "Item of ID: " + itemID + "|" + WMap.Instance.items.Count + "|" + itemQuantity + ", has been created at coords: ");
			StaticPackets.sendSystemMessageToClient(c, 1, chr.getPosition()[0] + ":" + chr.getPosition()[1] + ":" + chr.getMap() + "!");
			WMap.Instance.items.Add(WMap.Instance.items.Count, new Item(itemID, itemQuantity));
			return;
		}
	}
}

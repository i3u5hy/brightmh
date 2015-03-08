using System;
using System.Collections.Generic;
using System.Net.Sockets;
using gameServer.Core.Network;
using gameServer.Game;
using gameServer.Packets;
using gameServer.Packets.Handlers.Commands;
using gameServer.Tools;
using gameServer.Tools.externalDataProvider;
using gameServer.Tools.vfsDataProvider;

namespace gameServer.Servers
{
	class HellEmissary
	{
		private Acceptor m_acceptor;
		private List<MartialClient> m_clients;
		private PacketProcessor m_processor;

		public HellEmissary(short port)
		{
			m_acceptor = new Acceptor(port);
			m_acceptor.OnClientAccepted = OnClientAccepted;

			m_clients = new List<MartialClient>();

			PacketHandlers();
			CommandProcessor.InitCommandHandlers();
		}

		public void SendBytes(byte[] packet) {
			foreach (MartialClient mc in m_clients) {
				mc.WriteRawPacket(packet);
			}
		}

		private void PacketHandlers()
		{
			m_processor = new PacketProcessor("MainProcessor");

			m_processor.AppendHandler(1,	Packets.Handlers.LoginHandler.Ping);
			m_processor.AppendHandler(2,	Packets.Handlers.LoginHandler.LauncherValidate);
			m_processor.AppendHandler(670,	Packets.Handlers.LoginHandler.InvalidVersion);

			m_processor.AppendHandler(666,	Packets.Handlers.Selection.Quit);
			m_processor.AppendHandler(672,	Packets.Handlers.Selection.CreateNewCharacter);
			m_processor.AppendHandler(673,	Packets.Handlers.Selection.RemoveCharacter);
			m_processor.AppendHandler(675,	Packets.Handlers.Selection.RequestSpawn);
			m_processor.AppendHandler(680,	Packets.Handlers.Selection.MoveToVV);
			m_processor.AppendHandler(1332, Packets.Handlers.Selection.ReturnToSelection);

			m_processor.AppendHandler(1337, Packets.Handlers.InventoryManagement.UseItem);
			m_processor.AppendHandler(1344, Packets.Handlers.InventoryManagement.Equip);
			m_processor.AppendHandler(1346, Packets.Handlers.InventoryManagement.Drop);
			m_processor.AppendHandler(1347, Packets.Handlers.InventoryManagement.Pickup);
			m_processor.AppendHandler(1348, Packets.Handlers.InventoryManagement.MoveOrUnequip);
			m_processor.AppendHandler(1351, Packets.Handlers.InventoryManagement.BuyFromNPC);
			m_processor.AppendHandler(1352, Packets.Handlers.InventoryManagement.SellToNPC);
			m_processor.AppendHandler(1353, Packets.Handlers.InventoryManagement.DeleteItem);
			m_processor.AppendHandler(1362, Packets.Handlers.InventoryManagement.ViewInventory);
			m_processor.AppendHandler(1372, Packets.Handlers.InventoryManagement.CraftItem);
			m_processor.AppendHandler(1382, Packets.Handlers.InventoryManagement.UpgradeItem);
			m_processor.AppendHandler(1407, Packets.Handlers.InventoryManagement.MHShop);

			m_processor.AppendHandler(1338, Packets.Handlers.PlayerState.AttackDefendState);
			m_processor.AppendHandler(1339, Packets.Handlers.PlayerState.Chat);
			m_processor.AppendHandler(1345, Packets.Handlers.PlayerState.Movement);
			m_processor.AppendHandler(1456, Packets.Handlers.PlayerState.ToggleMutationEffect);
			m_processor.AppendHandler(1379, Packets.Handlers.PlayerState.UnknownStatimizer); // Both of them - unsure
			m_processor.AppendHandler(1438, Packets.Handlers.PlayerState.Fury);
			//m_processor.AppendHandler(1444, Packets.Handlers.PlayerState._buffie2); // Both of them - unsure

			m_processor.AppendHandler(1381, Packets.Handlers.CommunityManagement.HandleFriends);
			m_processor.AppendHandler(1386, Packets.Handlers.CommunityManagement.RefreshFriends);
			m_processor.AppendHandler(1415, Packets.Handlers.CommunityManagement.SendMessage);

			m_processor.AppendHandler(1349, Packets.Handlers.SkillManagement.ShortcutBar);
			m_processor.AppendHandler(1373, Packets.Handlers.SkillManagement.LearnSkill);
			m_processor.AppendHandler(1384, Packets.Handlers.SkillManagement.CastSkill);

			m_processor.AppendHandler(1376, Packets.Handlers.CargoManagement.MoveFromInv);
			m_processor.AppendHandler(1377, Packets.Handlers.CargoManagement.MoveToInv);
			m_processor.AppendHandler(1378, Packets.Handlers.CargoManagement.Move);

			m_processor.AppendHandler(1387, Packets.Handlers.VendingManagement.StateVending);

			m_processor.AppendHandler(1393, Packets.Handlers.GuildManagement.CreateGuild);
			m_processor.AppendHandler(1397, Packets.Handlers.GuildManagement.Refresh); // Both of them - unsure
			m_processor.AppendHandler(1413, Packets.Handlers.GuildManagement.DeclareWar); // Both of them - unsure
			m_processor.AppendHandler(1435, Packets.Handlers.GuildManagement.UpdateNews);
		}

		private bool InitialiseWorld()
		{
			if(!Constants.VFSSkip)
				if(!vfsDataProvider.Instance.initVFSloader()) {
					Console.WriteLine("VFS Providing, couldn't be initialised!");
					MasterServer.Instance.Shutdown();
					return false;
				}

			mobProviding.loadMobs();
			if(!mapProviding.loadMaps()) return false;
			npcProviding.loadNPCs();
			furyProviding.loadFury();
			itemProviding.loadItems();
			newCharInventories.loadNewCharacterInventories();
			expProviding.loadExpRelated();
			fameProviding.loadFameNicknames();
			shopItemProviding.loadItemShop();
			upgradeProviding.loadUprades();
			manualProviding.loadManuals();
			skillProviding.loadSkills();

			if(Constants.clearCache && !Constants.VFSSkip)
				vfsDataProvider.Instance.Clear();
			return true;
		}
		
		private void OnClientAccepted(Socket client)
		{
			MartialClient mc = new MartialClient(client, m_processor, m_clients.Remove);
			m_clients.Add(mc);
			mc.WriteRawPacket(Constants.authSuccess);
			Logger.WriteLog(Logger.LogTypes.HEmi, "Client {0} connected to Hell Emissary.", mc.Label);
		}

		public bool Run()
		{
			if(!InitialiseWorld())
				return false;

			m_acceptor.Start();
			Logger.WriteLog(Logger.LogTypes.HEmi, "HellEmissary		- {0}.", m_acceptor.Port);
			return true;
		}

		public bool checkIfClientExists(int aID)
		{
			foreach(MartialClient i in m_clients)
			{
				if(i.getAccount() == null) continue;
				if(i.getAccount().aID == aID) return true;
			}
			return false;
		}

		public void Shutdown()
		{
			m_acceptor.Stop();
			//foreach (MartialClient mc in m_clients)
			//	mc.Close();
		}
	}
}
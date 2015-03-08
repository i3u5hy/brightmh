﻿using gameServer.Core.IO;
using gameServer.Game.World;
using gameServer.Tools;

namespace gameServer.Packets.Handlers {
	class RefreshArea {
		public static void _buffie2(MartialClient c, InPacket p) {
			c.WriteRawPacket(new byte[] {(byte)0x09, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x02, (byte)0x00, (byte)0x36, (byte)0x00, (byte)0xf6, (byte)0x09, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x02, (byte)0x00, (byte)0x41, (byte)0x00, (byte)0xf6, (byte)0x09, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x02, (byte)0x00, (byte)0x70, (byte)0x00, (byte)0xec, (byte)0x10, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x02, (byte)0x00, (byte)0x06, (byte)0x00, (byte)0x80, (byte)0x4b, (byte)0x7d, (byte)0xce, (byte)0x83, (byte)0x11, (byte)0xf7, (byte)0x59});
		}
	}
}
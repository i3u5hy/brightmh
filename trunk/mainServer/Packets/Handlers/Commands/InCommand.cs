using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gameServer.Packets.Handlers.Commands
{
	public class InCommand
	{
		public string	commandName;
		public string[] commandArgs;

		public InCommand(string commandName, string[] commandArgs)
		{
			this.commandName = commandName;
			this.commandArgs = commandArgs;
		}
	}
}

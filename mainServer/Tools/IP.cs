using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace gameServer.Tools
{
	class IP
	{
		public static string ExternalIPAddress()
		{
			return (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches((new WebClient()).DownloadString("http://checkip.dyndns.org/"))[0].ToString();
		}

		public string LocalIPAddress()
		{
			IPHostEntry host;
			string localIP = "";
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					localIP = ip.ToString();
					break;
				}
			}
			return localIP;
		}
	}
}

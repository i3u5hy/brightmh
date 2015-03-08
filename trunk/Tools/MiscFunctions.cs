using System;

namespace gameServer.Tools
{
	public class MiscFunctions
	{
		public static string obscureString(string name)
		{
			if(name == null)
				return null;
			int i = name.IndexOf('\0');
			if(i >= 0) name = name.Substring(0, i);
			return name;
		}

		public static Boolean IsNumeric(string param)
		{
			int valueParsed;
			if(Int32.TryParse(param, out valueParsed))
				return true;
			return false;
		}

		private static readonly DateTime Jan1st1970 = new DateTime
				(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static long CurrentTimeMillis()
		{
			return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
		}

		public static long GetTime(long realTimestamp)
		{
			if(realTimestamp == -1L)
			{
				return 150842304000000000L;
			}
			if(realTimestamp == -2L)
			{
				return 94354848000000000L;
			}
			if(realTimestamp == -3L)
			{
				return 150841440000000000L;
			}
			return realTimestamp * 10000L + 116444592000000000L;
		}
	}
}
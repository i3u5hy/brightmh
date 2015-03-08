using System;
using System.IO;
using System.Text;

namespace gameServer.Tools
{
	public static class FileWriter
	{
		public static void Write(string pFilename, string pWhat, bool pWriteToConsole = false)
		{
			FileStream fs = new FileStream(pFilename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			byte[] dat = Encoding.ASCII.GetBytes(pWhat);
			fs.Write(dat, 0, dat.Length);
			fs.Flush();
			fs.Close();
			fs.Dispose();

			if (pWriteToConsole) Console.Write(pWhat);
		}

		public static void WriteLine(string pFilename, string pWhat, bool pWriteToConsole = false)
		{
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(pFilename));
				pWhat += "\r\n";
				FileStream fs = new FileStream(pFilename, FileMode.Append, FileAccess.Write, FileShare.Write);
				byte[] dat = Encoding.ASCII.GetBytes(pWhat);
				fs.Write(dat, 0, dat.Length);
				fs.Flush();
				fs.Close();
				fs.Dispose();

				if (pWriteToConsole) Console.Write(pWhat);
			}
			catch(Exception ex) {
				Logger.WriteLog(Logger.LogTypes.Error, "Error while writing to file {0}", ex);
			}
		}
	}
}

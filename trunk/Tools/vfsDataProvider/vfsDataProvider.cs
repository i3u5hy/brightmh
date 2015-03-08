using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using gameServer.Servers;

namespace gameServer.Tools.vfsDataProvider {
	public sealed class vfsDataProvider {
		private static readonly vfsDataProvider instance = new vfsDataProvider();
		private vfsDataProvider() {
		}

		public static vfsDataProvider Instance
		{
			get
			{
				return instance;
			}
		}

		public List<string> fileNames;
		public List<Tuple<Int64, Int64>> offLength;
		public List<string> pureFileNames;
		public string vfsPath = AppDomain.CurrentDomain.BaseDirectory + "data\\data.vfs";
		public string infPath = AppDomain.CurrentDomain.BaseDirectory + "data.inf";

		public void Clear()
		{
			fileNames.Clear();
			offLength.Clear();
			pureFileNames.Clear();
			vfsPath = null;
			infPath = null;
		}

		public Boolean initVFSloader() {
			if(initINFfiles(Constants.gameDirectory)) {
				loadVFSfiles();
				return true;
			}
			return false;
		}

		public Boolean initINFfiles(string gamePath = null) {
			if(gamePath != null) {
				if(Directory.Exists(gamePath)) {
					if(File.Exists(gamePath + "data.inf")) {
						if(File.Exists(gamePath + "data\\data.vfs")) {
							this.infPath = gamePath + "data.inf";
							this.vfsPath = gamePath + "data\\data.vfs";
							return true;
						}
					}
				}
			}
			if(File.Exists("data.inf")) {
				if(File.Exists("data\\data.vfs")) {
					this.infPath = AppDomain.CurrentDomain.BaseDirectory + "data.inf";
					this.vfsPath = AppDomain.CurrentDomain.BaseDirectory + "data\\data.vfs";
					return true;
				}
			}
			return false;
		}

		public Boolean loadVFSfiles() {
			byte[] bytes = File.ReadAllBytes(infPath);
			if(bytes.Length < 24) {
				Console.WriteLine("The file you provided is either wrong or corrupt");
				return false;
			}
			else {
				Int64 position = 12;
				byte[] fileCount = new byte[4];
				for(int i = 0;i < 4;i++) {
					fileCount[i] = bytes[position + i];
				}

				int files = BitConverter.ToInt32(fileCount, 0);

				fileNames = new List<string>(files);
				offLength = new List<Tuple<Int64, Int64>>(files);
				pureFileNames = new List<string>(files);

				position = 24;

				while(position < bytes.Length) {
					byte[] name = new byte[104];
					for(int i = 0;i < 104;i++) {
						if(bytes[position + i] == 0x00) {
							break;
						}
						name[i] = bytes[position + i];
					}
					byte[] offset = new byte[8];
					byte[] length = new byte[8];

					for(int i = 0;i < 8;i++) {
						offset[i] = bytes[position + i + 104];
						length[i] = bytes[position + i + 104 + 8];
					}

					string bundeswehr = System.Text.Encoding.Default.GetString(name).Replace("\0", string.Empty);
					try {
						fileNames.Add(bundeswehr.Replace("/", "\\"));
						pureFileNames.Add(Path.GetFileName(bundeswehr.Replace("/", "\\")));
						offLength.Add(new Tuple<Int64, Int64>(BitConverter.ToInt64(offset, 0), BitConverter.ToInt64(length, 0)));
					}
					catch(ArgumentException) {
						Console.WriteLine("It seems, that you've been using older version of Martial Heroes,");
						Console.WriteLine("than the required one.");
						Console.WriteLine("Press any key to exit.");
						Console.ReadKey();
						MasterServer.Instance.Shutdown();
					}

					position += 144;
				}
				return true;
			}
		}

		public void unpackFromVFS(string vfsFile, string unpackedFile) {
			FileStream fstream = null;
			if(File.Exists(unpackedFile))
				return;
			try {
				fstream = new FileStream(vfsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			}
			catch(FileNotFoundException) {
				/*Console.WriteLine("File: " + vfsPath + " could not be found");*/
				return;
			}

			int indexar = fileNames.IndexOf(vfsFile);
			if(indexar == -1) {
				return;
			}
			else {
				Int64 offseti = offLength.ElementAt(indexar).Item1;
				Int64 lengthi = offLength.ElementAt(indexar).Item2;
				byte[] fileData = new byte[lengthi];

				fstream.Seek(offseti, SeekOrigin.Begin);
				for(Int64 fdata = 0;fdata < lengthi;fdata++) {
					fileData[fdata] = (byte)fstream.ReadByte();
				}
				try {
					File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\" + unpackedFile, fileData);
				}
				catch(Exception) {
					Console.WriteLine("Could not write to the file: " + unpackedFile);
					return;
				}
			}
		}
	}
}

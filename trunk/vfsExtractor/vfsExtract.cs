 using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

class vfsExtract
{
	enum MHTypes
	{
		None,
		MartialHeroes,
		DOOnline
	}

	[STAThread]
	static void Main(string[] args)
	{
		Console.Title = "MH Extractor";

		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Data file (.inf)|*.inf";
		openFileDialog.FilterIndex = 1;

OpenInfFile:
		Console.WriteLine("Provide an .inf file path.");

		string dataInfPath = null;
		DialogResult _openDataInf = DialogResult.None;
		while(_openDataInf != DialogResult.OK)
		{
			_openDataInf = openFileDialog.ShowDialog();
			if(_openDataInf == DialogResult.Cancel)
			{
				Environment.Exit(0);
				return;
			}
			dataInfPath = openFileDialog.FileName;
		}
		Console.WriteLine("Selected .inf file: {0}.", dataInfPath);

		byte[] bytes = File.ReadAllBytes(dataInfPath);
		if(bytes.Length < 24)
		{
			Console.WriteLine("The file you provided is either wrong or corrupt.");
			goto OpenInfFile;
		}

		MHTypes _mhType = MHTypes.None;
		_mhType = bytes[20] == 0x0 ? MHTypes.DOOnline : MHTypes.MartialHeroes;
		if(_mhType == MHTypes.None)
		{
			Environment.Exit(0);
			return;
		}

		int files = BitConverter.ToInt32(bytes, 12);
		Console.WriteLine("Files found: {0}.", files);

		List<string> fileNames = new List<string>(files);
		List<Tuple<Int64, int>> offLength = new List<Tuple<Int64, int>>(files);

		int position = 0;
		switch(_mhType)
		{
			case MHTypes.DOOnline:
				position = 24;
				break;
			case MHTypes.MartialHeroes:
				position = 20;
				break;
		}
		while(position < bytes.Length)
		{
			string name = "";
			for(int i = 0;i < 104;i++)
			{
				if(bytes[position + i] == 0x00)
				{
					break;
				}
				name += (char)bytes[position + i];
			}

			try
			{
				fileNames.Add(name);
				switch(_mhType)
				{
					case MHTypes.DOOnline:
						offLength.Add(new Tuple<Int64, int>(BitConverter.ToInt64(bytes, position + 104), BitConverter.ToInt32(bytes, position + 112)));
						break;
					case MHTypes.MartialHeroes:
						offLength.Add(new Tuple<Int64, int>(BitConverter.ToInt32(bytes, position + 100), BitConverter.ToInt32(bytes, position + 104)));
						break;
				}
			}
			catch(ArgumentException)
			{
				Console.WriteLine("It seems, that you've choosed other version of Martial Heroes inf file, than the required one.");
				Environment.Exit(0);
				return;
			}

			position += _mhType == MHTypes.DOOnline ? 144 : _mhType == MHTypes.MartialHeroes ? 132 : 0;
		}

		if(fileNames.Count == 0)
		{
			Console.WriteLine("Looks like, you've provided wrong .inf file.");
			goto OpenInfFile;
		}

		Console.WriteLine("Select a destination path.");

		FolderBrowserDialog folderDialog = new FolderBrowserDialog();

		string destinationPath = null;
		_openDataInf = DialogResult.None;
		while(_openDataInf != DialogResult.OK)
		{
			_openDataInf = folderDialog.ShowDialog();
			if(_openDataInf == DialogResult.Cancel)
			{
				Environment.Exit(0);
				return;
			}
			destinationPath = folderDialog.SelectedPath;
		}
		Console.WriteLine("Selected a destination path:\r\n{0}.", destinationPath);

		BinaryReader fstream = new BinaryReader(File.OpenRead(/*vfsPath*/Path.ChangeExtension(Path.GetDirectoryName(dataInfPath) + "/data/" + Path.GetFileName(dataInfPath), ".vfs")));
		for(int i = 0;i < fileNames.Count;i++)
		{
			if(!Directory.Exists(destinationPath + "\\" + Path.GetDirectoryName(fileNames[i])))
			{
				Directory.CreateDirectory(destinationPath + "\\" + Path.GetDirectoryName(fileNames[i]));
			}

			fstream.BaseStream.Seek(offLength[i].Item1, SeekOrigin.Begin);

			try
			{
				using(FileStream vfsStream = new FileStream(destinationPath + "\\" + fileNames[i], FileMode.Create))
				using(BinaryWriter bw = new BinaryWriter(vfsStream))
				{
					vfsStream.Write(fstream.ReadBytes((int)offLength[i].Item2), 0, (int)offLength[i].Item2);
					Console.Title = "MH Extractor " + (i + 1) + "/" + fileNames.Count;
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("Could not write the file: {0}.", destinationPath + "\\" + fileNames[i]);
				Console.WriteLine(e);
				Environment.Exit(0);
				return;
			}
		}
		fstream.Close();

		Console.Title = "MH Extractor - Done";
		Console.WriteLine("Extraction has been finished.");
		Console.ReadKey();
	}
}
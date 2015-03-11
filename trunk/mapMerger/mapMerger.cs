using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

class FileInfo
{
	public string filePath { get; set; }
	public int line { get; set; }
	public int row { get; set; }
}

static class mapMerger
{
	public static Dictionary<int, List<FileInfo>> files = new Dictionary<int, List<FileInfo>>();
	public static Dictionary<int, Point> mapSizeStart = new Dictionary<int, Point>();
	public static Dictionary<int, Point> mapSizeEnd = new Dictionary<int, Point>();

	[STAThread]
	static void Main(string[] args)
	{
		Console.Title = "MH Map Merger";
SelectInputFolder:
		Console.WriteLine("Select a folder with map images - \"data/effect/map\".");
		FolderBrowserDialog _fBD = new FolderBrowserDialog();
		DialogResult _openDataInf = DialogResult.None;
		while(_openDataInf != DialogResult.OK)
		{
			_openDataInf = _fBD.ShowDialog();
			if(_openDataInf == DialogResult.Cancel)
			{
				Environment.Exit(0);
			}
		}
		string[] _files = Directory.GetFiles(@_fBD.SelectedPath + "/", "d*x*");
		if(_files.Count() == 0)
		{
			Console.WriteLine("Looks like you've selected a wrong folder.");
			goto SelectInputFolder;
		}
		foreach(string i in Directory.GetFiles(@_fBD.SelectedPath + "/", "d*x*"))
		{
			addToMaps(Convert.ToInt32(Path.GetFileNameWithoutExtension(i).Substring(1, 3)), i);
		}
		Dictionary<int, List<FileInfo>> tempSort = new Dictionary<int, List<FileInfo>>();
		foreach(KeyValuePair<int, List<FileInfo>> entry in files)
		{
			tempSort[entry.Key] = entry.Value.OrderBy(w => w.row).ThenBy(w => w.line).ToList();
		}
		files = tempSort;
		Console.WriteLine("Selected \"{0}\" map folder with {1} items.", _fBD.SelectedPath, files.Count);
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
		//--
		var b = new Bitmap(128, 128);
		for(int h = 0;h < 128;h++)
			for(int g = 0;g < 128;g++)
				b.SetPixel(h, g, Color.Black);
		Image black = new Bitmap(b, new Size(128, 128));
		int index = 1;
		Console.Title = "MH Map Merger " + index + "/" + files.Count;
		foreach(KeyValuePair<int, List<FileInfo>> entry in files)
		{
			int x = (mapSizeEnd[entry.Key].X - mapSizeStart[entry.Key].X) + 1;
			int y = (mapSizeEnd[entry.Key].Y - mapSizeStart[entry.Key].Y) + 1;
			Image _container = new Bitmap(x * 128, y * 128);
			for(int i = 0;i < x;i++)
			{
				for(int z = 0;z < y;z++)
				{
					Image tmp = null;
					FileInfo tmpzzz = findFileInfo(entry.Key, z + mapSizeStart[entry.Key].Y, i + mapSizeStart[entry.Key].X);
					if(tmpzzz != null)
					{
						tmp = new Bitmap(tmpzzz.filePath);
						tmp = new Bitmap(tmp, new Size(128, 128));
					}
					else
					{
						tmp = black;
					}
					using(Graphics g = Graphics.FromImage(_container))
					{
						g.DrawImage(tmp, (i * 128), (z * 128));
					}
				}
			}
			Bitmap tempy = new Bitmap(_container);
			tempy.Save(destinationPath + "/m" + entry.Key + ".bmp");
			index++;
			Console.Title = "MH Map Merger " + index + "/" + files.Count;
		}

		Console.Title = "MH Map Merger - Done";
		Console.WriteLine("Extracted {0} map files.", files.Count);
		Console.ReadKey();
	}

	private static FileInfo findFileInfo(int map, int line, int row)
	{
		return files[map].Find(w => w.line == line && w.row == row);
	}

	private static void addToMaps(int map, string filePath)
	{
		if(Path.GetFileNameWithoutExtension(filePath).Contains("_"))
			return;
		if(!files.ContainsKey(map))
			files.Add(map, new List<FileInfo>());
		if(!mapSizeStart.ContainsKey(map))
			mapSizeStart.Add(map, new Point(-1, -1));
		if(!mapSizeEnd.ContainsKey(map))
			mapSizeEnd.Add(map, new Point(-1, -1));
		FileInfo fInfo = new FileInfo();
		fInfo.filePath = filePath;
		string fileName = Path.GetFileNameWithoutExtension(filePath);
		fInfo.line = Convert.ToInt32(fileName.Substring(fileName.IndexOf("z") + 1));
		fInfo.row = Convert.ToInt32(fileName.Substring(fileName.IndexOf("x") + 1, fileName.IndexOf("z") - fileName.IndexOf("x") - 1));
		countNewOffsetsForMap(map, new Point(fInfo.row, fInfo.line));
		files[map].Add(fInfo);
	}

	private static void countNewOffsetsForMap(int map, Point point)
	{
		if(mapSizeStart[map].X == -1) mapSizeStart[map] = point;
		if(mapSizeEnd[map].X == -1) mapSizeEnd[map] = point;
		if(mapSizeStart[map].X > point.X) mapSizeStart[map] = new Point(point.X, mapSizeStart[map].Y);
		if(mapSizeStart[map].Y > point.Y) mapSizeStart[map] = new Point(mapSizeStart[map].X, point.Y);
		if(mapSizeEnd[map].X < point.X) mapSizeEnd[map] = new Point(point.X, mapSizeEnd[map].Y);
		if(mapSizeEnd[map].Y < point.Y) mapSizeEnd[map] = new Point(mapSizeEnd[map].X, point.Y);
	}
}
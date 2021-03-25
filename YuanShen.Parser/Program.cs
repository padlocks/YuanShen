using System;
using System.Collections.Generic;
using System.IO;
using YuanShen.Core.Net;

namespace YuanShen.Parser
{
    class Program
    {
		static void Main(string[] args)
		{

			IEnumerable<string> files = Directory.EnumerateFiles(args[0]);
			foreach (string file in files)
			{
				Console.WriteLine($"----- Packet : {file} -----\n");
				FileStream fs = new FileStream(file, FileMode.Open);

				while (fs.Position != fs.Length)
				{
					GenshinPacket packet = new GenshinPacket(fs);
					Console.WriteLine(packet.Dump());
				}
			}
			Console.ReadLine();
		}
    }
}

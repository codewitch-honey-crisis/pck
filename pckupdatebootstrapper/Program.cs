using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Pck
{
	class Program
	{
		static void Main(string[] _args)
		{
			var exeName = _args[0];
			Console.Error.WriteLine("Downloading from {0}...",_args[1]);
			WebRequest wrq = WebRequest.Create(_args[1]);
			WebResponse wrs = wrq.GetResponse();
			using (var stm = wrs.GetResponseStream())
			{
				// equiv of this. but not available on this platform:
				//zip.ExtractToDirectory(Environment.CurrentDirectory, true);
				var zip = new ZipArchive(stm);
				Console.Error.WriteLine("done!");
				foreach (var entry in zip.Entries)
				{
					Console.Error.Write(string.Concat("Unzipping ", entry.Name, "..."));
					try
					{
						File.Delete(entry.FullName);
					}
					catch { }
					var d = Path.GetDirectoryName(entry.FullName);
					if (!string.IsNullOrEmpty(d))
					{
						try
						{
							Directory.CreateDirectory(d);
						}
						catch { }
					}
					using (var stm1 = entry.Open())
					using (var stm2 = File.OpenWrite(entry.FullName))
						stm1.CopyTo(stm2);
					Console.Error.WriteLine("done!");
				}
			}
			Console.Error.WriteLine("Cleaning up...");
			var psi = new ProcessStartInfo();
			var args = new StringBuilder();
			var delim = "";
			for (var i = 2; i < _args.Length; i++)
			{
				args.Append(delim);
				args.Append(_Esc(_args[i]));
				args.Append(' ');
			}
			psi.FileName = exeName;
			psi.Arguments = args.ToString();
			var proc = Process.Start(psi);
			
		}
		static string _Esc(string arg)
		{
			return string.Concat("\"", arg.Replace("\"", "\"\""), "\"");
		}
	}
}

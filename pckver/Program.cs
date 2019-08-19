using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Pck
{
	static class Program
	{
		
		static void _PrintUsage()
		{
			Console.Error.Write(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().GetModules()[0].Name));
			Console.Error.WriteLine(" [/update [<major>.<minor>.<build>.<revision>]");
			Console.Error.WriteLine();
			Console.Error.WriteLine("Updates the Pck binaries from GitHub or displays the available versions.");
			Console.Error.WriteLine();
		}
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static int Main(string[] args)
		{
			var isCurrentOffline = false;
			var current = Assembly.GetEntryAssembly().GetName().Version;
			var versions = new List<Version>();
			versions.AddRange(Updater.Versions);
			Version selected = default(Version);
			if (!versions.Contains(current))
			{
				versions.Add(current);
				isCurrentOffline = true;
			}
			versions.Sort();
			versions.Reverse();

			switch (args.Length)
			{
				case 0:
					_PrintUsage();
					Console.WriteLine("Current version: Pck v{0}", current);
					Console.WriteLine();
					Console.WriteLine("Available versions:");
					for (int ic = versions.Count, i = 0; i < ic; ++i)
					{
						var v = versions[i];
						Console.Write("\tPck v");
						Console.Write(v);
						if (isCurrentOffline && v == current)
							Console.Write(" (offline only)");

						Console.WriteLine();
					}
					break;
				case 1:
					if("/updated"==args[0])
					{
						Console.Error.WriteLine("done!");
						Updater.DeleteUpdaterFiles();
						Console.Error.Write("Successfully changed version to Pck v");
						Console.Error.WriteLine(Assembly.GetEntryAssembly().GetName().Version);
						return 0;
					}
					selected = Updater.LatestVersion;
					goto case 2;
				case 2:
					if("/update"!=args[0])
					{
						Console.Error.WriteLine("Invalid switch: " + args[0]);
						Console.Error.WriteLine();
						_PrintUsage();
						return 1;
					}
					if(default(Version)==selected)
					{
						if(!Version.TryParse(args[1],out selected))
						{
							Console.Error.WriteLine("Invalid version format: " + args[1]);
							Console.Error.WriteLine();
							_PrintUsage();
							return 1;
						}
					}
					if(!versions.Contains(selected))
					{
						Console.Error.WriteLine("The specified version was not found: " + args[1]);
						Console.Error.WriteLine();
						Console.Error.WriteLine("Available versions:");
						for (int ic = versions.Count, i = 0; i < ic; ++i)
						{
							var v = versions[i];
							Console.Error.Write("\tPck v");
							Console.Error.Write(v);
							if (isCurrentOffline && v == current)
								Console.Error.Write(" (offline only)");
							Console.Error.WriteLine();
						}
						return 1;
					}
					if(selected == current)
					{
						Console.Error.WriteLine("The version is already selected. There is nothing to do.");
						return 0;
					}
					_DoUpdate(current,selected);
					break;
			}
			return 0;
		}
		static void _DoUpdate(Version c,Version v)
		{
			Console.Error.WriteLine("Version change from Pck v{0} to Pck v{1} in progress...",c,v);
			Updater.Update(new string[] { "/updated" });
		}
	}
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Pck
{
	static class Program
	{
		static readonly string _name = Path.GetFileName(Assembly.GetExecutingAssembly().GetModules()[0].Name);
		static readonly string _path = Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (0 < args.Length)
			{
				var s = args[0].ToLowerInvariant();
				switch(s)
				{
					case "/install":
						var currentUser = true;
						if (1 < args.Length && "all"==args[1].ToLowerInvariant())
							currentUser = false;
						Install(currentUser);
						return;
					case "/uninstall":
						currentUser = true;
						if (1 < args.Length && "all"==args[1].ToLowerInvariant() )
							currentUser = false;
						Uninstall(currentUser);
						return;
				}
			}
			
			if (!IsInstalled)
				Install();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var main = new Main();
			if(0<args.Length)
				main.OpenFiles(args);
			Application.Run(main);

		}
		public static void Install(bool currentUserOnly=true) {

			var path = string.Concat(@"Software\Microsoft\Windows\CurrentVersion\App Paths\", _name);
			var root = currentUserOnly ? Registry.CurrentUser : Registry.LocalMachine;
			using (var key = root.CreateSubKey(path))
				key.SetValue(null, _path);
			path = @"Software\Classes\xbnffile\shell\command\open";
			using (var key = root.CreateSubKey(path))
				key.SetValue(null, string.Concat("\"", _path, "\" \"%1\""));
			path = @"Software\Classes\.xbnf";
			using (var key = root.CreateSubKey(path))
				key.SetValue(null, "xbnffile");

			path = @"Software\Classes\pckfile\shell\command\open";
			using (var key = root.CreateSubKey(path))
				key.SetValue(null, string.Concat("\"", _path, "\" \"%1\""));
			path = @"Software\Classes\.pck";
			using (var key = root.CreateSubKey(path))
				key.SetValue(null, "pckfile");


		}
		public static void Uninstall(bool currentUserOnly=true) {
			var path = string.Concat(@"Software\Microsoft\Windows\CurrentVersion\App Paths\", _name);
			var root = currentUserOnly ? Registry.CurrentUser : Registry.LocalMachine;
			root.DeleteSubKeyTree(path,false);
			path = @"Software\Classes\xbnffile";
			root.DeleteSubKeyTree(path,false);
			path = @"Software\Classes\.xbnf";
			root.DeleteSubKeyTree(path,false);
			path = @"Software\Classes\pckfile";
			root.DeleteSubKeyTree(path, false);
			path = @"Software\Classes\.pck";
			root.DeleteSubKeyTree(path, false);
		}
		public static bool IsInstalled {
			get {
				var root = Registry.CurrentUser;
				var key = root.OpenSubKey(string.Concat(@"Software\Microsoft\Windows\CurrentVersion\App Paths", _name));
				var result = false;
				if (null != key)
				{
					result = true;
					key.Dispose();
				}
				if(!result) // check the system path
				{
					root = Registry.LocalMachine;
					key = root.OpenSubKey(string.Concat(@"Software\Microsoft\Windows\CurrentVersion\App Paths", _name));
					if (null != key)
					{
						result = true;
						key.Dispose();
					}
				}
				return result;
			}

		}
	}
}

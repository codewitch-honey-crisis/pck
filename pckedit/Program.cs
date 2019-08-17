using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Pck
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var main = new Main();
			if(0<args.Length)
				main.OpenFiles(args);
			Application.Run(main);

		}
		
	}
}

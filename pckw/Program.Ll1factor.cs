using System;
using System.IO;

namespace Pck
{
	partial class Program
	{
		static int _Ll1Factor(string[] args)
		{
			TextReader inp = null;
			TextWriter outp = null;
			switch (args.Length)
			{
				case 0:
					inp = Console.In;
					outp = Console.Out;
					break;
				case 1:
					string lstr;
					if ("/?" == args[0] || "-?" == args[0] || "/help" == (lstr = args[0].ToLowerInvariant()) || "--help" == lstr)
					{
						Console.Error.Write("Usage: ");
						_PrintUsageLL1Factor();
						return 0;
					}
					inp = new StreamReader(args[0]);
					outp = Console.Out;
					break;
				case 2:
					inp = new StreamReader(args[0]);
					outp = new StreamWriter(args[1]);
					break;
				default:
					Console.Error.Write("Usage: ");
					_PrintUsageLL1Factor();
					return 1;
			}
			var buf = inp.ReadToEnd();
			inp.Close();
			var cfg = CfgDocument.Parse(buf);
			var lex = _RipLex(buf);
			var hasErrors = false;
			foreach (var msg in cfg.PrepareLL1())
			{
				if (CfgErrorLevel.Warning == msg.ErrorLevel || CfgErrorLevel.Error == msg.ErrorLevel)
				{
					Console.Error.WriteLine(msg);
					if (CfgErrorLevel.Error == msg.ErrorLevel)
						hasErrors = true;
				}
			}

			if (!hasErrors)
			{
				outp.Write(cfg);
				outp.Write(lex);
			}
			outp.Close();
			return hasErrors ? 1 : 0;
		}
	}
}

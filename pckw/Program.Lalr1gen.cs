
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	partial class Program
	{
		static int _Lalr1Gen(string[] args)
		{
			string specFile = null;
			string outFile = null;
			string @namespace = null;
			string @class = null;
			string language = null;// "c#";
			var optIndex = -1;
			for (var i = 0; i < args.Length; ++i)
			{
				if ("--help" == args[i] || "/?" == args[i] || "/help" == args[i])
				{
					Console.Error.Write("Usage: ");
					_PrintUsageLalr1Gen();
					return 0;
				}
				if (args[i].StartsWith("/"))
				{
					optIndex = i;
					if (i == args.Length - 1)
					{
						Console.Error.Write("Usage: ");
						_PrintUsageLL1Gen();
						return 1;
					}
					switch (args[i])
					{
						case "/language":
							++i;
							language = args[i];
							break;
						case "/namespace":
							++i;
							@namespace = args[i];
							break;
						case "/class":
							++i;
							@class = args[i];
							break;
						default:
							Console.Error.Write("Usage: ");
							_PrintUsageLL1Gen();
							return 1;
					}
				}
				else
				{
					if (-1 != optIndex)
					{
						Console.Error.Write("Usage: ");
						_PrintUsageLL1Gen();
						return 1;
					}
					if (0 == i)
						specFile = args[i];
					else if (1 == i)
						outFile = args[i];
					else
					{
						Console.Error.Write("Usage: ");
						_PrintUsageLL1Gen();
						return 1;
					}
				}
			}
			TextReader inp = null;
			TextWriter outp = null;
			try
			{
				if (null == specFile)
					inp = Console.In;
				else
					inp = new StreamReader(specFile);
				if (null == outFile)
					outp = Console.Out;
				else
					outp = new StreamWriter(outFile);

				var buf = inp.ReadToEnd();
				var cfg = CfgDocument.Parse(buf);
				var hasErrors = false;
				foreach (var msg in cfg.TryValidateLalr1())
				{
					Console.Error.WriteLine(msg);
					if (CfgErrorLevel.Error == msg.ErrorLevel)
						hasErrors = true;
				}
				if (!hasErrors)
				{
					if (string.IsNullOrEmpty(@class))
					{
						if (null != outFile)
							@class = Path.GetFileNameWithoutExtension(outFile);
						else if (null == @class && 0 < cfg.Rules.Count)
							@class = string.Concat(cfg.StartSymbol, "Parser");
						else
							@class = "Parser";
					}
					Lalr1ParserCodeGenerator.WriteClassTo(cfg, @class,@namespace, language, outp);
					return 0;
				}
				return 1;

			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				return 1;
			}
			finally
			{
				if (null != inp)
					inp.Close();
				if (null != outp)
					outp.Close();
			}
		}
	}
}


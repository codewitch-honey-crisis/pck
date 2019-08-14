using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	partial class Program
	{
		static int _Fagen(string[] args)
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
					_PrintUsageFaGen();
					return 0;
				}
				if (args[i].StartsWith("/"))
				{
					optIndex = i;
					if (i == args.Length - 1)
					{
						Console.Error.Write("Usage: ");
						_PrintUsageFaGen();
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
							_PrintUsage();
							return 1;
					}
				}
				else
				{
					if (-1 != optIndex)
					{
						Console.Error.Write("Usage: ");
						_PrintUsageFaGen();
						return 1;
					}
					if (0 == i)
						specFile = args[i];
					else if (1 == i)
						outFile = args[i];
					else
					{
						Console.Error.Write("Usage: ");
						_PrintUsageFaGen();

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
				var lex = LexDocument.Parse(buf);
				var cfg = CfgDocument.Parse(buf);
				var syms = cfg.FillSymbols();
				if (string.IsNullOrEmpty(@class))
				{
					if (null != outFile)
						@class = Path.GetFileNameWithoutExtension(outFile);
					else if (0 < syms.Count)
					{
						foreach (var attrs in lex.AttributeSets)
						{
							var i = attrs.Value.IndexOf("start");
							if (-1 < i && attrs.Value[i].Value is bool && (bool)attrs.Value[i].Value)
							{
								@class = string.Concat(attrs.Key, "Tokenizer");
								break;
							}
						}
						if (null == @class)
							@class = string.Concat(syms[0], "Tokenizer");
					}
				}
				TokenizerCodeGenerator.WriteClassTo(lex, syms, @class,@namespace, language, outp);
				outp.Flush();
				return 0;

			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				return 1;
			}
			finally
			{
				inp.Close();
				outp.Close();
			}
		}
	}
}

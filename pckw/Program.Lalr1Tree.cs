using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pck
{
	partial class Program
	{
		static int _Lalr1Tree(string[] args)
		{
			if (1 > args.Length || args.Length > 2)
			{
				_PrintUsageLalr1Tree();
				return 1;
			}
			var pckspec = args[0];
			string pck;
			using (var sr = File.OpenText(pckspec))
				pck = sr.ReadToEnd();

			var cfg = CfgDocument.Parse(pck);
			var lex = LexDocument.Parse(pck);

			var tokenizer = lex.ToTokenizer(
				(1 < args.Length) ? (TextReaderEnumerable)new FileReaderEnumerable(args[1]) :
					new ConsoleReaderEnumerable(),
				cfg.EnumSymbols(),new _TokenizerConsoleProgress());
			var parser = cfg.ToLalr1Parser(tokenizer,new _Lalr1ConsoleProgress());
			Console.Error.WriteLine();
			parser.ShowHidden = true;
			while (LRNodeType.EndDocument != parser.NodeType)
				Console.WriteLine(parser.ParseReductions());

			return 1;
		}
		class _Lalr1ConsoleProgress : IProgress<CfgLalr1Progress>
		{
			public void Report(CfgLalr1Progress progress)
			{
				switch (progress.Status)
				{
					case CfgLalr1Status.ComputingClosure:
					case CfgLalr1Status.ComputingMove:
					case CfgLalr1Status.ComputingConfigurations:
					case CfgLalr1Status.CreatingLookaheadGrammar:
						break;
					default:
						Console.Error.Write(".");
						break;
				}

			}

		}
	}
	
	
}
